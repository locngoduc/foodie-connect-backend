using foodie_connect_backend.Data;

namespace foodie_connect_backend.Shared.Patterns.Builder;

public interface IUserBuilder
{
    public void Reset();
    public IUserBuilder WithDisplayName(string name);
    public IUserBuilder WithAvatarId(string? avatarId);
    public IUserBuilder WithPhoneNumber(string? phoneNumber);
    public IUserBuilder WithUserName(string userName);
    public IUserBuilder WithEmail(string email);
    public IUserBuilder WithRole(string role);
    public User Build();
}