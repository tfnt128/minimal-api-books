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

        public Administrator AddAdministrator(Administrator administrator)
        {
            _context.administrators.Add(administrator);
            _context.SaveChanges();

            return administrator;
        }

        public Administrator GetAdministratorById(int id)
        {
            return _context.administrators.Where(a => a.Id == id).FirstOrDefault();
        }

        public List<Administrator> GetAllAdministrators(int? page)
        {
            var query = _context.administrators.AsQueryable();

            int pageItems = 10;

            if (page != null)
                query = query.Skip((int)(page - 1) * pageItems).Take(pageItems);

            return query.ToList();
        }

        public Administrator? Login(LoginDTO loginDTO)
        {
            return  _context.administrators.Where(a => a.Email == loginDTO.Email && a.Password == loginDTO.Password).FirstOrDefault();
        }
    }
}
