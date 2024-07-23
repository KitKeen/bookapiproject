namespace BookCollectionAPI;

public class Book
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Title { get; init; }
    public string Author { get; init; }
    public string Type { get; init; }
}

public class Rating
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public int Score { get; init; }
    public Guid BookId { get; init; }
}


public enum EBookType
{
    Mystery = 1,
    Fantasy = 2,
    ScienceFiction = 3,
    Romance = 4,
    Horror = 5
}