using Application.Commands;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
services.AddSwaggerGen();

services.AddSingleton<Storage>();
services.AddSingleton<CodeGenerator>();
services.AddSingleton<ConnectionManager>();

builder.Services.AddSingleton<CommandDispatcher>();
builder.Services.AddSingleton<ICommand, JoinCommand>();
builder.Services.AddSingleton<ICommand, LeaveCommand>();

services.AddControllers();

var app = builder.Build();
app.UseWebSockets();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();