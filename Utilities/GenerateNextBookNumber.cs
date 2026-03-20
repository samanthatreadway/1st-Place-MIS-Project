
using System;
using System.Linq;
using fa25group23final.DAL;
using fa25group23final.Models;


namespace fa25group23final.Utilities
{
	public static class GenerateNextBookNumber
	{
		public static Int32 GetNextBookNumber(AppDbContext _context)
		{
			//set a constant to designate where the registration numbers 
			//should start
			const Int32 START_NUMBER = 222001;

			Int32 intMaxBookNumber; //the current maximum book number
			Int32 intNextBookNumber; //the course number for the next book

			if (_context.Books.Count() == 0) //there are no Books in the database yet
			{
				intMaxBookNumber = START_NUMBER; //registration numbers start at 10001
			}
			else
			{
				intMaxBookNumber = _context.Books.Max(c => c.BookNumber); //this is the highest number in the database right now
			}

			//You added records to the datbase before you realized 
			//that you needed this and now you have numbers less than 100 
			//in the database
			if (intMaxBookNumber < START_NUMBER)
			{
				intMaxBookNumber = START_NUMBER;
			}

			//add one to the current max to find the next one
			intNextBookNumber = intMaxBookNumber + 1;

			//return the value
			return intNextBookNumber;
		}

	}
}