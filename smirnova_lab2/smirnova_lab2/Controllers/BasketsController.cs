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
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        private readonly PersonContext _context;

        public BasketsController(PersonContext context)
        {
            _context = context;
        }

        // GET: api/Baskets
        [HttpGet]
       // [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Basket>>> GetBaskets()
        {
            return await _context.Baskets.Include(l => l.Listofproduct).ThenInclude(i => i.Products).ToListAsync();
        }

        // GET: api/Baskets/5
        [HttpGet("{id}")]
       // [Authorize(Roles = "admin")]
        public async Task<ActionResult<Basket>> GetBasket(long id)
        {
            var basket = _context.Baskets.Include(p => p.Listofproduct).ThenInclude(i => i.Products).FirstOrDefault(i => i.Id == id); 

            if (basket == null)
            {
                return NotFound(new { answer = $"The basket is empty" });
            }

            return basket;
        }

        //// PUT: api/Baskets/5
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutBasket(long id, Basket basket)
        //{
        //    if (id != basket.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(basket).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!BasketExists(id))
        //        {
        //            return NotFound(new { answer = $"The basket not found" });
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

       
        [HttpPost("add/{BasketId}/{itemid}/{amount}")]
       // [Authorize(Roles = "admin,user")]
       //personid??
        public async Task<ActionResult<Basket>> Addposition( long BasketId,long itemid,int amount)
        {
            var basket = _context.Baskets.Include(i => i.Listofproduct).ThenInclude(u => u.Products).FirstOrDefault(i => i.Id == BasketId);
            var b = basket.Listofproduct.FirstOrDefault(i => i.Products.Id == itemid);
            if (b == null)
            {
                var itemToAdd = _context.Items.FirstOrDefault(r => r.Id == itemid);
                basket.Addposition(itemToAdd, amount);
                await _context.SaveChangesAsync();
                return CreatedAtAction("Addposition", new { answer = "Position added"});
            }
            else
            {
                b.Amount += amount;
                await _context.SaveChangesAsync();
                return CreatedAtAction("Addposition", new { answer = "Amount changed" });
            }
        }

        [HttpDelete("delete/{BasketId}/{positionid}")]
      //  [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Basket>> Deleteposition(long BasketId, long positionid)
        {
            var basket = _context.Baskets.Include(i => i.Listofproduct).ThenInclude(u => u.Products).FirstOrDefault(i => i.Id == BasketId);
            if (basket!= null)
            {
                basket.Deleteposition(positionid);
                await _context.SaveChangesAsync();
                return CreatedAtAction("Deleteposition", new { answer = "Position was deleted" });

            }
            else
            {
                return CreatedAtAction("Deleteposition", new { answer = "Position not found" });
            }

        }
        [HttpPut("put/{BasketId}/{positionid}/{amount}")]
        //[Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Basket>> Changeposition(long BasketId, long positionid, int amount)
        {
            var basket = _context.Baskets.Include(i => i.Listofproduct).ThenInclude(u => u.Products).FirstOrDefault(i => i.Id == BasketId);
            if (basket!= null)
            {
                basket.Changeposition(positionid, amount);
                await _context.SaveChangesAsync();
                return CreatedAtAction("Changeposition", new { answer = "Position changed" });
            }
            else
            {
                return CreatedAtAction("Changeposition", new { answer = "Position not found" });
            }
        }
        

        //// DELETE: api/Baskets/5
        //[HttpDelete("{id}")]
        //public async Task<ActionResult<Basket>> DeleteBasket(long id)
        //{
        //    var basket = _context.Baskets.Include(i => i.Listofproduct).ThenInclude(u => u.Products).FirstOrDefault(i => i.Id == id);
        //    if (basket == null)
        //    {
        //        return NotFound();
        //    }

        //    _context.Baskets.Remove(basket);
        //    await _context.SaveChangesAsync();

        //    return basket;
        //}

        private bool BasketExists(long id)
        {
            return _context.Baskets.Any(e => e.Id == id);
        }
    }
}
