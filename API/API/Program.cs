using API.Domain.DTOs;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using API.Domain.Interfaces;
using API.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using API.Domain.Entities;
using API.Domain.ModelViews;
using API.Domain.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authorization;

#region Services
var builder = WebApplication.CreateBuilder(args);

var key = builder.Configuration.GetSection("Jwt").ToString();

if (string.IsNullOrEmpty(key)) key = "123456";

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(option =>
{
    option.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateLifetime = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
        ValidateIssuer = false,
        ValidateAudience = false,
    };
});

builder.Services.AddScoped<IAdministratorService, AdministratorService>();
builder.Services.AddScoped<IBookService, BookService>();

// Add services to the container.
builder.Services.AddDbContext<BookManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        In = ParameterLocation.Header,
        Description = "Insert JWT Token here"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var app = builder.Build();


#endregion

#region Administrators

string GenerateJwtToken(Administrator administrator)
{
    if (string.IsNullOrEmpty(key)) return string.Empty;

    var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
    var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);


    var claims = new List<Claim>
    {
        new Claim("Email", administrator.Email),       
        new Claim("Profile", administrator.Profile),
        new Claim(ClaimTypes.Role, administrator.Profile)
    };
    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddDays(1),
        signingCredentials: credentials
        );

    return new JwtSecurityTokenHandler().WriteToken(token);
}

app.MapPost("/administrators/login", ([FromBody]LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    var adm = administratorService.Login(loginDTO);
    if (adm != null)
    {
        string token = GenerateJwtToken(adm);
        return Results.Ok(new AdministratorLogged
        {
            Email = adm.Email,
            Profile = adm.Profile,
            Token = token
        });
    }
    else
        return Results.Unauthorized();

}).AllowAnonymous().WithTags("Administrators");

app.MapPost("/administrators", ([FromBody] AdministratorDTO adminstratorDTO , IAdministratorService administratorService) =>
{
    var validation = new ValidationErrors { 
        Errors = new List<string>()
    };

    if(string.IsNullOrEmpty(adminstratorDTO.Email))
        validation.Errors.Add("Email is required.");

    if (string.IsNullOrEmpty(adminstratorDTO.Password))
        validation.Errors.Add("Password is required.");

    if (adminstratorDTO.Profile == null)
        validation.Errors.Add("Profile is required.");

    if (validation.Errors.Count > 0)
        return Results.BadRequest(validation);

    Profile profile = (Profile)adminstratorDTO.Profile;

    var adm = new Administrator
    {
        Email = adminstratorDTO.Email,
        Password = adminstratorDTO.Password,
        Profile = profile.ToString()
    };
    administratorService.AddAdministrator(adm);

    return Results.Created($"/administrators/{adm.Id}", new AdministratorModelView
    {
        Id = adm.Id,
        Email = adm.Email,
        Profile = adm.Profile
    });

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administrators");

app.MapGet("/administrators", ([FromQuery] int? page, IAdministratorService administratorService) =>
{
    var adms = new List<AdministratorModelView>();
    var administrators = administratorService.GetAllAdministrators(page);

    foreach (var adm in administrators)
    {
        adms.Add(new AdministratorModelView
        {
            Id = adm.Id,
            Email = adm.Email,
            Profile = adm.Profile
        });
    }

    return Results.Ok(adms);
}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm"})
.WithTags("Administrators");

app.MapGet("/administrators/{id}", ([FromRoute] int id, IAdministratorService administratorService) =>
{
    var adm = administratorService.GetAdministratorById(id);

    if(adm == null)
        return Results.NotFound();

    return Results.Ok(new AdministratorModelView
    {
        Id = adm.Id,
        Email = adm.Email,
        Profile = adm.Profile
    });

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Administrators");
#endregion

#region Validation
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
#endregion

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

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.WithTags("Books");

app.MapGet("/books", ([FromQuery]int? page, IBookService bookService) =>
{
    var books = bookService.GetAllBooks(page);

    return Results.Ok(books);

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.WithTags("Books");

app.MapGet("/books/{id}", ([FromRoute] int id, IBookService bookService) =>
{
    var book = bookService.GetBookById(id);

    if (book == null)
        return Results.NotFound();

    return Results.Ok(book);

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm,Editor" })
.WithTags("Books");

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

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Books");

app.MapDelete("/books/{id}", ([FromRoute] int id, IBookService bookService) =>
{
    var book = bookService.GetBookById(id);

    if (book == null)
        return Results.NotFound();

    bookService.DeleteBook(book);

    return Results.NoContent();

}).RequireAuthorization()
.RequireAuthorization(new AuthorizeAttribute { Roles = "Adm" })
.WithTags("Books");
#endregion

#region App
// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.UseAuthentication();
app.UseAuthorization();

app.Run();
#endregion
