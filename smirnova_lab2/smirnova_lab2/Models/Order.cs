using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smirnova_lab2.Models
{
    public class Order
    {
        public long Id { set; get; }

        public long PersonId { get; set; }

        public DateTime OrderDate { get; set; }

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
    }
}

