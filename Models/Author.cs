using System.ComponentModel.DataAnnotations;

namespace LibraryApi.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required]
        public string FullName { get; set; }
    }
}
