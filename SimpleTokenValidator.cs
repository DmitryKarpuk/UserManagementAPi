namespace UserManagementAPI;

public interface ITokenValidator
{
    bool ValidateToken(string tocken);
}
public class SimpleTokenValidator : ITokenValidator
{ 
    private readonly HashSet<string> _validTokens = new()
    {
        "token123",
        "secret456",
        "admin789"
    };

    public bool ValidateToken(string token)
    {
        return _validTokens.Contains(token);
    }
}
