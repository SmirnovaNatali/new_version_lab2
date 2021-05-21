using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smirnova_lab2.Models;

namespace smirnova_lab2.Controllers
{
    [Route("api/People")]
    [ApiController]
    public class PeopleController : ControllerBase
    {
        private readonly PersonContext _context;

        public PeopleController(PersonContext context)
        {
            _context = context;
        }

        //// GET: api/People
        //[HttpGet("listoforders")]
        //public async Task<ActionResult<IEnumerable<Person>>> GetOrdersbyPersons()
        //{
          
        //    return await _context.Persons.Include(p => p.Orders).ThenInclude(i => i.Listofproduct).ThenInclude(i => i.Products).ToListAsync();
        //}
        //[HttpGet("listofbaskets")]
        //public async Task<ActionResult<IEnumerable<Person>>> GetBasketsbyPersons()
        //{
        //    return await _context.Persons.Include(p => p.PersonBasket).ThenInclude(i => i.Listofproduct).ThenInclude(i => i.Products).ToListAsync();
        //}

        [HttpGet("order/{id}/{orderid}")]
       // [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Order>> GetOrder(long id, long orderid)
        {
            var pers = _context.Persons.Include(p => p.Orders).ThenInclude(i => i.Listofproduct).ThenInclude(i => i.Products).FirstOrDefault(i => i.Id == id);
            Order or = pers.Orders.FirstOrDefault(i => i.Id == orderid);
            if (pers == null)
            {
                return NotFound();
            }

            return or;
        }

        [HttpGet("basket/{id}")]
        //[Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Basket>> GetBasket(long id)
        {
            var bas = _context.Persons.Include(p => p.PersonBasket).ThenInclude(i => i.Listofproduct).ThenInclude(i => i.Products).FirstOrDefault(i => i.Id == id);
            Basket basket = bas.PersonBasket;
            if (basket == null)
            {
                return NotFound();
            }

            return basket;
        }


        // GET: api/People/5
        [HttpGet("{id}")]
       // [Authorize(Roles = "admin")]
        public async Task<ActionResult<Person>> GetPerson(long id)
        {
            var person =  _context.Persons.FirstOrDefault(i => i.Id == id);

            if (person == null)
            {
                return NotFound(new { answer = $"The person was not registered" });
            }

            return person;
        }
        [HttpPost("countofclients")]
        //[Authorize(Roles = "admin")]
        public string Counts()
        {
            return $"Total: {_context.Persons.Count(person => person.Role == "user" || person.Role == null)} clients";
        }

        [HttpGet("allclients")]
       // [Authorize(Roles = "admin")]
        public IEnumerable<object> Getallclient()
        {
            return _context.Persons
                .Select(i => new
                {
                    Name = i.FullName,
                    Phone = i.Phone,
                    Mail = i.Login,
                    Balance = i.Balance
                }
              );
        }

        [HttpGet("orders")]
       // [Authorize(Roles = "admin")]
        public IEnumerable<object> GetOrders()
        {
            return _context.Persons.Include(i => i.Orders).ThenInclude(p => p.Listofproduct).ThenInclude(i => i.Products)
                .Select(i => new {
                    Id=i.Id,
                    FullName = i.FullName,
                    Balance = i.Balance,
                    OrderId = i.Orders.Select(o => o.Id),
                    OrderContent = i.Orders.Select(o => o.Listofproduct).ToList(),
                    OrderCount = i.Orders.Select(o => o.Listofproduct.Count()),
                    OrderDate = i.Orders.Select(o => o.OrderDate),
                    SumofOrder=i.Orders.Select(o => o.Sumofproduct)
                }
             );
        }

      //запрос как амоунт+амоунт
        [HttpPut("{id}")]
       // [Authorize(Roles = "admin,user")]
        public async Task<IActionResult> PutPerson(long id, [Bind("Login","Balance")] Person person)
        {
            if (id != person.Id)
            {
                return BadRequest();
            }

            _context.Entry(person).State = EntityState.Modified;

            try
            {
                if (person.Role == null)
                {
                    person.Role = "user";
                }
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PersonExists(id))
                {
                    return NotFound(new { answer = $"The person not found" });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }
 
        [HttpPost("add")]
        public async Task<ActionResult<Person>> PostPerson([Bind("Login","Password","Phone","FullName")] Person person)
        {

            var res = _context.Persons.FirstOrDefault(i => i.Login == person.Login);
            if (res == null)
            {
                if (person.Role == null)
                {
                    person.Role = "user";
                }
                _context.Persons.Add(person);
                await _context.SaveChangesAsync();
                Basket basket = new Basket();
                basket.Id = person.Id;
                basket.PersonId = person.Id;
                _context.Baskets.Add(basket);
                await _context.SaveChangesAsync();
                return CreatedAtAction("GetPerson", new { id = person.Id }, person);
            }
            else
            {
                return CreatedAtAction("PostPerson", new { answer = "The person with this login is already registered" });
            }

        }

        [HttpPost("pay")]
        // [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Person>> Postbalance([FromForm] long id, [FromForm] long OrderId)
        {
            var pers = _context.Persons.Include(p => p.Orders).ThenInclude(i => i.Listofproduct).ThenInclude(i => i.Products).FirstOrDefault(i => i.Id == id);
            var or = pers.Orders.FirstOrDefault(i => i.Id == OrderId);
            pers.Balance -= or.Sumofproduct;
            await _context.SaveChangesAsync();
            return CreatedAtAction("Postbalance", new {answer="Balance has been updated" });
        }


        [HttpDelete("{id}")]
       // [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Person>> DeletePerson(long id)
        {
            var person = await _context.Persons.FindAsync(id);
            if (person == null)
            {
                return NotFound(new { answer = $"The person not found" });
            }

            _context.Persons.Remove(person);
            await _context.SaveChangesAsync();

            return person;
        }

        private bool PersonExists(long id)
        {
            return _context.Persons.Any(e => e.Id == id);
        }
    }
}
