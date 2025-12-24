using DotnetBaseKit.Components.Api;
using DotnetBaseKit.Components.Application;
using DotnetBaseKit.Components.Infra.Sql;
using ProductService.Application.Extensions;
using ProductService.Infra.Context;
using ProductService.Infra.Extensions;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();
builder.Services.AddApplicationServices();

builder.Services.AddApi();
builder.Services.AddApplication();
builder.Services.AddRepositories();


builder.Services.AddDbContext<ProductContext>(configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", context =>
{
    context.Response.Redirect("/swagger");
    return Task.CompletedTask;
});

app.UseHttpsRedirection();
app.MapControllers();
app.Run();