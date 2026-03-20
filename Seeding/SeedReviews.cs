using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fa25group23final.Seeding
{
    public static class SeedReviews
    {
        public static void SeedAllReviews(AppDbContext context)
        {
            if (context.Reviews.Any()) return;

            // RESEED to 0 so that first book ID will be 1 (if using DELETE FROM)
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Reviews', RESEED, 0)");

            List<Reviews> reviews = new List<Reviews>()
            {
                new Reviews()
                {
                    //ReviewID = 1,
                    Rating = 5,
                    ReviewText = "Incredible pacing and tension throughout—couldn't stop reading!",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "cbaker@example.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "s.barnes@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Say Goodbye"),
                },
                new Reviews()
                {
                    //ReviewID = 2,
                    Rating = 4,
                    ReviewText = "Tight mystery with solid twists; a bit slow in the middle.",
                    DisputeStatus = false, // Reject = false
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "cbaker@example.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "j.mason@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Chasing Darkness"),
                },
                new Reviews()
                {
                    //ReviewID = 3,
                    Rating = 4,
                    ReviewText = "Classic Spenser. Sharp dialogue and old-school charm.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "wchang@example.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "c.silva@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "The Professional"),
                },
                new Reviews()
                {
                    //ReviewID = 4,
                    Rating = 3,
                    ReviewText = "Rich historical detail, but pacing drags at times.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "limchou@gogle.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "e.stuart@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "The Other Queen"),
                },
                new Reviews()
                {
                    //ReviewID = 5,
                    Rating = 5,
                    ReviewText = "Fast-moving and witty. Loved the Cape Cod setting.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "limchou@gogle.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "a.rogers@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Wrecked"),
                },
                new Reviews()
                {
                    //ReviewID = 6,
                    Rating = 4,
                    ReviewText = "Emotional and thrilling. Hauck's motives feel real.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "limchou@gogle.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "h.garcia@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Reckless"),
                },
                new Reviews()
                {
                    //ReviewID = 7,
                    Rating = 5,
                    ReviewText = "Lean, witty Spenser case—couldn't put it down.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "jeffh@sonic.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "c.silva@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "The Professional"),
                },
                new Reviews()
                {
                    //ReviewID = 8,
                    Rating = 4,
                    ReviewText = "Creepy, clever, and tightly plotted.",
                    DisputeStatus = false, // Reject = false
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "cmiller@bob.com")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "a.rogers@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Say Goodbye"),
                },
                new Reviews()
                {
                    //ReviewID = 9,
                    Rating = 4,
                    ReviewText = "Light, fun mystery with brisk pacing.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "elowe@netscare.net")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "e.stuart@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Wrecked"),
                },
                new Reviews()
                {
                    //ReviewID = 10,
                    Rating = 3,
                    ReviewText = "Gritty and tense, but a bit uneven.",
                    DisputeStatus = true, // Approve = true
                    reviewerID = context.Users.FirstOrDefault(u => u.Email == "elowe@netscare.net")?.Id,
                    approverID = context.Users.FirstOrDefault(u => u.Email == "e.stuart@bevosbooks.com")?.Id,
                    bookID = context.Books.FirstOrDefault(b => b.Title == "Reckless"),
                },
            };

            foreach (Reviews r in reviews)
            {
                if (!context.Reviews.Any(x => x.ReviewID == r.ReviewID))
                {
                    context.Reviews.Add(r);
                }
            }
            context.SaveChanges();
        }
    }
}