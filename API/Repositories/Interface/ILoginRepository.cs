using API.ViewModels;

namespace API.Repositories.Interface
{
    public interface ILoginRepository
    {
        bool Login(LoginVM loginVM);

        PayloadVM GetPayload(string email);
    }
}
