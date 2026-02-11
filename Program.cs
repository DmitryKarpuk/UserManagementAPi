using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

// Error handling Middleware
app.UseMiddleware<ErrorHandlingMiddleware>();
// Logging Middleware
app.UseMiddleware<LoggingMiddleware>();

var users = new ConcurrentDictionary<int, User>();
users.TryAdd(1, new User() {Name = "Oleg", Age = 18});
users.TryAdd(2, new User(){ Name = "Andrij", Age = 20});
int lastId = 2;

app.MapGet("/", () => "Main page");
app.MapGet("/users", () => Results.Ok(users.Values));
app.MapGet("/users/{id:int}", (int id) =>
{
    return users.TryGetValue(id, out var user)
        ? Results.Ok(user)
        : Results.NotFound("User not found");
});
app.MapPost("/users", (User user) =>
{
    if (user is null) return Results.BadRequest("User is required.");
    var error = UserValidator.Validate(user);
    if (error != null) return Results.BadRequest(error);
    int id = Interlocked.Increment(ref lastId);
    users[id] = user;
    return Results.Created();
});
app.MapPut("/users/{id:int}", (int id, User user) =>
{
    if (user is null) return Results.BadRequest("User is required.");
    var error = UserValidator.Validate(user);
    if (error != null) return Results.BadRequest(error);
    if (users.ContainsKey(id))
    {
        users[id] = user;
        return Results.Ok(user);
    }
    else return Results.NotFound("User not found");
});
app.MapDelete("/users/{id:int}", (int id) =>
{
    
    return users.TryRemove(id, out var user)
        ? Results.NoContent()
        : Results.NotFound("User not found");
});
app.Run();


public class User
{
    public required string Name {get; set;}
    public int Age {get; set;}

    public User( string name, int age)
    {
        Name = name;
        Age = age;
    }

    public User(){}
}

public static class UserValidator
{
    public static string? Validate(User user)
    {
        if (user == null) return "User is required.";
        if (string.IsNullOrWhiteSpace(user.Name)) return "User name is required.";
        if (user.Name.Length < 2) return "User name must be at least 2 characters.";
        if (user.Age < 0 || user.Age >= 120) return "User age must be between 0 and 120";
        return null;
    }
}

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _logger = logger;
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        string path = context.Request.Path;
        string method = context.Request.Method;
        await _next.Invoke(context);
        int statusCode = context.Response.StatusCode;
        _logger.LogInformation($"HTTP request {method} {path}, response status: {statusCode}");
    }
}

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // log details
            _logger.LogError(ex, "Unhandled exception.");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        // json message
        var jsonMessage = new { error = "Internal server error."};

        return context.Response.WriteAsJsonAsync(jsonMessage);
    }
}