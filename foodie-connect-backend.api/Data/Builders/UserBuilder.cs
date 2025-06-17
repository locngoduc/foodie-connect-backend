using System;
using foodie_connect_backend.Data;

namespace foodie_connect_backend.Data.Builders;

public class UserBuilder
{
    private readonly User _user = new();

    public UserBuilder WithId(string id) { _user.Id = id; return this; }
    public UserBuilder WithDisplayName(string name) { _user.DisplayName = name; return this; }
    public UserBuilder WithAvatarId(string? avatarId) { _user.AvatarId = avatarId; return this; }
    public UserBuilder WithCreatedAt(DateTime createdAt) { _user.CreatedAt = createdAt; return this; }
    public UserBuilder WithUpdatedAt(DateTime updatedAt) { _user.UpdatedAt = updatedAt; return this; }
    public UserBuilder WithUserName(string userName) { _user.UserName = userName; return this; }
    public UserBuilder WithEmail(string email) { _user.Email = email; return this; }
    public User Build() => _user;
}
