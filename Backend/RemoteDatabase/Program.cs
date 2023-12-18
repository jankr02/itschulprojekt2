global using MesseauftrittDatenerfassung.Models;
global using MesseauftrittDatenerfassung.Services.CustomerService;
global using MesseauftrittDatenerfassung.Dtos.CustomerDtos;
global using MesseauftrittDatenerfassung.Dtos.PictureDtos;
global using MesseauftrittDatenerfassung.Dtos.ProductGroupDtos;
global using MesseauftrittDatenerfassung.Dtos.BusinessDtos;
global using MesseauftrittDatenerfassung.Enums;
global using MesseauftrittDatenerfassung.Converters;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using MesseauftrittDatenerfassung.Data;
global using MesseauftrittDatenerfassung.Dtos.CustomerProductGroupDto;
global using System.Drawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddDbContext<DataContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(typeof(Program).Assembly);
builder.Services.AddScoped<ICustomerService, CustomerService>();

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

app.Run();