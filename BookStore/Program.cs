using BookStore.Models;
using Microsoft.EntityFrameworkCore;

namespace BookStore
{
	internal class Program
	{
		static void Main(string[] args)
		{
			using var context = new BookStoreDbContext();

			bool exit = false;
			while (!exit)
			{
				DisplayMenu();

				string choice = Console.ReadLine();

				switch (choice)
				{
					case "1":
						ListStockLevels(context);
						break;
					case "2":
						AddBookToStore(context);
						break;
					case "3":
						RemoveBookFromStore(context);
						break;
					case "4":
						AddNewBook(context);
						break;
					case "5":
						AddNewAuthor(context);
						break;
					case "6":
						EditBook(context);
						break;
					case "7":
						DeleteBook(context);
						break;
					case "8":
						EditAuthor(context);
						break;
					case "9":
						DeleteAuthor(context);
						break;
					case "10":
						exit = true;
						break;
					default:
						Console.WriteLine("Invalid choice, please try again.");
						Pause();
						break;
				}
			}
		}

		static void DisplayMenu()
		{

			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Welcome to the BookStore Management Application");
			Console.ResetColor();
			Console.WriteLine("1. List Stock Levels for Stores");
			Console.WriteLine("2. Add Book to Store Inventory");
			Console.WriteLine("3. Remove Book from Store Inventory");
			Console.WriteLine("4. Add New Book");
			Console.WriteLine("5. Add New Author");
			Console.WriteLine("6. Edit Book");
			Console.WriteLine("7. Delete Book");
			Console.WriteLine("8. Edit Author");
			Console.WriteLine("9. Delete Author");
			Console.WriteLine("10. Exit");
			Console.Write("Select an option: ");
		}
		//Method to go back to the menu
		static void Pause()
		{
			Console.WriteLine("\nPress enter to return to the menu...");
			Console.ReadKey();
		}

