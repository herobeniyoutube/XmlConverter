using Microsoft.AspNetCore.Mvc;
using XmlConverter.Web;
using XmlConverter.Web.Abstractions;
using XmlConverter.Web.Middleware;
using XmlConverter.Web.Services;
using XmlConverter.Web.XmlValidators.EmployersData;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IEmployeeDataStorage, EmployeeDataInMemoryStorage>();
builder.Services.AddTransient<IEmployersDataValidator, EmployersDataValidator>();
builder.Services.AddTransient<IConverterService, ConverterService>();

builder.Services.AddRazorPages(options =>
{
    options.Conventions.ConfigureFilter(new IgnoreAntiforgeryTokenAttribute());
});
var app = builder.Build();
app.MapRazorPages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();
