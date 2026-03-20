using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fa25group23final.Seeding
{
    public static class SeedOrderDetails
    {
        public static void SeedAllOrderDetails(AppDbContext context)
        {
            if (context.OrderDetails.Any()) return;

            // RESEED to 0 so that first book ID will be 1 (if using DELETE FROM)
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('OrderDetails', RESEED, 0)");

            List<OrderDetails> details = new List<OrderDetails>()
            {
                new OrderDetails()
                {
                    // OrderDetailsID = 1,
                    Price = 23.95m,
                    Cost = 10.30m,
                    Quantity = 2,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10001),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 1)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 2,
                    Price = 25.99m,
                    Cost = 13.25m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10001),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 2)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 3,
                    Price = 24.99m,
                    Cost = 20.99m,
                    Quantity = 2,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10002),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 52)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 4,
                    Price = 27.99m,
                    Cost = 25.75m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10003),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 50)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 5,
                    Price = 26.95m,
                    Cost = 7.01m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10004),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 43)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 6,
                    Price = 25.00m,
                    Cost = 11.25m,
                    Quantity = 5,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10005),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 4)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 7,
                    Price = 25.95m,
                    Cost = 9.08m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10005),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 3)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 8,
                    Price = 25.95m,
                    Cost = 23.61m,
                    Quantity = 70,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10006),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 11)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 9,
                    Price = 25.00m,
                    Cost = 18.00m,
                    Quantity = 10,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10006),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 60)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 10,
                    Price = 22.00m,
                    Cost = 9.46m,
                    Quantity = 23,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10006),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 61)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 11,
                    Price = 26.95m,
                    Cost = 7.01m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10007),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 43)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 12,
                    Price = 26.95m,
                    Cost = 7.01m,
                    Quantity = 2,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10008),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 43)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 13,
                    Price = 25.00m,
                    Cost = 11.25m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10009),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 4)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 14,
                    Price = 25.00m,
                    Cost = 18.00m,
                    Quantity = 3,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10010),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 60)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 15,
                    Price = 22.00m,
                    Cost = 9.46m,
                    Quantity = 11,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10010),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 61)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 16,
                    Price = 21.95m,
                    Cost = 12.95m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10011),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 298)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 17,
                    Price = 18.95m,
                    Cost = 3.60m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10012),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 28)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 18,
                    Price = 25.00m,
                    Cost = 2.75m,
                    Quantity = 1,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10013),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 42)
                },
                new OrderDetails()
                {
                    // OrderDetailsID = 19,
                    Price = 25.95m,
                    Cost = 3.11m,
                    Quantity = 3,
                    orderID = context.Orders.FirstOrDefault(o => o.OrderID == 10013),
                    bookID = context.Books.FirstOrDefault(b => b.BookID == 54)
                },
            };

            foreach (OrderDetails d in details)
            {
                if (!context.OrderDetails.Any(x => x.OrderDetailsID == d.OrderDetailsID))
                {
                    context.OrderDetails.Add(d);
                }
            }
            context.SaveChanges();
        }
    }
}
