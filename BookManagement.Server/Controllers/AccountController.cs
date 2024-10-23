using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Identity.Data;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;
using BookManagement.Server.Services;

namespace BookManagement.Server.Controllers;

[Area("Api")]
[ApiController]
[Route("/api/[controller]")]
public class AccountController : Controller
{
  private readonly SignInManager<User> _signInManager;
  private readonly UserManager<User> _userManager;
  private readonly JwtTokenUtil _jwtTokenUtil;
  private readonly IEmailSender _emailSender;
  private readonly IHttpContextAccessor _httpContextAccessor;
  private readonly UploadFile _uploadFile;

  public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, JwtTokenUtil jwtTokenUtil, IEmailSender emailSender, IHttpContextAccessor httpContextAccessor, UploadFile uploadFile)
  {
    _signInManager = signInManager;
    _userManager = userManager;
    _jwtTokenUtil = jwtTokenUtil;
    _emailSender = emailSender;
    _httpContextAccessor = httpContextAccessor;
    _uploadFile = uploadFile;
  }

  [HttpPost("login")]
  public async Task<IActionResult> Login([FromBody] LoginRequest loginRequest)
  {
    var user = await _userManager.FindByNameAsync(loginRequest.Email);
    if (user == null)
    {
      return Unauthorized(new { message = "Tài khoản không tồn tại" });
    }

    var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
    if (!result.Succeeded)
    {
      if (result.IsNotAllowed)
      {
        return Unauthorized(new { message = "Email chưa được xác nhận", error = result });
      }

      return Unauthorized(new { message = "Mật khẩu không đúng", error = result });
    }

    // Tạo JWT token
    var token = _jwtTokenUtil.GenerateJwtToken(user);

    var refreshToken = _jwtTokenUtil.GenerateRefreshToken();
    user.RefreshToken = refreshToken;
    user.RefreshTokenExpiryTime = DateTime.Now.AddDays(7);

    await _userManager.UpdateAsync(user);

    // Sử dụng SignInManager để tạo cookie xác thực cho admin
    await _signInManager.SignInAsync(user, isPersistent: false);

    // Tạo phản hồi đăng nhập
    var response = new
    {
      Token = token,
      RefreshToken = refreshToken,
      User = new UserDto
      {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        EmailConfirmed = user.EmailConfirmed,
        Address = user.Address,
        Image = user.Image,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
      }
    };

    return Ok(response);
  }

  [HttpPost("register")]
  public async Task<IActionResult> Register([FromBody] RegisterApiModel registerRequest)
  {
    var user = new User
    {
      UserName = registerRequest.Email,
      Email = registerRequest.Email,
      FullName = registerRequest.FullName
    };

    var result = await _userManager.CreateAsync(user, registerRequest.Password);

    if (result.Succeeded)
    {
      var userId = await _userManager.GetUserIdAsync(user);
      var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
      code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

      var request = _httpContextAccessor.HttpContext?.Request;
      var host = request?.Host.Value; // Lấy domain hoặc localhost hiện tại
      var scheme = request?.Scheme;

      var callbackUrl = $"{scheme}://{host}/account/ConfirmEmail?userId={userId}&code={code}";

      await _emailSender.SendEmailAsync(registerRequest.Email, "Xác nhận email của bạn",
        $"Vui lòng xác nhận tài khoản của bạn bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Ấn vào đây</a>.");

      return Ok(new { message = "User registered successfully" });
    }

    return BadRequest(new { errors = result });
  }

  [HttpPost("refresh")]
  public async Task<IActionResult> Refresh([FromBody] TokenApiModel tokenApiModel)
  {
    if (tokenApiModel is null)
    {
      return BadRequest("Invalid client request");
    }

    var principal = _jwtTokenUtil.GetPrincipalFromExpiredToken(tokenApiModel.Token);
    var username = principal.Identity?.Name ?? "";

    var user = await _userManager.FindByNameAsync(username);
    if (user == null || user.RefreshToken != tokenApiModel.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
    {
      return BadRequest("Invalid client request");
    }

    var newToken = _jwtTokenUtil.GenerateJwtToken(user);
    var newRefreshToken = _jwtTokenUtil.GenerateRefreshToken();

    user.RefreshToken = newRefreshToken;
    await _userManager.UpdateAsync(user);

    return Ok(new
    {
      Token = newToken,
      RefreshToken = newRefreshToken
    });
  }

  
  [HttpPost("logout")]
  public async Task<IActionResult> Logout() {
    await _signInManager.SignOutAsync();

    return Ok(new { message = "Đăng xuất thành công" });
  }

  [HttpPost("update")]
  [Authorize]
  public async Task<IActionResult> Update([FromForm] UpdateApiModel input) {
    var user = await _userManager.GetUserAsync(User);

    if (user == null)
    {
      return Unauthorized(new { message = "Tài khoản không tồn tại" });
    }

    if (!string.IsNullOrEmpty(input.FullName)) {
      user.FullName = input.FullName;
    }

    if (!string.IsNullOrEmpty(input.Address)) {
      user.Address = input.Address;
    }

    if (!string.IsNullOrEmpty(input.Phone)) {
      user.PhoneNumber = input.Phone;
    }
    
    if (input.File != null) {
      FileInformation fileInfo = await _uploadFile.UploadSingle(input.File, "User");
      user.Image = fileInfo.Path;
    }

    var result = await _userManager.UpdateAsync(user);

    if (!result.Succeeded) {
      return BadRequest(new { message = "Không thể cập nhập thông tin tài khoản" });
    }

    if (!string.IsNullOrEmpty(input.CurrentPassword) && !string.IsNullOrEmpty(input.NewPassword)) {
      var changePasswordResult = await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);

      if (!changePasswordResult.Succeeded) {
        return BadRequest(new { message = "Không thể cập nhập mật khẩu tin tài khoản" });
      }

      await _signInManager.RefreshSignInAsync(user);
    }

    return Ok(new UserDto() {
      Id = user.Id,
      Address = user.Address,
      CreatedAt = user.CreatedAt,
      Email = user.Email,
      EmailConfirmed = user.EmailConfirmed,
      FullName = user.FullName,
      Image = user.Image,
      PhoneNumber = user.PhoneNumber,
      UpdatedAt = user.UpdatedAt
    });
  }

  [HttpGet("current-user")]
  [Authorize]
  public async Task<IActionResult> GetCurrentUser()
  {
    var user = await _userManager.GetUserAsync(User);

    return Ok(new
    {
      User = user != null ? new UserDto
      {
        Id = user.Id,
        Email = user.Email,
        FullName = user.FullName,
        PhoneNumber = user.PhoneNumber,
        EmailConfirmed = user.EmailConfirmed,
        Address = user.Address,
        Image = user.Image,
        CreatedAt = user.CreatedAt,
        UpdatedAt = user.UpdatedAt
      } : null
    });
  }

  [HttpPost("reSendEmailConfirm")]
  public async Task<IActionResult> ReSendEmailConfirm([FromBody] ReSendEmailApiModel input)
  {
    var user = await _userManager.FindByEmailAsync(input.Email);

    if (user == null)
    {
      return Unauthorized(new { message = "Tài khoản không tồn tại" });
    }

    var userId = await _userManager.GetUserIdAsync(user);
    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

    var request = _httpContextAccessor.HttpContext?.Request;
    var host = request?.Host.Value; // Lấy domain hoặc localhost hiện tại
    var scheme = request?.Scheme;

    var callbackUrl = $"{scheme}://{host}/account/ConfirmEmail?userId={userId}&code={code}";

    await _emailSender.SendEmailAsync(input.Email, "Xác nhận email của bạn",
      $"Vui lòng xác nhận tài khoản của bạn bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Ấn vào đây</a>.");

    return Ok(new { message = "Vui lòng vào email để xác nhận tải khoản" });
  }

  [HttpPost("ConfirmEmail")]
  public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailApiModel input)
  {
    var user = await _userManager.FindByIdAsync(input.UserId);
    if (user == null)
    {
      return Unauthorized(new { message = "Tài khoản không tồn tại" });
    }

    var code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(input.Code));
    var result = await _userManager.ConfirmEmailAsync(user, code);

    if (!result.Succeeded)
    {
      return BadRequest(new { message = "Không thành công vui lòng thử lại" });
    }

    return Ok(new { message = "Tài khoản của bạn đã xác nhận email" });
  }
}

public class TokenApiModel
{
  public required string Token { get; set; }
  public required string RefreshToken { get; set; }
}

public class ReSendEmailApiModel
{
  public required string Email { get; set; }
}

public class ConfirmEmailApiModel
{
  public required string UserId { get; set; }
  public required string Code { get; set; }
}

public class RegisterApiModel
{
  public required string Email { get; set; }
  public required string Password { get; set; }
  public required string FullName { get; set; }
}

public class UpdateApiModel
{ 
  public string? FullName { get; set; }
  public string? Address { get; set; }
  public string? Phone { get; set; }
  public string? CurrentPassword { get; set; }
  public string? NewPassword { get; set; }
  public IFormFile? File { get; set; }
}