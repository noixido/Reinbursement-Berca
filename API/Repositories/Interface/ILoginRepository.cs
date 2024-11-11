using API.ViewModels;

namespace API.Repositories.Interface
{
    public interface ILoginRepository
    {
        bool Login(LoginVM loginVM); // fitur login

        PayloadVM GetPayload(string email); // membuat payload yang akan dilempar ke token jwt
    }
}