		//Method to display stock
		static void ListStockLevels(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("Stock Levels for Each Store:\n");
			Console.ResetColor();

			var stores = context.Stores.ToList();
			foreach (var store in stores)
			{
				Console.WriteLine($"Store: {store.StoreName}");
				var inventory = context.Inventories
									   .Where(i => i.StoreId == store.Id)
									   .ToList();
				foreach (var inv in inventory)
				{
					var book = context.Books.FirstOrDefault(b => b.Isbn13 == inv.Isbn);
					Console.WriteLine($" - {book?.Title} - Quantity: {inv.Quantity}");
				}
				Console.WriteLine();
			}
			Pause();
		}
		//Method to add book to a store
		static void AddBookToStore(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\nAdd a Book to Store Inventory:");
			Console.ResetColor();

			var stores = context.Stores.ToList();
			Console.WriteLine("Select Store:");
			for (int i = 0; i < stores.Count; i++)
			{
				Console.WriteLine($"{i + 1}. {stores[i].StoreName}");
			}

			int storeChoice;
			while (true)
			{
				if (!int.TryParse(Console.ReadLine(), out storeChoice) || storeChoice < 1 || storeChoice > stores.Count)
				{
					Console.WriteLine("Invalid choice. Please enter a valid number.");
				}
				else
				{
					break;
				}
			}

			var store = stores[storeChoice - 1];

			var books = context.Books.ToList();
			if (books.Count == 0)
			{
				Console.WriteLine("No books available to add. Press enter to return to the menu.");
				Pause();
				return;
			}

			Console.WriteLine("\nSelect Book to Add:");
			for (int i = 0; i < books.Count; i++)
			{
				Console.WriteLine($"{i + 1}. {books[i].Title}");
			}

			int bookChoice;
			while (true)
			{
				if (!int.TryParse(Console.ReadLine(), out bookChoice) || bookChoice < 1 || bookChoice > books.Count)
				{
					Console.WriteLine("Invalid choice. Please enter a valid number.");
				}
				else
				{
					break;
				}
			}

			var book = books[bookChoice - 1];

			int quantity;
			while (true)
			{
				Console.Write("\nEnter Quantity to Add: ");
				if (!int.TryParse(Console.ReadLine(), out quantity) || quantity <= 0)
				{
					Console.WriteLine("Invalid quantity. Please enter a valid number.");
				}
				else
				{
					break;
				}
			}

			var existingInventory = context.Inventories
				.FirstOrDefault(i => i.StoreId == store.Id && i.Isbn == book.Isbn13);

			if (existingInventory != null)
			{
				existingInventory.Quantity += quantity;
				context.Inventories.Update(existingInventory);
			}
			else
			{
				var inventory = new Inventory
				{
					StoreId = store.Id,
					Isbn = book.Isbn13,
					Quantity = quantity
				};
				context.Inventories.Add(inventory);
			}

			context.SaveChanges();

			Console.WriteLine($"Successfully added {quantity} copies of {book.Title} to {store.StoreName}.");
			Pause();
		}
		//Method to remove a book from a store
		static void RemoveBookFromStore(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\nRemove a Book from Store Inventory:");
			Console.ResetColor();

			var stores = context.Stores.ToList();
			if (stores.Count == 0)
			{
				Console.WriteLine("No stores available. Press enter to return to the menu.");
				Pause();
				return;
			}

			Console.WriteLine("Select Store:");
			for (int i = 0; i < stores.Count; i++)
			{
				Console.WriteLine($"{i + 1}. {stores[i].StoreName}");
			}

			int storeChoice;
			while (true)
			{
				if (!int.TryParse(Console.ReadLine(), out storeChoice) || storeChoice < 1 || storeChoice > stores.Count)
				{
					Console.WriteLine("Invalid choice. Please enter a valid number.");
				}
				else
				{
					break;
				}
			}

			var store = stores[storeChoice - 1];

			var inventory = context.Inventories
								   .Where(i => i.StoreId == store.Id)
								   .ToList();

			if (inventory.Count == 0)
			{
				Console.WriteLine("No books available in this store's inventory. Press enter to return to the menu.");
				Pause();
				return;
			}

			Console.WriteLine("\nSelect Book to Remove:");
			for (int i = 0; i < inventory.Count; i++)
			{
				var book = context.Books.FirstOrDefault(b => b.Isbn13 == inventory[i].Isbn);
				if (book != null)
				{
					Console.WriteLine($"{i + 1}. {book.Title} (Quantity: {inventory[i].Quantity})");
				}
			}

			int bookChoice;
			while (true)
			{
				if (!int.TryParse(Console.ReadLine(), out bookChoice) || bookChoice < 1 || bookChoice > inventory.Count)
				{
					Console.WriteLine("Invalid choice. Please enter a valid number.");
				}
				else
				{
					break;
				}
			}

			var invToRemove = inventory[bookChoice - 1];
			var bookToRemove = context.Books.FirstOrDefault(b => b.Isbn13 == invToRemove.Isbn);

			if (bookToRemove == null)
			{
				Console.WriteLine("The selected book could not be found. Press enter to return to the menu.");
				Pause();
				return;
			}

			int quantityToRemove;
			while (true)
			{
				Console.Write($"\nEnter Quantity to Remove (current quantity: {invToRemove.Quantity}): ");
				if (!int.TryParse(Console.ReadLine(), out quantityToRemove) || quantityToRemove <= 0 || quantityToRemove > invToRemove.Quantity)
				{
					Console.WriteLine($"Invalid quantity. Please enter a valid number between 1 and {invToRemove.Quantity}.");
				}
				else
				{
					break;
				}
			}

			if (quantityToRemove >= invToRemove.Quantity)
			{
				Console.WriteLine($"Removing all copies of {bookToRemove.Title} from the inventory.");
				context.Inventories.Remove(invToRemove);
			}
			else
			{
				invToRemove.Quantity -= quantityToRemove;
				context.Inventories.Update(invToRemove);
			}

			context.SaveChanges();

			Console.WriteLine("Inventory updated.");
			Pause();
		}
		//Method to add a new book
		static void AddNewBook(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\nAdd New Book:");
			Console.ResetColor();

			string isbn13;
			while (true)
			{
				Console.Write("Enter Book ISBN-13: ");
				isbn13 = Console.ReadLine();

				if (string.IsNullOrEmpty(isbn13))
				{
					Console.WriteLine("ISBN-13 cannot be empty. Please try again.");
				}
				else if (context.Books.Any(b => b.Isbn13 == isbn13))
				{
					Console.WriteLine("A book with this ISBN-13 exists. Please enter a unique ISBN-13.");
				}
				else
				{
					break;
				}
			}

			string title;
			while (true)
			{
				Console.Write("Enter Book Title: ");
				title = Console.ReadLine();

				if (string.IsNullOrEmpty(title))
				{
					Console.WriteLine("Book title cannot be empty. Please try again.");
				}
				else
				{
					break;
				}
			}

			decimal price;
			while (true)
			{
				Console.Write("Enter Book Price: ");
				if (decimal.TryParse(Console.ReadLine(), out price) && price > 0)
				{
					break;
				}
				else
				{
					Console.WriteLine("Invalid price. Please enter a valid positive number.");
				}
			}

			string language;
			while (true)
			{
				Console.Write("Enter Book Language: ");
				language = Console.ReadLine();

				if (string.IsNullOrEmpty(language))
				{
					Console.WriteLine("Language cannot be empty. Please try again.");
				}
				else
				{
					break;
				}
			}

			DateOnly publicationDate;
			while (true)
			{
				Console.Write("Enter Publication Date (yyyy-mm-dd): ");
				if (DateOnly.TryParse(Console.ReadLine(), out publicationDate))
				{
					break;
				}
				else
				{
					Console.WriteLine("Invalid date format. Please enter the date in the format yyyy-mm-dd.");
				}
			}

			Console.WriteLine("\nSelect Author (or enter new author):");
			var authors = context.Authors.ToList();
			for (int i = 0; i < authors.Count; i++)
			{
				Console.WriteLine($"{i + 1}. {authors[i].FirstName} {authors[i].LastName}");
			}
			Console.WriteLine($"{authors.Count + 1}. Add new author");

			int authorChoice;
			while (true)
			{
				if (int.TryParse(Console.ReadLine(), out authorChoice) && authorChoice >= 1 && authorChoice <= authors.Count + 1)
				{
					break;
				}
				else
				{
					Console.WriteLine("Invalid choice. Please select a valid author.");
				}
			}

			Author author;
			if (authorChoice == authors.Count + 1)
			{
				string firstName, lastName;

				while (true)
				{
					Console.Write("Enter Author First Name: ");
					firstName = Console.ReadLine();

					if (string.IsNullOrEmpty(firstName))
					{
						Console.WriteLine("First name cannot be empty. Please try again.");
					}
					else
					{
						break;
					}
				}

				while (true)
				{
					Console.Write("Enter Author Last Name: ");
					lastName = Console.ReadLine();

					if (string.IsNullOrEmpty(lastName))
					{
						Console.WriteLine("Last name cannot be empty. Please try again.");
					}
					else
					{
						break;
					}
				}

				author = new Author
				{
					FirstName = firstName,
					LastName = lastName
				};
				context.Authors.Add(author);
				context.SaveChanges();
			}
			else
			{
				author = authors[authorChoice - 1];
			}

			var newBook = new Book
			{
				Isbn13 = isbn13,
				Title = title,
				Price = price,
				Language = language,
				PublicationDate = publicationDate,
				AuthorId = author.Id
			};

			context.Books.Add(newBook);
			context.SaveChanges();

			Console.WriteLine("\nNew book added to the catalog.");
			Pause();
		}

