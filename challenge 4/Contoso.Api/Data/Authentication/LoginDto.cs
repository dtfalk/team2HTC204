namespace Contoso.Api.Data
{
    public class LoginDto
    {
        public required string Token { get; set; }

        public required string UserName  { get; set; }
    }
}