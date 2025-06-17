using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Users.DesignPattern.Interface;
using foodie_connect_backend.Modules.Users.Dtos;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes;
using foodie_connect_backend.Shared.Classes.Errors;
using Microsoft.AspNetCore.Identity;

namespace foodie_connect_backend.Modules.Users.DesignPattern;

public class CreateUserCommand: ICreateUserCommand
{
    private CreateUserDto _createUserDto;
    private UserManager<User> _userManager;
    private VerificationService _verificationService; 
    public CreateUserCommand(CreateUserDto body, UserManager<User> userManager, VerificationService verificationeSrvice)
    {
        this._createUserDto = body;
        this._userManager = userManager;
        this._verificationService = verificationeSrvice;
    }
    
    public async Task<Result<User>> execute()
    {
        var newUser = new User()
        {
            DisplayName = _createUserDto.DisplayName,
            PhoneNumber = _createUserDto.PhoneNumber,
            UserName = _createUserDto.UserName,
            Email = _createUserDto.Email,
        };
        
        var result = await this._userManager.CreateAsync(newUser, _createUserDto.Password);
        if (!result.Succeeded)
        {
            return result.Errors.FirstOrDefault()?.Code switch
            {
                nameof(IdentityErrorDescriber.DuplicateUserName) => Result<User>.Failure(UserError.DuplicateUsername(this._createUserDto.UserName)),
                nameof(IdentityErrorDescriber.DuplicateEmail) => Result<User>.Failure(UserError.DuplicateEmail(this._createUserDto.Email)),
                nameof(IdentityErrorDescriber.PasswordTooShort) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresDigit) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresLower) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresUpper) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                nameof(IdentityErrorDescriber.PasswordRequiresNonAlphanumeric) => Result<User>.Failure(UserError.PasswordNotValid(result.Errors.First().Description)),
                _ => Result<User>.Failure(AppError.UnexpectedError(result.Errors.First().Description))
            };
        }
        
        await this._userManager.AddToRoleAsync(newUser, "User");
        try
        {
            await this._verificationService.SendConfirmationEmail(newUser.Id);
        }
        catch
        {
            Console.WriteLine("Email verification failed");
        }
        
        return Result<User>.Success(newUser);
    }   
}