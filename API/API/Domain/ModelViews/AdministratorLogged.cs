namespace API.Domain.ModelViews
{
    public record AdministratorLogged
    {
        public string Email { get; set; }
        public string Profile { get; set; }
        public string Token { get; set; }
    }
}
