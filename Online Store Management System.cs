
using System;
using System.Collections.Generic;

interface IProduct
{
    decimal CalculateFinalPrice();
    void DisplayDetails();
}

abstract class Product : IProduct
{
    private string _name;
    private decimal _price;
    private decimal _discountPercentage;

    public int ProductId { get; private set; }

    public string Name
    {
        get { return _name; }
        set
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Product name cannot be empty.");

            _name = value;
        }
    }

    public decimal Price
    {
        get { return _price; }
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value), "Price must be greater than zero.");

            _price = value;
        }
    }

    public decimal DiscountPercentage
    {
        get { return _discountPercentage; }
        set
        {
            if (value < 0 || value > 100)
                throw new ArgumentOutOfRangeException(
                    nameof(value), "Discount must be between 0 and 100.");

            _discountPercentage = value;
        }
    }

    public Product(int productId, string name,
                   decimal price, decimal discountPercentage)
    {
        if (productId <= 0)
            throw new ArgumentOutOfRangeException(nameof(productId));

        ProductId = productId;
        Name = name;
        Price = price;
        DiscountPercentage = discountPercentage;
    }

    protected decimal GetDiscountedPrice()
    {
        return Price - (Price * DiscountPercentage / 100);
    }

    public abstract decimal CalculateFinalPrice();

    public virtual void DisplayDetails()
    {
        Console.WriteLine($"Product ID: {ProductId}");
        Console.WriteLine($"Name: {Name}");
        Console.WriteLine($"Price: {Price}");
        Console.WriteLine($"Discount: {DiscountPercentage}%");
    }
}

class PhysicalProduct : Product
{
    private decimal _shippingCost;

    public decimal ShippingCost
    {
        get { return _shippingCost; }
        set
        {
            if (value < 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value), "Shipping cost cannot be negative.");

            _shippingCost = value;
        }
    }

    public PhysicalProduct(int id, string name, decimal price,
                           decimal discount, decimal shippingCost)
        : base(id, name, price, discount)
    {
        ShippingCost = shippingCost;
    }

    public override decimal CalculateFinalPrice()
    {
        return GetDiscountedPrice() + ShippingCost;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"Shipping Cost: {ShippingCost}");
        Console.WriteLine("Type: Physical Product");
    }
}

class DigitalProduct : Product
{
    private string _downloadLink;

    public string DownloadLink
    {
        get { return _downloadLink; }
        set
        {
            if (!Uri.TryCreate(value, UriKind.Absolute, out _))
                throw new ArgumentException("Invalid download link.");

            _downloadLink = value;
        }
    }

    public DigitalProduct(int id, string name, decimal price,
                          decimal discount, string downloadLink)
        : base(id, name, price, discount)
    {
        DownloadLink = downloadLink;
    }

    public override decimal CalculateFinalPrice()
    {
        return GetDiscountedPrice();
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"Download Link: {DownloadLink}");
        Console.WriteLine("Type: Digital Product");
    }
}

class SubscriptionProduct : Product
{
    private int _durationMonths;

    public int DurationMonths
    {
        get { return _durationMonths; }
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(
                    nameof(value), "Duration must be greater than zero.");

            _durationMonths = value;
        }
    }

    public SubscriptionProduct(int id, string name, decimal monthlyPrice,
                               decimal discount, int durationMonths)
        : base(id, name, monthlyPrice, discount)
    {
        DurationMonths = durationMonths;
    }

    public override decimal CalculateFinalPrice()
    {
        return GetDiscountedPrice() * DurationMonths;
    }

    public override void DisplayDetails()
    {
        base.DisplayDetails();
        Console.WriteLine($"Duration: {DurationMonths} Months");
        Console.WriteLine("Type: Subscription Product");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("===== ONLINE STORE =====");

        PhysicalProduct headphones = new PhysicalProduct(
            1, "Headphones", 2000, 10, 100);

        DigitalProduct ebook = new DigitalProduct(
            2, "C# E-Book", 500, 20,
            "https://example.com/ebook");

        SubscriptionProduct subscription = new SubscriptionProduct(
            3, "Premium Subscription", 100, 10, 12);

        Console.WriteLine("\n===== VALIDATION TESTS =====");

        try
        {
            headphones.Price = -500;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine("Invalid Price: " + ex.Message);
        }

        try
        {
            ebook.DiscountPercentage = 150;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine("Invalid Discount: " + ex.Message);
        }

        try
        {
            subscription.DurationMonths = 0;
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Console.WriteLine("Invalid Duration: " + ex.Message);
        }

        List<IProduct> products = new List<IProduct>();

        products.Add(headphones);
        products.Add(ebook);
        products.Add(subscription);

        Console.WriteLine("\n===== STORE PRODUCTS =====");

        foreach (IProduct product in products)
        {
            product.DisplayDetails();

            Console.WriteLine(
                $"Final Price: {product.CalculateFinalPrice():N2} EGP");

            Console.WriteLine("--------------------------");
        }

        Console.WriteLine("\n===== END OF PROGRAM =====");
    }
}