		//Method to add a new author
		static void AddNewAuthor(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\nAdd New Author:");
			Console.ResetColor();

			string firstName;
			while (true)
			{
				Console.Write("Enter First Name: ");
				firstName = Console.ReadLine();

				if (string.IsNullOrEmpty(firstName))
				{
					Console.WriteLine("First name cannot be empty. Please try again.");
				}
				else
				{
					break;
				}
			}

			string lastName;
			while (true)
			{
				Console.Write("Enter Last Name: ");
				lastName = Console.ReadLine();

				if (string.IsNullOrEmpty(lastName))
				{
					Console.WriteLine("Last name cannot be empty. Please try again.");
				}
				else
				{
					break;
				}
			}

			var author = new Author
			{
				FirstName = firstName,
				LastName = lastName
			};

			context.Authors.Add(author);
			context.SaveChanges();

			Console.WriteLine("\nNew author added.");
			Pause();
		}

		//Method to edit book
		static void EditBook(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\nEdit Book Details:");
			Console.ResetColor();

			var books = context.Books.Include(b => b.Author).ToList();
			for (int i = 0; i < books.Count; i++)
			{
				var author = books[i].Author;
				if (author != null)
				{
					Console.WriteLine($"{i + 1}. {books[i].Title} (by {author.FirstName} {author.LastName})");
				}
				else
				{
					Console.WriteLine($"{i + 1}. {books[i].Title} by [No Author Assigned]");
				}
			}

			Console.Write("Select a book to edit: ");
			int bookChoice;
			if (!int.TryParse(Console.ReadLine(), out bookChoice) || bookChoice < 1 || bookChoice > books.Count)
			{
				Console.WriteLine("Invalid choice. Please enter a valid number.");
				Pause();
				return;
			}

			var book = books[bookChoice - 1];

			Console.WriteLine("\nEnter new details (leave blank to keep current value):");
			Console.Write($"Title ({book.Title}): ");
			string title = Console.ReadLine();
			Console.Write($"Price ({book.Price}): ");
			string priceInput = Console.ReadLine();
			Console.Write($"Language ({book.Language}): ");
			string language = Console.ReadLine();
			Console.Write($"Publication Date ({book.PublicationDate}): ");
			string dateInput = Console.ReadLine();

			book.Title = string.IsNullOrEmpty(title) ? book.Title : title;
			book.Price = string.IsNullOrEmpty(priceInput) ? book.Price : decimal.Parse(priceInput);
			book.Language = string.IsNullOrEmpty(language) ? book.Language : language;
			book.PublicationDate = string.IsNullOrEmpty(dateInput) ? book.PublicationDate : DateOnly.Parse(dateInput);

			context.Books.Update(book);
			context.SaveChanges();

			Console.WriteLine("\nBook details updated.");
			Pause();
		}

