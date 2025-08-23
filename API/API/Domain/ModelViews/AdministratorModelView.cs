using API.Domain.Enums;

namespace API.Domain.ModelViews
{
    public record AdministratorModelView
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Profile { get; set; }
    }
}
