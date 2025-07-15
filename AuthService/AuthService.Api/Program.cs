using AuthService.Application.Services;
using AuthService.Infra.Context;
using AuthService.Infra.Messaging;
using AuthService.Infra.Messaging.RabbitMq;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("AuthService");
builder.Host.UseSerilog((ctx, log) =>
    log.ReadFrom.Configuration(ctx.Configuration).WriteTo.Console());

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

builder.Services.AddDbContext<AuthContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<UserRegistrationService>();

builder.Services.AddSingleton<EventPublisher>();
builder.Services.AddSingleton<IEventPublisher>(sp => sp.GetRequiredService<EventPublisher>());

var app = builder.Build();

var rabbitPublisher = app.Services.GetRequiredService<EventPublisher>();
await rabbitPublisher.InitAsync(app.Configuration["RabbitMQ:Host"]);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();