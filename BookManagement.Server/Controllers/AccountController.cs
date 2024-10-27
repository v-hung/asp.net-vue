using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using BookManagement.Server.Core.Services;
using BookManagement.Server.Core.Models;
using AutoMapper;
using BookManagement.Server.Core.Controllers;

namespace BookManagement.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class AccountController : AccountCoreController
{
    public AccountController(SignInManager<User> signInManager, UserManager<User> userManager, JwtTokenUtil jwtTokenUtil, EmailSender emailSender, IHttpContextAccessor httpContextAccessor, UploadFile uploadFile, IMapper mapper) : base(signInManager, userManager, jwtTokenUtil, emailSender, httpContextAccessor, uploadFile, mapper)
    {
    }
}