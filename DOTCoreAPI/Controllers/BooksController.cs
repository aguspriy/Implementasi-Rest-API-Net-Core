using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DOTCoreAPI.Models;

namespace DOTCoreAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly BookStoresDBContext _context;

        public BooksController(BookStoresDBContext context)
        {
            _context = context;
        }

        // GET: api/Books
        [HttpGet("GetBooks")]
        public async Task<ActionResult<IEnumerable<Book>>> GetBooks()
        {                      
            return await _context.Books.ToListAsync();
        }

        // GET: Books/5
        [HttpGet("GetBook/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Book>> GetBook(int id)
        {
            var book = await _context.Books
                                            .Where(book => book.BookId == id)
                                            .FirstOrDefaultAsync();

            if (book == null)
            {
                return NotFound();
            }

            return book;
        }
        
        // PUT: api/Bookss/5
        [HttpPut("UpdateBook/{id}")]
        public async Task<IActionResult> PutBook(int id, Book book)
        {

            var result = "Update data book " + book.Title + " Sukses";
            if (id != book.BookId)
            {
                return BadRequest();
            }

            _context.Entry(book).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            //return NoContent();
            return Ok(result);

        }


        // POST: Books/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost("CreateBook")]
        public async Task<ActionResult<Book>> PostBook(Book book)
        {

            _context.Books.Add(book);
            await _context.SaveChangesAsync();

            return await Task.FromResult(book);
        }        
        
        // DELETE: api/Books/5
        [HttpDelete("DeleteBook/{id}")]
        public async Task<ActionResult<Book>> DeleteBook(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }

            _context.Books.Remove(book);
            await _context.SaveChangesAsync();

            return book;
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.BookId == id);
        }
    }
}
