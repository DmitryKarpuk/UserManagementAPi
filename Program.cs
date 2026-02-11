using System.Collections.Concurrent;

var builder = WebApplication.CreateBuilder(args);

// Add swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
var app = builder.Build();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

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