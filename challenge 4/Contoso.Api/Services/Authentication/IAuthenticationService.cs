using Contoso.Api.Data;
using Contoso.Api.Models;

namespace Contoso.Api.Services
{
    public interface IAuthenticationService
    {
        Task<LoginDto> LoginAsync(UserCredentialsDto userLoginDto);

        Task<LoginDto> RegisterAsync(UserDto userDto);
    }
}