using API.Domain.DTOs;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using API.Domain.Interfaces;
using API.Domain.Services;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IAdministratorService, AdministratorService>();

// Add services to the container.
builder.Services.AddDbContext<BookManagementContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("RemoteConnection")));


builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapPost("/login", ([FromBody]LoginDTO loginDTO, IAdministratorService administratorService) =>
{
    if (administratorService.Login(loginDTO) != null)
        return Results.Ok("Successful login");
    else
        return Results.Unauthorized();

});


app.Run();

