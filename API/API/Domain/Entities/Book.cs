using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace API.Domain.Entities
{
    public class Book
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        public string BookName { get; set; }

        [Required]
        [StringLength(100)]
        public string AuthorName { get; set; }

        [Required]
        public int Year { get; set; }

        public bool Status { get; set; }
    }
}
