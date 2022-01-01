using API.Middleware;
using Application.Records;
using FluentValidation.AspNetCore;
using MediatR;
using MongoDB.Driver;
using Persistence;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddFluentValidation(config =>
{
    config.RegisterValidatorsFromAssemblyContaining<Create>();
});

builder.Services.AddSingleton<IMongoClient>(s => new MongoClient(builder.Configuration["MongoDb:ConnectionString"]));
builder.Services.AddScoped(s => new MongoDbContext(s.GetRequiredService<IMongoClient>(), builder.Configuration["MongoDb:DatabaseName"]));
builder.Services.AddMediatR(typeof(List.Handler).Assembly);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
