using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema;
using smirnova_lab2.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace smirnova_lab2.Models
{
    public class Person
    {
        public long Id { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string Phone { get; set; }
        public string FullName { get; set; }
        public string Role { get; set; }
        public decimal Balance { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public Basket PersonBasket { get; set; } = new Basket();
       

    }
}
