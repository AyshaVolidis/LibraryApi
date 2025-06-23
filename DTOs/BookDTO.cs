using LibraryApi.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace LibraryApi.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        public string? BookType { get; set; }

        [MaxLength(80)]
        public string? Description { get; set; }

        public int AuthorId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal Price { get; set; }
        
        public IFormFile? Image { get; set; }
    }
}
