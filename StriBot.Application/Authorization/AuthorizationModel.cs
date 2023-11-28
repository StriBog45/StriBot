namespace StriBot.Application.Authorization
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