using System;
using System.Collections.Generic;

public abstract class LibraryItem
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;

    public abstract string GetDetails();
}

public class Book : LibraryItem
{
    public bool IsAvailable { get; set; }

    public override string GetDetails() =>
        $"Book: {Title} by {Author} — Available: {IsAvailable}";
}

public class EBook : LibraryItem
{
    public double FileSizeMB { get; set; }

    public override string GetDetails() =>
        $"EBook: {Title} by {Author} — {FileSizeMB}MB";
}

public class Member
{
    public string Name { get; set; } = string.Empty;
    public int MemberId { get; set; }
    public List<Book> BorrowedBooks { get; set; }

    public Member()
    {
        BorrowedBooks = new List<Book>();
    }
}

public interface INotifier
{
    void Notify(string message);
}

public interface IBorrowable
{
    void Borrow(Member member, Book book);
    void Return(Member member, Book book);
}

public interface ISearchable
{
    List<Book> Search(string keyword);
}

public interface IBookRepository
{
    void Add(Book book);
    Book FindByISBN(string isbn);
    List<Book> GetAll();
}

public class BookRepository : IBookRepository, ISearchable
{
    private readonly List<Book> _books = new List<Book>();

    public void Add(Book book)
    {
        _books.Add(book);
    }

    public Book FindByISBN(string isbn)
    {
        return _books.FirstOrDefault(b => b.ISBN == isbn);
    }

    public List<Book> GetAll()
    {
        return new List<Book>(_books);
    }

    public List<Book> Search(string keyword)
    {
        if (string.IsNullOrWhiteSpace(keyword))
        {
            return new List<Book>();
        }

        return _books
            .Where(b => b.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)
                     || b.Author.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList();
    }
}

public class ConsoleNotifier : INotifier
{
    public void Notify(string message) =>
        Console.WriteLine($"[Library] {message}");
}

public class EmailNotifier : INotifier
{
    public void Notify(string message) =>
        Console.WriteLine($"[EMAIL] Sending: {message}");
}

public class MultiNotifier : INotifier
{
    private readonly List<INotifier> _notifiers;

    public MultiNotifier(List<INotifier> notifiers)
    {
        _notifiers = notifiers;
    }

    public void Notify(string message)
    {
        foreach (var notifier in _notifiers)
        {
            notifier.Notify(message);
        }
    }
}

public class LibraryNotifier
{
    private readonly INotifier _notifier;

    public LibraryNotifier(INotifier notifier)
    {
        _notifier = notifier;
    }

    public void Announce(string message)
    {
        _notifier.Notify(message);
    }
}

public class BorrowingService : IBorrowable
{
    private readonly IBookRepository _repository;
    private readonly INotifier _notifier;

    public BorrowingService(IBookRepository repository, INotifier notifier)
    {
        _repository = repository;
        _notifier = notifier;
    }

    public void Borrow(Member member, Book book)
    {
        if (book == null)
        {
            _notifier.Notify($"{member.Name} requested a book that does not exist.");
            return;
        }

        if (!book.IsAvailable)
        {
            _notifier.Notify($"'{book.Title}' is not available for {member.Name}.");
            return;
        }

        book.IsAvailable = false;
        member.BorrowedBooks.Add(book);
        _notifier.Notify($"{member.Name} borrowed '{book.Title}'.");
    }

    public void Return(Member member, Book book)
    {
        if (book == null || !member.BorrowedBooks.Contains(book))
        {
            _notifier.Notify($"{member.Name} cannot return a book that was not borrowed.");
            return;
        }

        book.IsAvailable = true;
        member.BorrowedBooks.Remove(book);
        _notifier.Notify($"{member.Name} returned '{book.Title}'.");
    }
}

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var repo = new BookRepository();
        INotifier notifier = new ConsoleNotifier();
        var service = new BorrowingService(repo, notifier);

        repo.Add(new Book { Title = "Clean Code", Author = "Robert Martin", ISBN = "001", IsAvailable = true });
        repo.Add(new Book { Title = "The Pragmatic Programmer", Author = "Hunt & Thomas", ISBN = "002", IsAvailable = true });
        repo.Add(new Book { Title = "Design Patterns", Author = "Gang of Four", ISBN = "003", IsAvailable = true });

        var items = new List<LibraryItem>
        {
            repo.FindByISBN("001"),
            repo.FindByISBN("002"),
            new EBook { Title = "C# in Depth", Author = "Jon Skeet", ISBN = "E001", FileSizeMB = 3.7 }
        };

        var member = new Member { Name = "Ahmed", MemberId = 1 };
        service.Borrow(member, repo.FindByISBN("001"));
        service.Borrow(member, repo.FindByISBN("002"));

        service.Return(member, repo.FindByISBN("001"));

        Console.WriteLine($"\n{member.Name}'s borrowed books:");
        foreach (var book in member.BorrowedBooks)
        {
            Console.WriteLine($"  - {book.Title}");
        }

        Console.WriteLine("\nAll library items:");
        foreach (var item in items)
        {
            Console.WriteLine(item.GetDetails());
        }

        Console.WriteLine("\nSearch results for 'PRAGMATIC':");
        foreach (var book in repo.Search("PRAGMATIC"))
        {
            Console.WriteLine($"  - {book.Title}");
        }

        Console.WriteLine("\nMultiNotifier demo:");
        var multiNotifier = new MultiNotifier(new List<INotifier>
        {
            new ConsoleNotifier(),
            new EmailNotifier()
        });
        var multiService = new BorrowingService(repo, multiNotifier);
        multiService.Borrow(member, repo.FindByISBN("003"));

        var libraryNotifier = new LibraryNotifier(multiNotifier);
        libraryNotifier.Announce("Library closes at 6 PM today.");
    }
}
