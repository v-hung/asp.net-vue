using BookManagement.Server.Core.Authorization;
using BookManagement.Server.Core.Models;
using BookManagement.Server.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookManagement.Server.Controllers;

[ApiController]
[Route("/api/[controller]")]
public class BookController : Controller {

    [HttpGet]
    [Permission("Book", PermissionType.View)]
    [ProducesResponseType<Book>(StatusCodes.Status200OK)]
    public IActionResult GetAllBooks()
    {
        var books = new List<Book>();
        for (int i = 0; i < 10; i++)
        {
            books.Add(new Book
            {
                Id = Guid.NewGuid(),
                Title = $"Book {i}",
            });
        }

        return Ok(books);
    }

}