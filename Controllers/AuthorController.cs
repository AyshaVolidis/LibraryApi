using LibraryApi.Data;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class AuthorController : ControllerBase
    {
        private readonly AppDatabase db;

        public AuthorController(AppDatabase db) {
            this.db = db;
        }

        [HttpGet("all")]
        [Authorize(Roles = "Admin,Reader")]

        public ActionResult<List<Author>> GetAuthors()
        {
            return db.tblAuthor.AsNoTracking().ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Reader")]

        public ActionResult<Author> GetbyId(int id) 
        {
            try
            {
                var author = db.tblAuthor.AsNoTracking().FirstOrDefault(a => a.Id == id);
                if (author == null)
                {
                    return NotFound("Author not Found");
                }
                return Ok(author);
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("search")]
        [Authorize(Roles = "Admin,Reader")]

        public ActionResult<List<Author>> Search(string name)
        {
            if (name.Trim() == string.Empty)
            {
                return BadRequest("Enter Author Name to search it");
            }
            var lst= db.tblAuthor.AsNoTracking().Where(a=>a.FullName.ToLower().Contains(name.ToLower().Trim())).ToList();
            if (lst == null || lst.Count == 0)
            {
                return NotFound("No Author has this name");
            }
            return lst;

        }
        [Authorize(Roles = "Admin")]
        [HttpPost("add")]
        public ActionResult AddAuthor([FromForm]Author author)
        {
            try
            {
                var author1= db.tblAuthor.AsNoTracking().FirstOrDefault(a=>a.Id==author.Id);
                if (author1 != null)
                {
                    return BadRequest("This Author Id exists in database");
                }
                 db.tblAuthor.Add(author);
                 db.SaveChanges();
                 return Ok($"Author {author.FullName} added successfully");

            }
            catch (Exception ex) { 
                return BadRequest($"Error occurred : {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("update")]
        public ActionResult UpdateAuthor(int id,[FromForm]Author author)
        {
            var Oldauthor = db.tblAuthor.FirstOrDefault(a => a.Id == id);
            if (Oldauthor == null)
            {
                return NotFound("This Id not found in database");

            }
            if (author.Id != id)
            {
                return BadRequest("Please check you enter the same id");
            }
            try
            {
                Oldauthor.FullName = author.FullName;
                db.SaveChanges();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred : {ex.Message}");
            }
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("remove")]
        public ActionResult DeleteAuthor(int id) 
        { 
            var author=db.tblAuthor.FirstOrDefault(a=>a.Id==id);
            if (author == null) {
                return NotFound("Author Not Found");
            }

            try
            {
                db.tblAuthor.Remove(author);
                db.SaveChanges();
                return Ok($"Author {author.FullName} has been removed");
            }
            catch (Exception ex) 
            {
                return BadRequest($"Error occurred : {ex.Message}");
            }

        }


    }
}