		//Method to delete book
		static void DeleteBook(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\nDelete Book:");
			Console.ResetColor();

			var books = context.Books.Include(b => b.Author).ToList();

			if (!books.Any())
			{
				Console.WriteLine("No books available to delete.");
				Pause();
				return;
			}

			for (int i = 0; i < books.Count; i++)
			{
				var author = books[i].Author;
				string authorName = author != null ? $"{author.FirstName} {author.LastName}" : "Unknown Author";
				Console.WriteLine($"{i + 1}. {books[i].Title} by {authorName}");
			}

			Console.Write("Select a book to delete: ");
			if (!int.TryParse(Console.ReadLine(), out int bookChoice) || bookChoice < 1 || bookChoice > books.Count)
			{
				Console.WriteLine("Invalid selection. Please try again.");
				Pause();
				return;
			}

			var book = books[bookChoice - 1];

			try
			{
				var inventoryItems = context.Inventories.Where(i => i.Isbn == book.Isbn13).ToList();

				if (inventoryItems.Any())
				{
					Console.WriteLine($"This book has {inventoryItems.Count} inventory record(s). Deleting them...");

					context.Inventories.RemoveRange(inventoryItems);
					context.SaveChanges();

					Console.WriteLine("Related inventory records removed.");
				}
				else
				{
					Console.WriteLine("No related inventory records found for this book.");
				}

				context.Books.Remove(book);
				context.SaveChanges();

				Console.WriteLine("\nBook deleted from catalog.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while deleting the book: {ex.Message}");
			}

			Pause();
		}

