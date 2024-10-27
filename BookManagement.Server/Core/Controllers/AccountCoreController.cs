using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.AspNetCore.Identity.UI.Services;
using System.Text.Encodings.Web;
using System.ComponentModel.DataAnnotations;
using BookManagement.Server.Core.Services;
using BookManagement.Server.Core.Models;
using AutoMapper;
using BookManagement.Server.Core.Dto;
using BookManagement.Server.Core.Requests;
using BookManagement.Server.Core.Responses;

namespace BookManagement.Server.Core.Controllers;

public abstract class AccountCoreController : Controller
{
    private readonly SignInManager<User> _signInManager;
    private readonly UserManager<User> _userManager;
    private readonly JwtTokenUtil _jwtTokenUtil;
    private readonly EmailSender _emailSender;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly UploadFile _uploadFile;
    private readonly IMapper _mapper;

    public AccountCoreController(SignInManager<User> signInManager, UserManager<User> userManager, JwtTokenUtil jwtTokenUtil, EmailSender emailSender, IHttpContextAccessor httpContextAccessor, UploadFile uploadFile, IMapper mapper)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _jwtTokenUtil = jwtTokenUtil;
        _emailSender = emailSender;
        _httpContextAccessor = httpContextAccessor;
        _uploadFile = uploadFile;
        _mapper = mapper;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest input)
    {
        var user = await _userManager.FindByNameAsync(input.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Tài khoản không tồn tại" });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, input.Password, false);
        if (!result.Succeeded)
        {
            return Unauthorized(new { message = "Mật khẩu không đúng", error = result });
        }

        _jwtTokenUtil.RevokeExpiredRefreshTokens(user);

        // Tạo JWT token
        var token = _jwtTokenUtil.GenerateJwtToken(user);

        var refreshToken = _jwtTokenUtil.GenerateRefreshTokenModel();
        user.RefreshTokens.Add(refreshToken);
        await _userManager.UpdateAsync(user);

        // Sử dụng SignInManager để tạo cookie xác thực cho admin, api thì không cần
        // await _signInManager.SignInAsync(user, isPersistent: false);

        // Tạo phản hồi đăng nhập
        var response = new
        {
            Token = token,
            RefreshToken = refreshToken.Token,
            User = _mapper.Map<UserDto>(user)
        };

        return Ok(response);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest input)
    {
        var user = new User
        {
            UserName = input.Email,
            Email = input.Email,
            FullName = input.FullName 
        };

        var result = await _userManager.CreateAsync(user, input.Password);

        if (result.Succeeded)
        {
            // var userId = await _userManager.GetUserIdAsync(user);
            // var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            // code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

            // var request = _httpContextAccessor.HttpContext?.Request;
            // var host = request?.Host.Value; // Lấy domain hoặc localhost hiện tại
            // var scheme = request?.Scheme;

            // var callbackUrl = $"{scheme}://{host}/account/ConfirmEmail?userId={userId}&code={code}";

            // await _emailSender.SendEmailAsync(input.Email, "Xác nhận email của bạn",
            //   $"Vui lòng xác nhận tài khoản của bạn bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Ấn vào đây</a>.");

            return Ok(new { message = "User registered successfully" });
        }

        return BadRequest(new { errors = result });
    }

    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest input)
    {
        // get token from header "Authorization"
        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return BadRequest("Invalid client request");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var principal = _jwtTokenUtil.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity?.Name ?? "";

        var user = await _userManager.FindByNameAsync(username);
        if (user == null || !_jwtTokenUtil.IsRefreshTokenValid(user, input.RefreshToken))
        {
            return BadRequest("Invalid client request");
        }

        var newToken = _jwtTokenUtil.GenerateJwtToken(user);
        _jwtTokenUtil.RefreshTokenAsync(user, input.RefreshToken);

        return Ok(new RefreshResponse
        {
            Token = newToken,
            RefreshToken = input.RefreshToken
        });
    }


    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] LogoutRequest input)
    {
        // await _signInManager.SignOutAsync();

        var authHeader = Request.Headers["Authorization"].ToString();
        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
        {
            return BadRequest("Invalid client request");
        }

        var token = authHeader.Substring("Bearer ".Length).Trim();

        var principal = _jwtTokenUtil.GetPrincipalFromExpiredToken(token);
        var username = principal.Identity?.Name ?? "";

        var user = await _userManager.FindByNameAsync(username);

        if (input.RefreshToken != null && user != null)
        {
            _jwtTokenUtil.RevokeRefreshToken(user, input.RefreshToken);
        }

        return Ok(new { message = "Đăng xuất thành công" });
    }

    [HttpPost("update")]
    [Authorize]
    public async Task<IActionResult> Update([FromForm] UpdateUserRequest input)
    {
        var user = await _userManager.GetUserAsync(User);
        return Unauthorized(new { message = "Tài khoản không tồn tại" });

        // if (user == null)
        // {
        //   return Unauthorized(new { message = "Tài khoản không tồn tại" });
        // }

        // if (!string.IsNullOrEmpty(input.FullName)) {
        //   user.FullName = input.FullName;
        // }

        // if (!string.IsNullOrEmpty(input.Address)) {
        //   user.Address = input.Address;
        // }

        // if (!string.IsNullOrEmpty(input.Phone)) {
        //   user.PhoneNumber = input.Phone;
        // }

        // if (input.File != null) {
        //   FileInformation fileInfo = await _uploadFile.UploadSingle(input.File, "User");
        //   user.Image = fileInfo.Path;
        // }

        // var result = await _userManager.UpdateAsync(user);

        // if (!result.Succeeded) {
        //   return BadRequest(new { message = "Không thể cập nhập thông tin tài khoản" });
        // }

        // if (!string.IsNullOrEmpty(input.CurrentPassword) && !string.IsNullOrEmpty(input.NewPassword)) {
        //   var changePasswordResult = await _userManager.ChangePasswordAsync(user, input.CurrentPassword, input.NewPassword);

        //   if (!changePasswordResult.Succeeded) {
        //     return BadRequest(new { message = "Không thể cập nhập mật khẩu tin tài khoản" });
        //   }

        //   await _signInManager.RefreshSignInAsync(user);
        // }

        // return Ok(new UserDto() {
        //   Id = user.Id,
        //   Address = user.Address,
        //   CreatedAt = user.CreatedAt,
        //   Email = user.Email,
        //   EmailConfirmed = user.EmailConfirmed,
        //   FullName = user.FullName,
        //   Image = user.Image,
        //   PhoneNumber = user.PhoneNumber,
        //   UpdatedAt = user.UpdatedAt
        // });
    }

    [HttpGet("current-user")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var user = await _userManager.GetUserAsync(User);

        return Ok(new
        {
            User = user != null ? _mapper.Map<UserDto>(user) : null
        });
    }

    [HttpPost("reSendEmailConfirm")]
    public async Task<IActionResult> ReSendEmailConfirm([FromBody] ReSendEmailConfirmRequest input)
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
        var host = request?.Host.Value;
        var scheme = request?.Scheme;

        var callbackUrl = $"{scheme}://{host}/account/ConfirmEmail?userId={userId}&code={code}";

        await _emailSender.SendEmailAsync(input.Email, "Xác nhận email của bạn",
          $"Vui lòng xác nhận tài khoản của bạn bằng cách <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>Ấn vào đây</a>.");

        return Ok(new { message = "Vui lòng vào email để xác nhận tải khoản" });
    }

    [HttpPost("ConfirmEmail")]
    public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailRequest input)
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