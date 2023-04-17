using AutoMapper;
using HrApi.Domain;
using HrApi.Profiles;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// The default web application builder has about 195+ "Services" that do all the work in your API

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connectionString = builder.Configuration.GetConnectionString("hr-data");

if (connectionString is null)
{
    throw new Exception("No connection string for HR database");
}

builder.Services.AddDbContext<HrDataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

var mapperConfiguration = new MapperConfiguration(options =>
{ 
    options.AddProfile<Departments>();
});

builder.Services.AddSingleton<IMapper>(mapperConfiguration.CreateMapper());
builder.Services.AddSingleton<MapperConfiguration>(mapperConfiguration);

// World before the application is built is above here.
var app = builder.Build();

// Everything after here the application is built.
// How do you want to handle the HTTP requests and responses

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Open API - the documentation
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();

// Creates "phone directory"
app.MapControllers();
// Console.WriteLine("About to run the app.");

// Route Table:
//  If someone does a GET /departments:
//      public async Task GetDepartments()
//          return Ok()

// Starting the web server and "blocking here"
app.Run();
// Console.WriteLine("Done running the app.");
