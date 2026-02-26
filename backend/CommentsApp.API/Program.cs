using CommentsApp.API.Hubs;
using CommentsApp.API.Middleware;
using CommentsApp.Application.Behaviors;
using CommentsApp.Application.CQRS.Comments.Queries;
using CommentsApp.Application.Interfaces;
using CommentsApp.Application.Services;
using CommentsApp.Domain.Interfaces;
using CommentsApp.Infrastructure.Cache;
using CommentsApp.Infrastructure.Data;
using CommentsApp.Infrastructure.Queue;
using CommentsApp.Infrastructure.Repositories;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CommentsDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"),
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// Redis
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
var redisOptions = ConfigurationOptions.Parse(redisConnection);
redisOptions.AbortOnConnectFail = false;
redisOptions.ConnectRetry = 3;
redisOptions.ConnectTimeout = 5000;

builder.Services.AddSingleton<IConnectionMultiplexer>(
    _ => ConnectionMultiplexer.Connect(redisOptions));

// DI
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddSingleton<CaptchaService>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();
//builder.Services.AddSingleton<ICommentsCache, RedisCommentsCache>();
//builder.Services.AddSingleton<ICommentQueue, RedisCommentQueue>();
//builder.Services.AddHostedService<CommentQueueWorker>();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<GetCommentsQueryHandler>();

// MediatR + Validation
builder.Services.AddMediatR(cfg =>
    {
        cfg.RegisterServicesFromAssemblies(
            typeof(Program).Assembly,
            typeof(GetCommentsQuery).Assembly);

        cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
    }
);

// SignalR (WebSocket)
builder.Services.AddSignalR();

// CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost:5173")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials());
});

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseMiddleware<ValidationExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CommentsDbContext>();
    await db.Database.MigrateAsync();
}

if (!app.Environment.IsEnvironment("Docker") || !app.Environment.IsDevelopment())
    app.UseHttpsRedirection();

app.UseCors();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapHub<CommentsHub>("/hubs/comments");

app.Run();
