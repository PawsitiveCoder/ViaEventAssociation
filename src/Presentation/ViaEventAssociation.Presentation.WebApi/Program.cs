using ViaEventAssociation.Core.Application;
using ViaEventAssociation.Core.Tools.ObjectMapper;
using ViaEventAssociation.Infrastructure.EfcDmPersistence;
using ViaEventAssociation.Infrastructure.EfcQueries;
using ViaEventAssociation.Infrastructure.Time;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add custom services
builder.Services.AddApplicationCommandHandlers();
builder.Services.AddObjectMapper(typeof(Program).Assembly);
builder.Services.AddInfrastructureDmContext();
builder.Services.AddInfrastructureQueryHandlers();
builder.Services.AddInfrastructureSystemTime();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
