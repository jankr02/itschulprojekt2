global using RemoteDatabase.Models;
global using RemoteDatabase.Services.CustomerService;
global using RemoteDatabase.Dtos.CustomerDtos;
global using RemoteDatabase.Dtos.PictureDtos;
global using RemoteDatabase.Dtos.ProductGroupDtos;
global using RemoteDatabase.Dtos.BusinessDtos;
global using RemoteDatabase.Enums;
global using RemoteDatabase.Converters;
global using AutoMapper;
global using Microsoft.EntityFrameworkCore;
global using RemoteDatabase.Data;
global using RemoteDatabase.Dtos.CustomerProductGroupDto;
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