namespace Library.NTier.Dtos;

// One DTO shared by "list" and "get" - so both endpoints pay for fields only one of them needs.
public record BookDto(int Id, string Title, string Author, bool IsAvailable, DateOnly? DueBackOn);
