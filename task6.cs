using System;
using System.Collections.Generic;
public class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; }
    public decimal TotalAmount { get; set; }
    public string ShippingType { get; set; }
    public bool IsValid { get; set; }
}

public interface IDiscount
{
    decimal Apply(decimal price);
}

public class PercentageDiscount : IDiscount
{
    private readonly decimal _percentage;

    public PercentageDiscount(decimal percentage)
    {
        if (percentage < 0m || percentage > 1m)
            throw new ArgumentOutOfRangeException(nameof(percentage), "Percentage must be between 0 and 1.");
        _percentage = percentage;
    }

    public decimal Apply(decimal price) => price - (price * _percentage);
}

public class NoDiscount : IDiscount
{
    public decimal Apply(decimal price) => price;
}

public interface IOrderValidator
{
    void Validate(Order order);
}

public class OrderValidator : IOrderValidator
{
    public void Validate(Order order)
    {
        if (order.TotalAmount <= 0)
        {
            order.IsValid = false;
            throw new ArgumentException("Order total must be greater than zero.");
        }

        if (string.IsNullOrWhiteSpace(order.CustomerEmail) || !order.CustomerEmail.Contains("@"))
        {
            order.IsValid = false;
            throw new ArgumentException("Invalid customer email address.");
        }

        order.IsValid = true;
    }
}

public interface IShippingStrategy
{
    string ShippingType { get; }
    decimal Calculate(Order order);
}

public class StandardShipping : IShippingStrategy
{
    public string ShippingType => "Standard";
    public decimal Calculate(Order order) => 5.99m;
}

public class ExpressShipping : IShippingStrategy
{
    public string ShippingType => "Express";
    public decimal Calculate(Order order) => 14.99m;
}

public class FreeShipping : IShippingStrategy
{
    public string ShippingType => "Free";
    public decimal Calculate(Order order) => 0.00m;
}

public interface IShippingCalculator
{
    decimal Calculate(Order order);
}

public class ShippingCalculator : IShippingCalculator
{
    private readonly Dictionary<string, IShippingStrategy> _strategies;

    public ShippingCalculator(IEnumerable<IShippingStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.ShippingType, StringComparer.OrdinalIgnoreCase);
    }

    public decimal Calculate(Order order)
    {
        if (!_strategies.TryGetValue(order.ShippingType ?? string.Empty, out var strategy))
            throw new ArgumentException($"Unknown shipping type: {order.ShippingType}");

        return strategy.Calculate(order);
    }
}

public interface IOrderRepository
{
    void Save(Order order);
}

public class SqlOrderRepository : IOrderRepository
{
    private readonly string _connectionString;

    public SqlOrderRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public void Save(Order order)
    {
        Console.WriteLine($"Saving order {order.Id} to database.");
    }
}

public interface IEmailSender
{
    void SendOrderConfirmation(Order order);
}

public class SmtpEmailSender : IEmailSender
{
    public void SendOrderConfirmation(Order order)
    {
        Console.WriteLine($"Sending confirmation email to {order.CustomerEmail} for order {order.Id}.");
    }
}

public interface IInvoiceGenerator
{
    string Generate(Order order);
}

public class TextInvoiceGenerator : IInvoiceGenerator
{
    public string Generate(Order order)
    {
        return $"Invoice for Order {order.Id}\nCustomer: {order.CustomerEmail}\nAmount: {order.TotalAmount:C}";
    }
}

public class OrderProcessor
{
    private readonly IOrderValidator _validator;
    private readonly IShippingCalculator _shippingCalculator;
    private readonly IOrderRepository _repository;
    private readonly IEmailSender _emailSender;
    private readonly IInvoiceGenerator _invoiceGenerator;

    public OrderProcessor(
        IOrderValidator validator,
        IShippingCalculator shippingCalculator,
        IOrderRepository repository,
        IEmailSender emailSender,
        IInvoiceGenerator invoiceGenerator)
    {
        _validator = validator;
        _shippingCalculator = shippingCalculator;
        _repository = repository;
        _emailSender = emailSender;
        _invoiceGenerator = invoiceGenerator;
    }

    public void ProcessOrder(Order order, IDiscount discount)
    {
        _validator.Validate(order);

        decimal shippingCost = _shippingCalculator.Calculate(order);
        decimal discountedTotal = discount.Apply(order.TotalAmount);

        _repository.Save(order);
        _emailSender.SendOrderConfirmation(order);
        string invoice = _invoiceGenerator.Generate(order);

        Console.WriteLine($"Order {order.Id} for {order.CustomerEmail} processed with total: {discountedTotal + shippingCost:C}");
        Console.WriteLine($"Shipping cost: {shippingCost:C}");
        Console.WriteLine(invoice);
    }
}

public static class Program
{
    public static void Main()
    {
        var shipping = new ShippingCalculator(new IShippingStrategy[]
        {
            new StandardShipping(),
            new ExpressShipping(),
            new FreeShipping()
        });

        var processor = new OrderProcessor(
            new OrderValidator(),
            shipping,
            new SqlOrderRepository("connection_string"),
            new SmtpEmailSender(),
            new TextInvoiceGenerator());

        var order = new Order
        {
            Id = 1,
            CustomerEmail = "ahmed@example.com",
            TotalAmount = 200m,
            ShippingType = "Express"
        };

        processor.ProcessOrder(order, new PercentageDiscount(0.10m));
        processor.ProcessOrder(order, new NoDiscount());
    }
}
