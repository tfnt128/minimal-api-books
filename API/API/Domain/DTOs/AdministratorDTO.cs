using API.Domain.Enums;

namespace API.Domain.DTOs
{
    public class AdministratorDTO
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public int Profile { get; set; }
    }
}
