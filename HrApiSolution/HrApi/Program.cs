using AutoMapper;
using HrApi;
using HrApi.Domain;
using HrApi.Profiles;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
// The default web application builder has about 195+ "Services" that do all the work in your API

// Add services to the container.

builder.Services.AddControllers(options =>
{
    // globally now, every controller will use this filter    
    options.Filters.Add<CancellationTokenExceptionFilter>();
}).AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "HR Api",
        Version = "v1",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Jeff Gonzalez",
            Email = "jeff@aol.com"
        },
        License = new Microsoft.OpenApi.Models.OpenApiLicense
        {
            Name = "MIT",
            Url = new Uri("https://opensource.org/license/mit/")
        }
    });

    var xmlFileName = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    c.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileName));
});

var connectionString = builder.Configuration.GetConnectionString("hr-data");

var someValue = builder.Configuration.GetValue<bool>("features:demo");

// IOption<FeaturesOptions>
builder.Services.Configure<FeaturesOptions>(
 builder.Configuration.GetSection(FeaturesOptions.FeatureName)
 );

Console.WriteLine($"Got this value for the limit {someValue}");

if (connectionString is null)
{
    throw new Exception("No connection string for HR database");
}

builder.Services.AddDbContext<HrDataContext>(options =>
{
    options.UseSqlServer(connectionString);
});

// "Slow" - so we are "eagerly" creating this at application startup.

var mapperConfiguration = new MapperConfiguration(options =>
{ 
    options.AddProfile<Departments>();
    options.AddProfile<HiringRequests>();
});

builder.Services.AddSingleton<IMapper>(mapperConfiguration.CreateMapper());
builder.Services.AddSingleton<MapperConfiguration>(mapperConfiguration);

// Lazy
builder.Services.AddScoped<IManageHiringRequests, EntityFrameworkHiringManager>();

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


//app.Use(async (context, next) => {
//    await Console.Out.WriteLineAsync($"Just got a request from {context.Request.Headers.UserAgent}");
//    await next();
//});
// app.Use(LoggingStuff.LogIt);
app.UseSuperLogging();

// Creates "phone directory"
app.MapControllers();
// Console.WriteLine("About to run the app.");

// Route Table:
//  If someone does a GET /departments:
//      Create an instance of the DepartmentsController
//          To create an instance of this, you have to give
//      Call the GetDepartments method
//  If someone does a GET /departments/(SOME INTEGER)
//      Create the DepartmentContoller and call GetById with

// Starting the web server and "blocking here"
app.Run();
// Console.WriteLine("Done running the app.");
