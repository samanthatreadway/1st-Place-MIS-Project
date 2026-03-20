using System;
using System.Collections.Generic;
using System.Linq;
using fa25group23final.DAL;
using fa25group23final.Models;
using Microsoft.EntityFrameworkCore;

namespace fa25group23final.Seeding
{
    public static class SeedCreditCards
    {
        public static void SeedAllCreditCards(AppDbContext context)
        {
            if (context.CreditCards.Any())
            {
                return;
            }

            // RESEED to 1000 so that first card ID will be 1001 (since previous data may have been deleted)
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('CreditCards', RESEED, 1000)");

            List<CreditCard> allCards = new List<CreditCard>()
            {
                new CreditCard
                {
                    //CardID = 1001,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "cbaker@example.com"), // Customer # 9010
                    CardNumber = "3517193267072490",
                    CardType = CreditCardTypeEnum.Visa,
                },
                new CreditCard
                {
                    //CardID = 1002,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "cbaker@example.com"), // Customer # 9010
                    CardNumber = "5653264624505624",
                    CardType = CreditCardTypeEnum.Mastercard,
                },
                new CreditCard
                {
                    //CardID = 1003,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "franco@example.com"), // Customer # 9012
                    CardNumber = "2340139018242888",
                    CardType = CreditCardTypeEnum.Mastercard,
                },
                new CreditCard
                {
                    //CardID = 1004,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "wchang@example.com"), // Customer # 9013
                    CardNumber = "4888561830797648",
                    CardType = CreditCardTypeEnum.Visa,
                },
                new CreditCard
                {
                    //CardID = 1005,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "limchou@gogle.com"), // Customer # 9014
                    CardNumber = "7874839329412510",
                    CardType = CreditCardTypeEnum.Amex,
                },
                new CreditCard
                {
                    //CardID = 1006,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "jeffh@sonic.com"), // Customer # 9021
                    CardNumber = "8882933892564410",
                    CardType = CreditCardTypeEnum.Visa,
                },
                new CreditCard
                {
                    //CardID = 1007,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "cmiller@bob.com"), // Customer # 9034
                    CardNumber = "9577230402048890",
                    CardType = CreditCardTypeEnum.Mastercard,
                },
                new CreditCard
                {
                    //CardID = 1008,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "elowe@netscare.net"), // Customer # 9028
                    CardNumber = "3391194669212420",
                    CardType = CreditCardTypeEnum.Amex,
                },
                new CreditCard
                {
                    //CardID = 1009,
                    CustomerID = context.Users.FirstOrDefault(u => u.Email == "knelson@aoll.com"), // Customer # 9035
                    CardNumber = "4186773703003410",
                    CardType = CreditCardTypeEnum.Visa,
                },
            };


            foreach (CreditCard item in allCards)
            {
                var dbItem = context.CreditCards.FirstOrDefault(i => i.CardID == item.CardID);
                if (dbItem == null)
                {
                    context.CreditCards.Add(item);
                }
            }

            context.SaveChanges();

        }
    }
}