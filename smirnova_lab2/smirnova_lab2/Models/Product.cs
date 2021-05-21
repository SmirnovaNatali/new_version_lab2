using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace smirnova_lab2.Models
{
    public class Product
    {
        public long Id { set; get; }
        public Item Products { set; get; }
        public int Amount { set; get; }
    }
    public class IdAmountforProduct
    {
        public long Id { set; get; }
        public int amount { set; get; }
    }
}
