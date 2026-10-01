using Library.NTier.Repositories;
using Library.NTier.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton(TimeProvider.System);

// Every layer is registered by technical role.
builder.Services.AddSingleton<IBookRepository, InMemoryBookRepository>();
builder.Services.AddSingleton<ILoanRepository, InMemoryLoanRepository>();
builder.Services.AddScoped<IBookService, BookService>();
builder.Services.AddScoped<ILoanService, LoanService>();

var app = builder.Build();
app.MapControllers();
app.Run();
