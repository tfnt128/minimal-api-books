using API.Domain.DTOs;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using API.Domain.Interfaces;
using API.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using API.Domain.Entities;
using API.Domain.ModelViews;

#region Services
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IBookService, BookService>();

// Add services to the container.
builder.Services.AddDbContext<BookManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
#endregion

#region Administrators
app.MapPost("/administrators/login", ([FromBody]LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Successful login");
    else
        return Results.Unauthorized();

}).WithTags("Administrators");
#endregion


ValidationErrors validDTO(BookDTO bookDTO)
{
    var validation = new ValidationErrors { 
    Errors = new List<string>()
    };

    if (string.IsNullOrEmpty(bookDTO.BookName))
        validation.Errors.Add("Book name is required.");

    if (string.IsNullOrEmpty(bookDTO.AuthorName))
        validation.Errors.Add("Author Name is required.");

    if (bookDTO.Year < 100)
        validation.Errors.Add("Books is too old, only a book of the year 100 or more are acceptable");

    return validation;
}
#region Books
app.MapPost("/books", ([FromBody] BookDTO bookDTO, IBookService bookService) =>
{    
    var validation = validDTO(bookDTO);

    if(validation.Errors.Count > 0)    
        return Results.BadRequest(validation);

    var book = new Book
    {
        BookName = bookDTO.BookName,
        AuthorName = bookDTO.AuthorName,
        Year = bookDTO.Year,
        Status = bookDTO.Status
    };
    bookService.AddBook(book);

    return Results.Created($"/books/{book.Id}", book);

}).WithTags("Books");
app.MapGet("/books", ([FromQuery]int? page, IBookService bookService) =>
{
    var books = bookService.GetAllBooks(page);

    return Results.Ok(books);

}).WithTags("Books");

app.MapGet("/books/{id}", ([FromRoute] int id, IBookService bookService) =>
{
    var book = bookService.GetBookById(id);

    if (book == null)
        return Results.NotFound();

    return Results.Ok(book);

}).WithTags("Books");

app.MapPut("/books/{id}", ([FromRoute] int id, BookDTO bookDTO ,IBookService bookService) =>
{
    var validation = validDTO(bookDTO);

    if (validation.Errors.Count > 0)
        return Results.BadRequest(validation);

    var book = bookService.GetBookById(id);

    if (book == null)
        return Results.NotFound();

    book.BookName = bookDTO.BookName;
    book.AuthorName = bookDTO.AuthorName;
    book.Year = bookDTO.Year;
    book.Status = bookDTO.Status;

    bookService.UpdateBook(book);

    return Results.Ok(book);

}).WithTags("Books");

app.MapDelete("/books/{id}", ([FromRoute] int id, IBookService bookService) =>
{
    var book = bookService.GetBookById(id);

    if (book == null)
        return Results.NotFound();

    bookService.DeleteBook(book);

    return Results.NoContent();

}).WithTags("Books");
#endregion

#region App
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();
#endregion
