using API.Domain.DTOs;
using API.Domain.Entities;

namespace API.Domain.Interfaces
{
    public interface IAdministratorService
    {
        Administrator? Login(LoginDTO loginDTO);
    }
}
