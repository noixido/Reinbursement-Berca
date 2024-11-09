using API.Context;
using API.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Menambahkan DbContext dengan Connection String
builder.Services.AddDbContext<MyContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Reimbursement")).EnableSensitiveDataLogging());

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

builder.Services.AddScoped<AccountRepository>();

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
