using Tamkeen.bll.Model;

namespace Tamkeen.bll.Auth
{
    public interface ILogin
    {
        LoginResponseMessage DoLogin(LoginRequestMessage loginRequest);
    }
}