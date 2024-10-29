using BookManagement.Server.Core.Authorization;
using BookManagement.Server.Core.Controllers;
using BookManagement.Server.Core.Services;
using BookManagement.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MenuController : MenuCoreController {

    public MenuController(MenuService menuService) : base(menuService)
    {
    }
}