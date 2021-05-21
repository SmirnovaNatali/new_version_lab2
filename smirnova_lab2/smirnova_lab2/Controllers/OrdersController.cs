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
    public class OrdersController : ControllerBase
    {
        private readonly PersonContext _context;

        public OrdersController(PersonContext context)
        {
            _context = context;
        }

        [HttpGet]
       // [Authorize(Roles = "admin")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrders()
        {
            var orders = _context.Orders.Include(p => p.Listofproduct).ThenInclude(i => i.Products);
            return await orders.ToArrayAsync();
        }

        // GET: api/Orders/5
        [HttpGet("{id}")]
      //  [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Order>> GetOrder(long id)
        {
            var order = _context.Orders.Include(p => p.Listofproduct).ThenInclude(i => i.Products).FirstOrDefault(i => i.Id == id);
            if (order == null)
            {
                return NotFound(new { answer = $"The order not found" });
            }

            return order;
        }

        [HttpGet("allorders")]
      //  [Authorize(Roles = "admin")]
        public IEnumerable<object> Getorders()
        {
            return _context.Orders.Include(p => p.Listofproduct).ThenInclude(i => i.Products)
                .Select(o => new {
                    Id=o.Id,
                    Product = o.Listofproduct,
                    DateOfOrder = o.OrderDate,
                    PersonId = o.PersonId,
                    Sumofproduct=o.Sumofproduct
                }
            ).OrderBy(o => o.PersonId).ThenBy(o => o.DateOfOrder);
        }

        //// PUT: api/Orders/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to, for
        //// more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        //[HttpPut("{id}")]
        //public async Task<IActionResult> PutOrder(long id, Order order)
        //{
        //    if (id != order.Id)
        //    {
        //        return BadRequest();
        //    }

        //    _context.Entry(order).State = EntityState.Modified;

        //    try
        //    {
        //        await _context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException)
        //    {
        //        if (!OrderExists(id))
        //        {
        //            return NotFound(new { answer = $"The order not found" });
        //        }
        //        else
        //        {
        //            throw;
        //        }
        //    }

        //    return NoContent();
        //}

        [HttpPost("{PersonId}")]
      //  [Authorize(Roles = "user , admin")]
        public async Task<ActionResult<Order>> Postorders(long PersonId,List<IdAmountforProduct> prod)
        {       
            Order order = new Order();
            foreach (var i in prod)
            {
                foreach (Item items in _context.Items)
                {
                    if (i.Id == items.Id)
                    {
                        Product products= new Product();
                        products.Products = items;
                        products.Amount = i.amount;
                        order.Listofproduct.Add(products);
                    }
                }
            }
            if (order.Listofproduct.Count == 0)
            {
                return CreatedAtAction("PostOrder", new {answer = "Product is out of stock"});
            }
            else
            {
                order.OrderDate = DateTime.Now;
                order.PersonId = PersonId;
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return CreatedAtAction("PostOrder", new { id = order.Id }, order);
            }
            //_context.Orders.Add(order);
            //order.OrderDate = DateTime.Now;
            //await _context.SaveChangesAsync();
            //return CreatedAtAction("PostOrder", new { id = order.Id }, order);

        }

        [HttpPost("basket/{PersonId}")]
        //  [Authorize(Roles = "user , admin")]
        public async Task<ActionResult<Order>> Postordersfrombasket(long PersonId)
        {
            Order order = new Order();
            var basket = _context.Baskets.Include(i => i.Listofproduct).ThenInclude(u => u.Products).FirstOrDefault(i => i.Id == PersonId);
            order.Listofproduct = basket.Listofproduct;
            order.OrderDate = DateTime.Now;
            order.PersonId = PersonId;
            _context.Orders.Add(order);
            basket.Listofproduct = null;
             await _context.SaveChangesAsync();
            return CreatedAtAction("PostOrder", new { id = order.Id }, order);
        }
        // DELETE: api/Orders/5
        [HttpDelete("{id}")]
       // [Authorize(Roles = "admin,user")]
        public async Task<ActionResult<Order>> DeleteOrder(long id)
        {
            //var order = await _context.Orders.FindAsync(id);
            var order = _context.Orders.Include(i => i.Listofproduct).ThenInclude(u => u.Products).FirstOrDefault(i => i.Id == id);
            if (order == null)
            {
                return NotFound();
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return order;
        }

        private bool OrderExists(long id)
        {
            return _context.Orders.Any(e => e.Id == id);
        }
    }
}
