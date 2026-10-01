using Library.VerticalSlice.Features.Books;
using Library.VerticalSlice.Features.Loans;
using Library.VerticalSlice.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<LibraryDb>();
builder.Services.AddSingleton(TimeProvider.System);

var app = builder.Build();

// One line per feature. Reading this list tells you what the application DOES.
ListBooks.MapEndpoint(app);
GetBook.MapEndpoint(app);
BorrowBook.MapEndpoint(app);
ReturnBook.MapEndpoint(app);

app.Run();
