using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace fa25group23final.Services
{
    public class RecommendationService
    {
        private readonly AppDbContext _context;

        public RecommendationService(AppDbContext context)
        {
            _context = context;
        }

        // MAIN ENTRY POINT: Get up to 3 recommended books
        public List<Books> GetRecommendationsForBook(int bookId, string userId)
        {
            var recommendations = new List<Books>();

            // Get the "target" Books (the one they just bought)
            var targetBook = _context.Books
                .Include(b => b.genre)
                .FirstOrDefault(b => b.BookID == bookId);

            if (targetBook == null)
                return recommendations;

            var targetGenreId = targetBook.genre;
            var targetAuthor = targetBook.Authors;

            // 1) All books the user has already purchased (so we can exclude them)
            List<int> purchasedBookIds = _context.Orders
                .Where(o => o.customerID != null && o.customerID.Id == userId)
                .SelectMany(o => o.OrderDetails)           // nav prop on Orders
                .Select(od => od.bookID.BookID)             // 🔴 use the nav prop + BookID
                .Distinct()
                .ToList();


            // Base query for recommendable books (not purchased, not discontinued, in stock)
            IQueryable<Books> baseBooks = _context.Books
      .Include(b => b.genre)
      .Where(b => !purchasedBookIds.Contains(b.BookID)   // types now match
                  && b.BookStatus == true
                  && b.InventoryQuantity > 0);

            // Helper: compute rating for a Books
            Func<int, double?> getAverageRating = (int bId) =>
            {
                var ratings = _context.Reviews
                    .Where(r => r.bookID != null && r.bookID.BookID == bId)
                    .Select(r => (double)r.Rating);

                if (!ratings.Any()) return null;
                return ratings.Average();
            };

            // Track authors we've already used in recommendations
            var usedAuthorNames = new HashSet<string>();

            // -----------------------------
            // STEP 1: Same-author, same-genre
            // -----------------------------
            var sameAuthorSameGenre = baseBooks
                .Where(b => b.Authors == targetAuthor && b.genre == targetGenreId && b.BookID != targetBook.BookID)
                .ToList();

            if (sameAuthorSameGenre.Any())
            {
                var bestAuthorBook = sameAuthorSameGenre
                    .Select(b => new
                    {
                        Books = b,
                        AvgRating = getAverageRating(b.BookID) ?? 0.0
                    })
                    .OrderByDescending(x => x.AvgRating)
                    .ThenBy(x => x.Books.Title)
                    .First().Books;

                recommendations.Add(bestAuthorBook);
                usedAuthorNames.Add(bestAuthorBook.Authors);
            }

            // -----------------------------
            // STEP 2: Highly rated same-genre (>= 4.0), different authors
            // -----------------------------
            if (recommendations.Count < 3)
            {
                var sameGenreBooks = baseBooks
                    .Where(b => b.genre == targetGenreId && b.BookID != targetBook.BookID)
                    .ToList();

                var highlyRatedSameGenre = sameGenreBooks
                    .Select(b => new
                    {
                        Books = b,
                        AvgRating = getAverageRating(b.BookID)
                    })
                    .Where(x => x.AvgRating.HasValue && x.AvgRating.Value >= 4.0)
                    .OrderByDescending(x => x.AvgRating.Value)
                    .ThenBy(x => x.Books.Title)
                    .ToList();

                foreach (var candidate in highlyRatedSameGenre)
                {
                    if (recommendations.Count >= 3) break;

                    // Different authors
                    if (usedAuthorNames.Contains(candidate.Books.Authors)) continue;

                    recommendations.Add(candidate.Books);
                    usedAuthorNames.Add(candidate.Books.Authors);
                }
            }

            // -----------------------------
            // STEP 3: Fill with low/no-rating books in same genre
            // -----------------------------
            if (recommendations.Count < 3)
            {
                var sameGenreBooks = baseBooks
                    .Where(b => b.genre == targetGenreId && b.BookID != targetBook.BookID)
                    .ToList();

                var fillerSameGenre = sameGenreBooks
                    .Where(b => !recommendations.Any(r => r.BookID == b.BookID))
                    .Select(b => new
                    {
                        Books = b,
                        AvgRating = getAverageRating(b.BookID) ?? 0.0
                    })
                    .OrderByDescending(x => x.AvgRating)
                    .ThenBy(x => x.Books.Title)
                    .ToList();

                foreach (var candidate in fillerSameGenre)
                {
                    if (recommendations.Count >= 3) break;
                    recommendations.Add(candidate.Books);
                    usedAuthorNames.Add(candidate.Books.Authors);
                }
            }

            // -----------------------------
            // STEP 4: If no same-genre options, use highest-rated overall
            // (or if still short, top rated overall)
            // -----------------------------
            if (recommendations.Count < 3)
            {
                var excludedBookIds = recommendations.Select(r => r.BookID).ToList();

                var overallCandidates = baseBooks
                    .Where(b => !excludedBookIds.Contains(b.BookID))  // CHANGE THIS LINE
                    .ToList();

                var highestRatedOverall = overallCandidates
                    .Select(b => new
                    {
                        Books = b,
                        AvgRating = getAverageRating(b.BookID) ?? 0.0
                    })
                    .OrderByDescending(x => x.AvgRating)
                    .ThenBy(x => x.Books.Title)
                    .ToList();

                foreach (var candidate in highestRatedOverall)
                {
                    if (recommendations.Count >= 3) break;
                    recommendations.Add(candidate.Books);
                }
            }

            return recommendations;
        }
    }
}
