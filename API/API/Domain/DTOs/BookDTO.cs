using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.DTOs
{
    public record BookDTO
    {
        public string BookName { get; set; }
        public string AuthorName { get; set; }
        public int Year { get; set; }
        public bool Status { get; set; }
    }
}
