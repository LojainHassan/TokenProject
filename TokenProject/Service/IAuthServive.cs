using TokenProject.Model;

namespace TokenProject.Service;

public interface IAuthServive
{
    Task<AuthModel> Register(RegisterModel dto);
}
