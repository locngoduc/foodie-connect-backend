using foodie_connect_backend.Data;

namespace foodie_connect_backend.Shared.Patterns.Builder;

public class UserDirector(IUserBuilder userBuilder)
{
    private IUserBuilder _userBuilder = userBuilder;

    public User MakeUser(string displayName, string username, string email, string phoneNumber)
    {
        _userBuilder.Reset();
        _userBuilder.WithDisplayName(displayName);
        _userBuilder.WithUserName(username);
        _userBuilder.WithPhoneNumber(phoneNumber);
        _userBuilder.WithEmail(email);
        _userBuilder.WithRole("User");
        
        return _userBuilder.Build();
    }
    
    public User MakeHead(string displayName, string username, string email, string phoneNumber)
    {
        _userBuilder.Reset();
        _userBuilder.WithDisplayName(displayName);
        _userBuilder.WithUserName(username);
        _userBuilder.WithPhoneNumber(phoneNumber);
        _userBuilder.WithEmail(email);
        _userBuilder.WithRole("Head");
        
        return _userBuilder.Build();
    }
}