using SixLabors.ImageSharp;
using BookManagement.Server.core.Models;

namespace BookManagement.Server.core.Services;

public class UploadFile
{
    private static string UPLOAD_FOLDER_NAME = "uploads";
    private static readonly string[] ImageExtensions = ["jpg", "jpeg", "png", "gif", "bmp", "svg", "webp"];
    private static readonly string[] AudioExtensions = ["mp3", "wav", "ogg", "flac", "aac", "wma"];
    private static readonly string[] VideoExtensions = ["mp4", "avi", "mov", "wmv", "mkv", "flv"];
    private readonly IWebHostEnvironment _webHostEnvironment;

    public UploadFile(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public List<FileInformation> GetFiles(string? path = null)
    {
        string uploadsFolderPath = string.IsNullOrEmpty(path) ? UPLOAD_FOLDER_NAME : $"{UPLOAD_FOLDER_NAME}/{path}";

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolderPath);

        List<FileInformation> filesInfo = [];

        if (Directory.Exists(uploadsFolder))
        {
            string[] fileList = Directory.GetFiles(uploadsFolder);

            foreach (string filePath in fileList)
            {
                FileInfo fileInfo = new FileInfo(filePath);

                string Name = Path.GetFileName(filePath);

                FileInformation fileInformation = new FileInformation()
                {
                    Name = Name,
                    Path = $"/{uploadsFolderPath}/{Name}",
                    Size = fileInfo.Length,
                    Extension = Path.GetExtension(filePath)?.ToLower().Substring(1)
                };

                if (ImageExtensions.Contains(fileInformation.Extension))
                {
                    using (var image = Image.Load(filePath))
                    {
                        fileInformation.ImageWidth = image.Width;
                        fileInformation.ImageHeight = image.Height;
                    }
                }

                filesInfo.Add(fileInformation);
            }
        }

        return filesInfo;
    }

    public async Task<FileInformation> UploadSingle(IFormFile file, string? path = null)
    {
        string? extension = Path.GetExtension(file.FileName)?.ToLower().Substring(1);

        if (string.IsNullOrEmpty(extension) ||
            !IsMediaExtension(extension))
        {
            throw new Exception("Tệp tin không hợp lệ. Vui lòng tải lên ảnh, âm thanh hoặc video.");
        }

        string uploadsFolderPath = string.IsNullOrEmpty(path) ? UPLOAD_FOLDER_NAME : $"{UPLOAD_FOLDER_NAME}/{path}";

        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, uploadsFolderPath);

        if (!Directory.Exists(uploadsFolder))
        {
            Directory.CreateDirectory(uploadsFolder);
        }

        string fileName = Path.GetFileName(file.FileName);
        string filePath = Path.Combine(uploadsFolder, fileName);

        using (var stream = File.Create(filePath))
        {
            await file.CopyToAsync(stream);
        }

        FileInfo fileInfo = new FileInfo(filePath);

        FileInformation fileInformation = new FileInformation()
        {
            Name = fileName,
            Path = $"/{uploadsFolderPath}/{fileName}",
            Size = fileInfo.Length,
            Extension = Path.GetExtension(filePath)?.ToLower().Substring(1)
        };

        if (ImageExtensions.Contains(fileInformation.Extension))
        {
            using (var image = Image.Load(filePath))
            {
                fileInformation.ImageWidth = image.Width;
                fileInformation.ImageHeight = image.Height;
            }
        }

        return fileInformation;
    }

    public async Task<List<FileInformation>> UploadMultiple(List<IFormFile> files, string? path = null)
    {
        var uploadTasks = new List<Task<FileInformation>>();

        foreach (var file in files)
        {
            // Thêm mỗi task vào danh sách
            uploadTasks.Add(UploadSingle(file, path));
        }

        FileInformation[] results = await Task.WhenAll(uploadTasks);

        return results.ToList();
    }

    public void DeleteFile(string path)
    {
        if (path.StartsWith("/uploads/"))
        {
            path = path.Remove(0, "/uploads/".Length);
        }

        // để bắt buộc chỉ xóa trong thư mục uploads
        string filePath = Path.Combine(_webHostEnvironment.WebRootPath, $"{UPLOAD_FOLDER_NAME}/{path}");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
        else if (Directory.Exists(filePath))
        {
            Directory.Delete(filePath, true);
        }
    }

    private bool IsMediaExtension(string extension)
    {
        return ImageExtensions.Contains(extension) ||
            AudioExtensions.Contains(extension) ||
            VideoExtensions.Contains(extension);
    }
}