		//Method to edit author
		static void EditAuthor(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Green;
			Console.WriteLine("\nEdit Author Details:");
			Console.ResetColor();

			var authors = context.Authors.ToList();

			if (!authors.Any())
			{
				Console.WriteLine("No authors available to edit.");
				Pause();
				return;
			}

			for (int i = 0; i < authors.Count; i++)
			{
				Console.WriteLine($"{i + 1}. {authors[i].FirstName} {authors[i].LastName}");
			}

			Console.Write("Select an author to edit: ");
			if (!int.TryParse(Console.ReadLine(), out int authorChoice) || authorChoice < 1 || authorChoice > authors.Count)
			{
				Console.WriteLine("Invalid selection. Please try again.");
				Pause();
				return;
			}

			var author = authors[authorChoice - 1];

			Console.WriteLine("\nEnter new details (leave blank to keep current value):");
			Console.Write($"First Name ({author.FirstName}): ");
			string firstName = Console.ReadLine();
			Console.Write($"Last Name ({author.LastName}): ");
			string lastName = Console.ReadLine();

			author.FirstName = string.IsNullOrEmpty(firstName) ? author.FirstName : firstName;
			author.LastName = string.IsNullOrEmpty(lastName) ? author.LastName : lastName;

			try
			{
				context.Authors.Update(author);
				context.SaveChanges();
				Console.WriteLine("\nAuthor details updated.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while updating the author details: {ex.Message}");
			}

			Pause();
		}

		//Method to delete an author
		static void DeleteAuthor(BookStoreDbContext context)
		{
			Console.Clear();
			Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("\nDelete Author:");
			Console.ResetColor();

			var authors = context.Authors.ToList();

			if (!authors.Any())
			{
				Console.WriteLine("No authors available to delete.");
				Pause();
				return;
			}

			for (int i = 0; i < authors.Count; i++)
			{
				Console.WriteLine($"{i + 1}. {authors[i].FirstName} {authors[i].LastName}");
			}

			int authorChoice;

			while (true)
			{
				Console.Write("Select an author to delete: ");
				string input = Console.ReadLine();

				if (int.TryParse(input, out authorChoice) && authorChoice >= 1 && authorChoice <= authors.Count)
				{
					break;
				}
				else
				{
					Console.WriteLine("Invalid selection. Please try again.");
				}
			}

			var author = authors[authorChoice - 1];

			var booksByAuthor = context.Books.Where(b => b.AuthorId == author.Id).ToList();
			if (booksByAuthor.Any())
			{
				Console.WriteLine("\nThis author has books in the catalog. Deleting the author will also delete their books.");
				Console.Write("Are you sure? (y/n): ");
				string confirm = Console.ReadLine().ToLower();

				if (confirm != "y")
				{
					Console.WriteLine("\nOperation canceled.");
					Pause();
					return;
				}

				try
				{
					context.Books.RemoveRange(booksByAuthor);
					context.SaveChanges();
					Console.WriteLine("\nBooks by this author have been deleted.");
				}
				catch (Exception ex)
				{
					Console.WriteLine($"An error occurred while deleting books: {ex.Message}");
					Pause();
					return;
				}
			}

			try
			{
				context.Authors.Remove(author);
				context.SaveChanges();
				Console.WriteLine("\nAuthor deleted from catalog.");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"An error occurred while deleting the author: {ex.Message}");
			}

			Pause();
		}

	}
}
