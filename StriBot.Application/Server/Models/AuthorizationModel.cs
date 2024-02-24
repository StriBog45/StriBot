namespace StriBot.Application.Server.Models
{
    public class AuthorizationModel
    {
        public string Code { get; }

        public AuthorizationModel(string code)
        {
            Code = code;
        }
    }
}