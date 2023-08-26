using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NitariCupBackend.Server.Data;
using NitariCupBackend.Server.EndPoints;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<NitariCupBackendServerContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DB_CONNECTION_STRING") ?? throw new InvalidOperationException("Connection string 'NitariCupBackendServerContext' not found.")));

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapTaskSchemeEndpoints();

app.Run();
