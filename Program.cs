using BookCollectionAPI.Controllers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddScoped<IDbMapper, DbMapper>();
builder.Services.AddScoped<IBookStorageService, BookStorageService>();
builder.Services.AddScoped<IRatingStorageService, RatingStorageService>();
builder.Services.AddScoped<SqlExecutor>();
builder.Services.AddScoped<MainRequestHandler>();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();