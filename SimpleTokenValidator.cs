namespace UserManagementAPI;

public interface ITockenValidator
{
    bool ValidateTocken(string tocken);
}
public class SimpleTokenValidator
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
