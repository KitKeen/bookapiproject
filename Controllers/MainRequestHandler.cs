// Controllers/BooksController.cs
using Microsoft.AspNetCore.Mvc;

namespace BookCollectionAPI.Controllers;

[Route("api")]
[ApiController]
public class MainRequestHandler : ControllerBase
{
    private readonly IBookStorageService _bookStorageService;
    private readonly IRatingStorageService _ratingStorageService;

    public MainRequestHandler(IBookStorageService bookStorageService,
        IRatingStorageService ratingStorageService)
    {
        _bookStorageService = bookStorageService;
        _ratingStorageService = ratingStorageService;
    }

    [HttpPost("book/add")]
    public async Task<ActionResult<Book>> PostBook([FromBody]Book book)
    {
        if (string.IsNullOrEmpty(book.Title) || string.IsNullOrEmpty(book.Author))
        {
            return BadRequest("Title or Author is null or empty");
        }

        await _bookStorageService.AddBook(book);

        return Ok(book);
    }

    [HttpGet("books")]
    public async Task<ActionResult<IEnumerable<AllBooksWithRatingsOutContract>>> GetBooks()
    {
        var books = await _bookStorageService.GetBooks();

        var bookIds = books.Select(b => b.Id).ToArray();

        var ratings = await _ratingStorageService.GetRatings(bookIds);

        var result = new List<AllBooksWithRatingsOutContract>();

        foreach (var book in books)
        {
            result.Add(new AllBooksWithRatingsOutContract
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Type = book.Type,
                Ratings = ratings.Where(r => r.BookId == book.Id).ToArray()
            });
        }

        return Ok(result);
    }

    [HttpPost("search")]
    public async Task<ActionResult<IEnumerable<Book>>> SearchBooks([FromBody]SearchRequest contract)
    {
        if (contract.Query is null || contract.Query.Length < 3)
        {
            return BadRequest("Query is null or too short");
        }

        var books = 
        (await _bookStorageService
            .GetBooks())
            .Where(b => b.Title.Contains(contract.Query, StringComparison.OrdinalIgnoreCase));

        return Ok(books);
    }

    [HttpPost("rating/add")]
    public async Task<IActionResult> PostRating([FromBody]Rating rating)
    {
        var book = await _bookStorageService.GetSingleBookOrNull(rating.BookId);

        if (book is null)
        {
            return NotFound("Book is not found");
        }

        if (rating.Score < 1 || rating.Score > 5)
        {
            return BadRequest("Rating should be between 1 and 5 included");
        }

        await _ratingStorageService.AddRating(rating);

        return Ok(book);
    }
}


public class AllBooksWithRatingsOutContract : Book
{
    public Rating[] Ratings { get; init; } = Array.Empty<Rating>();
    public double AverageRating => Ratings.Any() ? Ratings.Average(r => r.Score) : 0;
    public int TotalRatings => Ratings.Length;
}

public class SearchRequest
{
    public string Query { get; set; }
}