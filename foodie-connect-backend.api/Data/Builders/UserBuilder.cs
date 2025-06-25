using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class UserBuilder: IUserBuilder
{
    private string _displayName = null!;
    private string? _avatarId = string.Empty;
    private string? _phoneNumber;
    private string _userName = null!;
    private string _email = null!;
    private string _role = "User";

    public void Reset()
    {
        _displayName = null!;
        _avatarId = null!;
        _phoneNumber = null!;
        _userName = null!;
        _email = null!;
        _role = null!;
    }
    
    public IUserBuilder WithDisplayName(string name) { _displayName = name; return this; }
    public IUserBuilder WithAvatarId(string? avatarId) { _avatarId = avatarId; return this; }
    public IUserBuilder WithPhoneNumber(string? phoneNumber) { _phoneNumber = phoneNumber; return this; }
    public IUserBuilder WithUserName(string userName) { _userName = userName; return this; }
    public IUserBuilder WithEmail(string email) { _email = email; return this; }
    public IUserBuilder WithRole(string role) { _role = role; return this; }

    public User Build() => new User
    {
        DisplayName = _displayName,
        AvatarId = _avatarId,
        PhoneNumber = _phoneNumber,
        CreatedAt = DateTime.UtcNow,
        UpdatedAt = DateTime.UtcNow,
        UserName = _userName,
        Email = _email,
        Role = _role,
    };
}
