using API.Domain.DTOs;
using API.Domain.Entities;
using API.Domain.Interfaces;
using API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace API.Domain.Services
{
    public class AdministratorService : IAdministratorService
    {

        private readonly BookManagementContext _context;
        public AdministratorService(BookManagementContext context)
        {
            _context = context;
        }
        public Administrator? Login(LoginDTO loginDTO)
        {
            return  _context.administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        }
    }
}
