using System;
using System.Collections.Generic;
using System.Linq;

// Клас Book
public class Book
{
    public string Title { get; set; }
    public string Author { get; set; }
    public int Year { get; set; }
    public double Rating { get; set; }

    public Book(string title, string author, int year, double rating)
    {
        Title = title;
        Author = author;
        Year = year;
        Rating = rating;
    }
}

// Власний делегат
public delegate int MyMathDelegate(int a, int b); 

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Створення колекції книг
        List<Book> books = new List<Book>
        {
            new Book("The Pragmatic Programmer", "Andrew Hunt", 1999, 4.8),
            new Book("Clean Code", "Robert C. Martin", 2008, 4.7),
            new Book("Code Complete", "Steve McConnell", 2004, 4.3),
            new Book("Refactoring", "Martin Fowler", 2019, 4.6),
            new Book("Patterns of Enterprise Application Architecture", "Martin Fowler", 2002, 4.4)
        };

        // LINQ: вибір книг із рейтингом > 4.5
        var highRatedBooks = books.Where(b => b.Rating > 4.5);

        Console.WriteLine("Книги з рейтингом > 4.5:");
        foreach (var b in highRatedBooks)
            Console.WriteLine($"{b.Title} — {b.Rating}");

        // Групування за автором + сортування за роком
        var grouped = books
            .GroupBy(b => b.Author)
            .Select(g => new
            {
                Author = g.Key,
                Books = g.OrderBy(b => b.Year)
            });

        Console.WriteLine("\nГрупування за автором (сортування за роком):");
        foreach (var group in grouped)
        {
            Console.WriteLine($"\nАвтор: {group.Author}");
            foreach (var b in group.Books)
                Console.WriteLine($"  {b.Year} — {b.Title}");
                }

        // Анонімний метод (delegates)
        MyMathDelegate compareRatings = delegate (int r1, int r2)
        {
             // Повертаємо 1, якщо перший рейтинг вищий, -1 якщо нижчий, 0 якщо рівні
            if (r1 > r2) return 1;
            if (r1 < r2) return -1;
            return 0;
        };

        Console.WriteLine("\nАнонімний метод — порівняння рейтингів двох книг:");
        Console.WriteLine($"Clean Code vs Code Complete: {compareRatings(47, 43)}"); 

        // Лямбда-вираз для того ж делегата
        MyMathDelegate ratingDifference = (r1, r2) => r1 - r2;

        Console.WriteLine("Лямбда — різниця рейтингів Clean Code та Refactoring:");
        Console.WriteLine(ratingDifference(47, 46)); 

        // Використання Predicate<T>
        Predicate<Book> highRatingCheck = book => book.Rating > 4.5;
        var filtered = books.FindAll(highRatingCheck);

        Console.WriteLine("\nPredicate<Book> — книги з високим рейтингом:");
        filtered.ForEach(b => Console.WriteLine(b.Title));

        // Використання Func<T>
        Func<Book, string> formatBook = b => $"{b.Title} ({b.Year}) — {b.Rating}";
        Console.WriteLine("\nFunc<Book,string> — форматований вивід:");
        books.ForEach(b => Console.WriteLine(formatBook(b)));

        // Використання Action<T>
        Action<Book> printAction = b =>
        {
            Console.WriteLine($"Action => {b.Title}");
        };

        Console.WriteLine("\nAction<Book> — вивід назв:");
        books.ForEach(printAction);

        // Aggregate – обчислення середнього рейтингу
        double avgRating = books
            .Select(b => b.Rating)
            .Aggregate((acc, val) => acc + val) / books.Count;

        Console.WriteLine($"\nСередній рейтинг всіх книг: {avgRating:F2}");
    }
}