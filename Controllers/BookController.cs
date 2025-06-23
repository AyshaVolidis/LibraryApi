using LibraryApi.Data;
using LibraryApi.DTOs;
using LibraryApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace LibraryApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        private readonly AppDatabase db;
        private readonly IConfiguration config;
        private readonly IWebHostEnvironment hostEnvironment;

        public BookController(AppDatabase db,IConfiguration config,IWebHostEnvironment hostEnvironment) 
        {
            this.db = db;
            this.config = config;
            this.hostEnvironment = hostEnvironment;
        }

        [Authorize(Roles = "Admin,Reader")]

        [HttpGet]
        [Route("all")]

        public ActionResult<List<Book>> GetAll()
        {
            return db.tblBook.AsNoTracking().Include(b=>b.Author).ToList();
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Reader")]

        public ActionResult<Author> GetbyId(int id)
        {
            try
            {
                var book = db.tblBook
                    .AsNoTracking()
                    .Include(b => b.Author)
                    .FirstOrDefault(b => b.Id == id);
                if (book == null)
                {
                    return NotFound("Book not Found");
                }
                return Ok(book);
            }catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        [Route("search")]
        [Authorize(Roles = "Admin,Reader")]

        public ActionResult search(string BookName)
        {
            if (BookName.Trim() == string.Empty)
            {
                return BadRequest("Enter Book Name to search it");
            }
            var lst=db.tblBook.AsNoTracking().Where(b=>b.Title.ToLower().Contains(BookName.ToLower().Trim())).ToList();
            if(lst == null || lst.Count == 0)
            {
                return NotFound("No Book has this name");
            }
            return Ok(lst);
        }

        [HttpPost]
        [Route("add")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> AddBook([FromForm]BookDTO book)
        {
            string? subFolderPath = config.GetSection("uploading:folderPath").Value;
            string[]? allowedFileExtensions = config.GetSection("uploading:allowedFileExtensions").Get<string[]>();
            string imageUrl = null;


            try
            {
                var book1 = db.tblBook.AsNoTracking().FirstOrDefault(b => b.Id == book.Id);
                if (book1 != null)
                {
                    return BadRequest("This Book Id exists in database");
                }
                if (!db.tblAuthor.AsNoTracking().Any(a => a.Id == book.AuthorId))
                {
                    return BadRequest("Author Id Not Found");
                }

                if (book.Image != null)
                {
                    if (!(allowedFileExtensions?.Contains(Path.GetExtension(book.Image.FileName.ToLower())) ?? false))
                    {
                        return BadRequest("Blocked Image extension");
                    }
                    string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, subFolderPath);

                    string fileName = Guid.NewGuid() + Path.GetExtension(book.Image.FileName);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var filestrem = new FileStream(filePath, FileMode.Create))
                    {
                        await book.Image.CopyToAsync(filestrem);
                    }

                     imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";
                }

                var b = new Book()
                {
                    Title = book.Title,
                    AuthorId = book.AuthorId,
                    Description = book.Description,
                    BookType = book.BookType,
                    Price = book.Price,
                    ImagePath = imageUrl,
                };

                db.tblBook.Add(b);
                await db.SaveChangesAsync();
                return CreatedAtAction(nameof(GetbyId),new {id=b.Id},b);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred : {ex.Message}");

            }
        }

        [HttpPut("update")]
        [Authorize(Roles = "Admin")]

        public async Task<ActionResult> UpdateBook([FromForm]BookDTO book)
        {

            var book1 = db.tblBook.FirstOrDefault(b => b.Id == book.Id);
            if (book1 == null)
            {
                return BadRequest("This Book Id Not Vaild");

            }
            if (book1.Id != book.Id)
            {
                return BadRequest("Please check you enter the same id");
            }

            if (!db.tblAuthor.AsNoTracking().Any(a => a.Id == book.AuthorId))
            {
                return BadRequest("Author Id Not Found");
            }
            string? subFolderPath = config.GetSection("uploading:folderPath").Value;
            string[]? allowedFileExtensions = config.GetSection("uploading:allowedFileExtensions").Get<string[]>();
            string imageUrl = null;

            try
            {
                if (book.Image != null)
                {
                    if (!(allowedFileExtensions?.Contains(Path.GetExtension(book.Image.FileName.ToLower())) ?? false))
                    {
                        return BadRequest("Blocked Image extension");
                    }
                    string uploadFolder = Path.Combine(hostEnvironment.WebRootPath, subFolderPath);

                    string fileName = Guid.NewGuid() + Path.GetExtension(book.Image.FileName);

                    string filePath = Path.Combine(uploadFolder, fileName);

                    using (var filestrem = new FileStream(filePath, FileMode.Create))
                    {
                        await book.Image.CopyToAsync(filestrem);
                    }

                    imageUrl = $"{Request.Scheme}://{Request.Host}/images/{fileName}";
                }

                book1.Title=book.Title;
                book1.Description=book.Description;
                book1.AuthorId=book.AuthorId;
                book1.BookType=book.BookType;
                book1.Price=book.Price;
                book1.ImagePath=imageUrl;

                db.tblBook.Update(book1);
                await db.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"Error occurred : {ex.Message}");

            }
        }

        [HttpDelete("delete")]
        [Authorize(Roles = "Admin")]

        public ActionResult DeleteBook(int id) 
        {
            var book=db.tblBook.FirstOrDefault(b=>b.Id==id);
            if (book == null)
            {
                return NotFound("No book has this id");
            }
            try
            {
                if (!string.IsNullOrEmpty(book.ImagePath))
                {
                    string fileName = Path.GetFileName(book.ImagePath); 
                    string filePath = Path.Combine(hostEnvironment.WebRootPath, "images", fileName); 

                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath); 
                    }
                }
                db.tblBook.Remove(book);
                db.SaveChanges();
                return Ok($"Book {book.Title} has been removed");
            }
            catch (Exception ex) {
                return BadRequest($"Error occurred : {ex.Message}");

            }
        }

    }
}
