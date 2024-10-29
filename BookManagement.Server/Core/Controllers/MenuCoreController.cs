using BookManagement.Server.Core.Authorization;
using BookManagement.Server.Core.Models;
using BookManagement.Server.Core.Services;
using BookManagement.Server.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Server.Core.Controllers;

[ApiController]
public abstract class MenuCoreController : Controller {
    private readonly MenuService _menuService;

    public MenuCoreController(MenuService menuService)
    {
        _menuService = menuService;
    }

    [HttpGet]
    [Authorize]
    [ProducesResponseType(typeof(List<MenuItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllMenus([FromQuery] bool? tree = true)
    {
        var menus = await _menuService.GetMenuForCurrentUser(tree);

        return Ok(menus);
    }

}