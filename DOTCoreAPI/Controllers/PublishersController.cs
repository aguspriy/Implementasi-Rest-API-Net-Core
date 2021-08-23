using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DOTCoreAPI.Models;
using Microsoft.AspNetCore.Authorization;

namespace DOTCoreAPI.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class PublishersController : ControllerBase
    {
        private readonly BookStoresDBContext _context;

        public PublishersController(BookStoresDBContext context)
        {
            _context = context;
        }

        // GET: api/Publishers
        [HttpGet("GetPublishers")]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishers()
        {
            return await _context.Publishers.ToListAsync();
        }

        // GET: api/Publishers/5
        [HttpGet("GetPublisher/{id}")]
        [MapToApiVersion("1.0")]
        public async Task<ActionResult<Publisher>> GetPublisher(int id)
        {
            var publisher = await _context.Publishers
                                            .Where(pub => pub.PubId == id)
                                            .FirstOrDefaultAsync();

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // GET: api/Publishers/5
        [HttpGet("GetPublisherDetails/{PublisherId}")]
        public async Task<ActionResult<Publisher>> GetPublisherDetails(string PublisherId)
        {
            //Eager Loading
            //var publisher = await _context.Publishers
            //                                .Include(pub => pub.Users)
            //                                .Include(pub => pub.Books)
            //                                    .ThenInclude(book => book.Sales)
            //                                .Where(pub => pub.PubId == id)
            //                                .FirstOrDefaultAsync();

            //Explicit Loading
            var publisher = await _context.Publishers.SingleAsync(pub => pub.PubId == Convert.ToInt32(PublisherId));

            _context.Entry(publisher)
                    .Collection(pub => pub.Books)
                    .Query()
                    .Include(book => book.BookAuthors)
                    .Load();

            if (publisher == null)
            {
                return NotFound();
            }

            return publisher;
        }

        // GET: api/Publishers/5
        [HttpPost("PostPublisherDetails/")]
        public async Task<ActionResult<Publisher>> PostPublisherDetails()
        {
            var publisher = new Publisher();
            publisher.PublisherName = "Harper & Brothers";
            publisher.City = "New York City";
            publisher.State = "NY";
            publisher.Country = "USA";

            Book book1 = new Book();
            book1.Title = "Good night moon - 1";
            book1.PublishedDate = DateTime.Now;

            Book book2 = new Book();
            book2.Title = "Good night moon - 2";
            book2.PublishedDate = DateTime.Now;            

            publisher.Books.Add(book1);
            publisher.Books.Add(book2);

            _context.Publishers.Add(publisher);
            _context.SaveChanges();

            var publishers = await _context.Publishers
                                            .Include(pub => pub.Books)
                                            .ThenInclude(book => book.BookAuthors)
                                            .Include(pub => pub.Country)
                                            .Where(pub => pub.PubId == publisher.PubId)
                                            .FirstOrDefaultAsync();

            if (publishers == null)
            {
                return NotFound();
            }

            return publishers;
        }


        // PUT: api/Publishers/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPut("UpdatePublisher/{id}")]
        public async Task<IActionResult> PutPublisher(int id, Publisher publisher)
        {

            var result = "Update data publisher " + publisher.PublisherName + " Sukses";
            if (id != publisher.PubId)
            {
                return BadRequest();
            }

            _context.Entry(publisher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PublisherExists(id))
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

        // POST: api/Publishers
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        [HttpPost("CreatePublisher")]
        public async Task<ActionResult<Publisher>> PostPublisher(Publisher publisher)
        {

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return await Task.FromResult(publisher); //CreatedAtAction("GetPublisher", new { id = publisher.PubId }, publisher);
        }

        // DELETE: api/Publishers/5
        [HttpDelete("DeletePublisher/{id}")]
        public async Task<ActionResult<Publisher>> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null)
            {
                return NotFound();
            }

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();

            return publisher;
        }

        private bool PublisherExists(int id)
        {
            return _context.Publishers.Any(e => e.PubId == id);
        }
    }
}
