using BookManagement.Server.Core.Authorization;
using BookManagement.Server.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BookController : Controller {

    [HttpGet]
    [Permission("Report3", PermissionType.View)]
    public async Task<IActionResult> GetAllBooks()
    {
        return Ok();
    }

}