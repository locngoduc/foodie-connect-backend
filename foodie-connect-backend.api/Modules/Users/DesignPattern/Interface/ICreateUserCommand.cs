using foodie_connect_backend.Data;
using foodie_connect_backend.Shared.Classes;

namespace foodie_connect_backend.Modules.Users.DesignPattern.Interface;

public interface ICreateUserCommand
{
    Task<Result<User>> execute();
}