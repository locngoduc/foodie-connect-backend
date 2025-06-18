using System;
using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Patterns.Builder;

namespace foodie_connect_backend.Data.Builders;

public class UserBuilder: IBuilder<User>
{
    private string _displayName = null!;
    private string? _avatarId = string.Empty;
    private string? _phoneNumber;
    private DateTime _createdAt = DateTime.UtcNow;
    private DateTime _updatedAt = DateTime.UtcNow;
    private string _userName = null!;
    private string _email = null!;
    public UserBuilder WithDisplayName(string name) { _displayName = name; return this; }
    public UserBuilder WithAvatarId(string? avatarId) { _avatarId = avatarId; return this; }
    public UserBuilder WithPhoneNumber(string? phoneNumber) { _phoneNumber = phoneNumber; return this; }
    public UserBuilder WithCreatedAt(DateTime createdAt) { _createdAt = createdAt; return this; }
    public UserBuilder WithUpdatedAt(DateTime updatedAt) { _updatedAt = updatedAt; return this; }
    public UserBuilder WithUserName(string userName) { _userName = userName; return this; }
    public UserBuilder WithEmail(string email) { _email = email; return this; }
    public User Build() => new User
    {
        DisplayName = _displayName,
        AvatarId = _avatarId,
        PhoneNumber = _phoneNumber,
        CreatedAt = _createdAt,
        UpdatedAt = _updatedAt,
        UserName = _userName,
        Email = _email
    };
}
