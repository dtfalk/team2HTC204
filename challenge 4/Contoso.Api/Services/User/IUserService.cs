using Contoso.Api.Models;

namespace Contoso.Api.Services;
public interface IUserService
{
    Task<UserInfoDto> GetUserInfo(int userId);

    Task UpdateUserInfo(UserInfoDto userInfo, int userId);
}