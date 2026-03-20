using fa25group23final.DAL;
using fa25group23final.Models;
using Humanizer.Localisation;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fa25group23final.Seeding
{
    public static class SeedGenres
    {
        public static void SeedAllGenres(AppDbContext context)
        {
            if (context.Genres.Any()) return;

            // GenreID starts at 1 and increments by 1 automatically
            // RESEED to 0 so that first book ID will be 1 (if using DELETE FROM)
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('Genres', RESEED, 0)");

            List<Genres> genres = new List<Genres>()
            {
                new Genres
                {
                    GenreName = "Adventure",
                },
                new Genres
                {
                    GenreName = "Contemporary Fiction",
                },
                new Genres
                {
                    GenreName = "Fantasy",
                },
                new Genres
                {
                    GenreName = "Historical Fiction",
                },
                new Genres
                {
                    GenreName = "Horror",
                },
                new Genres
                {
                    GenreName = "Humor",
                },
                new Genres
                {
                    GenreName = "Mystery",
                },
                new Genres
                {
                    GenreName = "Poetry",
                },
                new Genres
                {
                    GenreName = "Romance",
                },
                new Genres
                {
                    GenreName = "Science Fiction",
                },
                new Genres
                {
                    GenreName = "Shakespeare",
                },
                new Genres
                {
                    GenreName = "Suspense",
                },
                new Genres
                {
                    GenreName = "Thriller",
                },
            };

            foreach (Genres g in genres)
            {
                if (!context.Genres.Any(gen => gen.GenreID == g.GenreID))
                {
                    context.Genres.Add(g);
                }
            }

            context.SaveChanges();
        }
    }
}