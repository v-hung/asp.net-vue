using BookManagement.Server.Core.Authorization;
using BookManagement.Server.Core.Services;
using BookManagement.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class MenuController : Controller {

    private readonly ApplicationDbContext _context;
    private readonly MenuService _menuService;

    public MenuController(ApplicationDbContext context, MenuService menuService)
    {
        _context = context;
        _menuService = menuService;
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAllMenus([FromQuery] bool? tree = true)
    {
        var menus = await _menuService.GetMenuForCurrentUser(tree);

        return Ok(new
        {
            menus
        });
    }

}