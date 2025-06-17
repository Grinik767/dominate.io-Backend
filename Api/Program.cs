using Application;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddSwaggerGen();

services.AddSingleton<Storage>();
services.AddSingleton<CodeGenerator>();
services.AddSingleton<ConnectionManager>();

builder.Services.AddSingleton<IConnectionService>(sp =>
{
    var manager = sp.GetRequiredService<ConnectionManager>();
    return new ConnectionService(manager);
});

services.AddControllers();

var app = builder.Build();
app.UseWebSockets();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();