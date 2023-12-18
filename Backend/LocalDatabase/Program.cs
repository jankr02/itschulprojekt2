global using LocalDatabase.Models;
global using LocalDatabase.Services.CustomerService;
global using LocalDatabase.Dtos.CustomerDtos;
global using LocalDatabase.Dtos.PictureDtos;
global using LocalDatabase.Dtos.ProductGroupDtos;
global using LocalDatabase.Dtos.BusinessDtos;
global using LocalDatabase.Enums;
global using LocalDatabase.Converters;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using LocalDatabase.Data;
global using LocalDatabase.Dtos.CustomerProductGroupDto;
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