using CommentsApp.API.Converters;
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
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Ensure wwwroot exists
var webRootPath = builder.Environment.WebRootPath
                  ?? Path.Combine(builder.Environment.ContentRootPath, "wwwroot");
Directory.CreateDirectory(webRootPath);
builder.Environment.WebRootPath = webRootPath;

// ── Database ──
builder.Services.AddDbContext<CommentsDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"),
        sql => sql.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery)));

// ── Redis ──
var redisConnection = builder.Configuration.GetConnectionString("Redis") ?? "localhost:6379,abortConnect=false";
var redisOptions = ConfigurationOptions.Parse(redisConnection);
redisOptions.AbortOnConnectFail = false;
redisOptions.ConnectRetry = 3;
redisOptions.ConnectTimeout = 5000;
builder.Services.AddSingleton<IConnectionMultiplexer>(_ => ConnectionMultiplexer.Connect(redisOptions));

// ── Services ──
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddSingleton<CaptchaService>();
builder.Services.AddSingleton<ICacheService, RedisCacheService>();
builder.Services.AddSingleton<IBackgroundTaskQueue, BackgroundTaskQueue>();
builder.Services.AddHostedService<QueuedHostedService>();

// ── Validation & MediatR ──
builder.Services.AddValidatorsFromAssemblyContaining<GetCommentsQueryHandler>();
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssemblies(
        typeof(Program).Assembly,
        typeof(GetCommentsQuery).Assembly);
    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
});

// ── SignalR + CORS + Controllers ──
builder.Services.AddSignalR();
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:4200", "http://localhost")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()));

builder.Services.AddControllers()
    .AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new UtcDateTimeConverter()));

if (builder.Environment.IsDevelopment())
    builder.Services.AddOpenApi();

var app = builder.Build();

// ── Middleware pipeline (order matters!) ──
app.UseMiddleware<ValidationExceptionMiddleware>();

if (app.Environment.IsDevelopment()) app.MapOpenApi();
// app.MapScalarApiReference(); // uncomment if needed
// Auto-migrate database
using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<CommentsDbContext>().Database.MigrateAsync();
}

app.UseCors();
app.UseStaticFiles();
app.UseRouting();
app.MapControllers();
app.MapHub<CommentsHub>("/hubs/comments");

app.Run();