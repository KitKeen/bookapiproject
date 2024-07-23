using BookCollectionAPI;

public interface IRatingStorageService
{
    Task AddRating(Rating book);
    Task<IEnumerable<Rating>> GetRatings(Guid[] bookIds);
}

public class RatingStorageService : IRatingStorageService
{
    private readonly IDbMapper _dbMapper;

    public RatingStorageService(IDbMapper dbMapper)
    {
        _dbMapper = dbMapper;
    }

    public Task AddRating(Rating rating)
    {
        var sql = "insert into ratings (id, score, bookid) values (@id, @score, @bookid)";

        return _dbMapper.ExecuteNonQueryAsync(sql, rating);
    }

    public Task<IEnumerable<Rating>> GetRatings(Guid[] bookIds)
    {
        var sql = "select * from ratings where bookid = any(@bookIds)";

        return _dbMapper.ExecuteListAsync<Rating>(sql, new { bookIds } );
    }
}