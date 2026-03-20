using System;
using System.Linq;
using System.Collections.Generic;
using fa25group23final.Models;
using fa25group23final.DAL;
using Microsoft.EntityFrameworkCore;

namespace fa25group23final.Seeding
{
    public static class SeedOrders
    {
        public static void SeedAllOrders(AppDbContext context)
        {
            if (context.Orders.Any()) return;

            // RESEED to 10001 so that first order ID will be 10001 (since starting from fresh db)
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Orders', RESEED, 10000)");

            List<Orders> orders = new List<Orders>()
            {
                new Orders()
                {
                    //OrderID = 10001,
                    OrderDate = null,
                    ShippingFee = null,
                    OrderStatus = false,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "cbaker@example.com"),
                    creditCard = null,
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10002,
                    OrderDate = null,
                    ShippingFee = null,
                    OrderStatus = false,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "toddj@yourmom.com"),
                    creditCard = null,
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10003,
                    OrderDate = null,
                    ShippingFee = null,
                    OrderStatus = false,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "cmiller@bob.com"),
                    creditCard = null,
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10004,
                    OrderDate = new DateTime(2025, 10, 31),
                    ShippingFee = 3.50m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "wchang@example.com"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1004),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10005,
                    OrderDate = new DateTime(2025, 10, 1),
                    ShippingFee = 9.50m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "cbaker@example.com"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1002),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10006,
                    OrderDate = new DateTime(2025, 10, 5),
                    ShippingFee = 107.00m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "limchou@gogle.com"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1005),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10007,
                    OrderDate = new DateTime(2025, 10, 30),
                    ShippingFee = 3.50m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "wchang@example.com"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1004),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10008,
                    OrderDate = new DateTime(2025, 11, 1),
                    ShippingFee = 5.00m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "jeffh@sonic.com"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1006),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10009,
                    OrderDate = new DateTime(2025, 11, 3),
                    ShippingFee = 3.50m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "cmiller@bob.com"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1007),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10010,
                    OrderDate = new DateTime(2025, 11, 2),
                    ShippingFee = 6.50m,
                    OrderStatus = true,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "elowe@netscare.net"),
                    creditCard = context.CreditCards.FirstOrDefault(c => c.CardID == 1008),
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10011,
                    OrderDate = null,
                    ShippingFee = null,
                    OrderStatus = false,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "knelson@aoll.com"),
                    creditCard = null,
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10012,
                    OrderDate = null,
                    ShippingFee = null,
                    OrderStatus = false,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "cluce@gogle.com"),
                    creditCard = null,
                    couponID = null,
                },
                new Orders()
                {
                    //OrderID = 10013,
                    OrderDate = null,
                    ShippingFee = null,
                    OrderStatus = false,
                    customerID = context.Users.FirstOrDefault(u => u.Email == "erynrice@aoll.com"),
                    creditCard = null,
                    couponID = null,
                },
            };

            foreach (Orders o in orders)
            {
                if (!context.Orders.Any(x => x.OrderID == o.OrderID))
                {
                    context.Orders.Add(o);
                }
            }
            context.SaveChanges();
        }
    }
}
