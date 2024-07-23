using BookCollectionAPI;

public interface IBookStorageService
{
    Task AddBook(Book book);
    Task<IEnumerable<Book>> GetBooks();
    Task<Book> GetSingleBookOrNull(Guid id);
}

public class BookStorageService : IBookStorageService
{
    private readonly IDbMapper _dbMapper;

    public BookStorageService(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task AddBook(Book book)
    {
        var sql = "insert into books (id, title, author, type) values (@id, @title, @author, @type);";
        return _dbMapper.ExecuteNonQueryAsync(sql, book);
    }

    public Task<IEnumerable<Book>> GetBooks()
    {
        var sql = @"select * from books;";

        return _dbMapper.ExecuteListAsync<Book>(sql);
    }

    public Task<Book> GetSingleBookOrNull(Guid id)
    {
        var sql = @"select * from books where id = @id;";
    
        return _dbMapper.ExecuteObjectOrNullAsync<Book>(sql, new { id });
    }
}