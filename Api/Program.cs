using Application.Commands;
using Infrastructure;

var builder = WebApplication.CreateBuilder(args);

var services = builder.Services;
var configuration = builder.Configuration;

services.AddSwaggerGen();

services.AddSingleton<Storage>();
services.AddSingleton<CodeGenerator>();
services.AddSingleton<ConnectionManager>();

services.AddSingleton<CommandDispatcher>();
services.AddSingleton<ICommand, JoinCommand>();
services.AddSingleton<ICommand, LeaveCommand>();
services.AddSingleton<ICommand, GetPlayersCommand>();
services.AddSingleton<ICommand, ChangePhaseCommand>();
services.AddSingleton<ICommand, TurnEndCommand>();
services.AddSingleton<ICommand, MakeMoveCommand>();
services.AddSingleton<ICommand, SwitchReadinessCommand>();

services.AddCors(options =>
{
    var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>();

    options.AddPolicy("AllowFrontend",
        corsPolicyBuilder => corsPolicyBuilder
            .WithOrigins(allowedOrigins!)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

services.AddControllers();

var app = builder.Build();
app.UseWebSockets();
app.MapControllers();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.Run();