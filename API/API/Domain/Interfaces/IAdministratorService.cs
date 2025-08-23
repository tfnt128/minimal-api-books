using API.Domain.DTOs;
using API.Domain.Entities;

namespace API.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
        Administrator? AddAdministrator(Administrator administrator);
        List<Administrator> GetAllAdministrators(int? page);
        Administrator? GetAdministratorById(int id);
    }
}
