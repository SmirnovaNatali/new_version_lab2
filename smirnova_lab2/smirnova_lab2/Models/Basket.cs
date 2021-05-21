using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;

namespace smirnova_lab2.Models
{
    public class Basket
    {
        public long Id { set; get; }
        public long PersonId { get; set; }
        private decimal sum;
        public List<Product> Listofproduct { get; set; } = new List<Product>();
        public decimal Sumofproduct
        {
            get
            {
                foreach (Product s in Listofproduct)
                {
                    sum += s.Amount * s.Products.Price;
                }
                return sum;
            }

        }
        public void Deleteposition(long id)
        {
            var deleteposition = Listofproduct.FirstOrDefault(r => r.Id == id);
            if (deleteposition != null)
            {
                Listofproduct.Remove(deleteposition);
            }

        }
        public void Changeposition(long Itemid, int new_a)
        {
            var changeposition = Listofproduct.FirstOrDefault(r => r.Id == Itemid);
            if (changeposition != null)
            {
                changeposition.Amount = new_a;
            }
        }
        public void Addposition(Item item, int amount)
        {

            Product position = new Product();
            position.Products = item;
            position.Amount = amount;
            Listofproduct.Add(position);
        }
       

    }
}
