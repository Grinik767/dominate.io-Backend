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
builder.Services.AddSingleton<ICommand, GetPlayersCommand>();
builder.Services.AddSingleton<ICommand, SwitchReadinessCommand>();

services.AddControllers();

var app = builder.Build();
app.UseWebSockets();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.Run();