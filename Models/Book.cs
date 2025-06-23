using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LibraryApi.Models
{
    public class Book
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Title { get; set; }

        public string? BookType {  get; set; }

        [MaxLength(80)]
        public string? Description {  get; set; }

        public int AuthorId {  get; set; }

        public string? ImagePath { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Price cannot be negative")]
        public decimal  Price { get; set; }

        [ForeignKey("AuthorId")]
        public Author? Author { get; set; }


    }
}
