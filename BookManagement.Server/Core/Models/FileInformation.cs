using System.ComponentModel.DataAnnotations;

namespace BookManagement.Server.Core.Models;
public class FileInformation
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Path { get; set; }

    public string? Extension { get; set; }

    [Required]
    public long Size { get; set; } = 0;

    public int ImageWidth { get; set; }

    public int ImageHeight { get; set; }

    public string FormatFileSize
    {
        get
        {
            const int scale = 1024;
            string[] orders = new string[] { "TB", "GB", "MB", "KB", "bytes" };
            long max = (long)Math.Pow(scale, orders.Length - 1);

            foreach (string order in orders)
            {
                if (Size > max)
                    return string.Format("{0:##.##} {1}", decimal.Divide(Size, max), order);

                max /= scale;
            }

            return "0 bytes";
        }
    }
}