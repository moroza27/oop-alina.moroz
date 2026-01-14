using System;

namespace lab20;

class ProgramGood
{
    // Клас даних
    public class Order
    {
        public int Id;
        public string CustomerName;
        public decimal TotalAmount;
        public string Status;

        public Order(int id, string name, decimal amount)
        {
            Id = id;
            CustomerName = name;
            TotalAmount = amount;
            Status = "New";
        }
    }

    // Інтерфейси
    interface IValidator { bool IsValid(Order order); }
    interface IRepository { void Save(Order order); }
    interface IEmail { void Send(Order order); }

    // Реалізації
    class SimpleValidator : IValidator
    {
        public bool IsValid(Order order) => order.TotalAmount > 0;
    }

    class SimpleRepository : IRepository
    {
        public void Save(Order order) => Console.WriteLine("Order saved");
    }

    class ConsoleEmail : IEmail
    {
        public void Send(Order order) => Console.WriteLine("Email sent to " + order.CustomerName);
    }

    // Сервіс (координує)
    class OrderService
    {
        private IValidator validator;
        private IRepository repository;
        private IEmail email;

        public OrderService(IValidator v, IRepository r, IEmail e)
        {
            validator = v;
            repository = r;
            email = e;
        }

        public void Process(Order order)
        {
            if (!validator.IsValid(order))
            {
                Console.WriteLine("Order invalid");
                order.Status = "Cancelled";
                return;
            }

            repository.Save(order);
            email.Send(order);

            order.Status = "Processed";
            Console.WriteLine("Order done");
        }
    }

    static void Main()
    {
        Order goodOrder = new Order(1, "Anna", 500);
        Order badOrder = new Order(2, "Oleh", -100);

        OrderService service = new OrderService(
            new SimpleValidator(),
            new SimpleRepository(),
            new ConsoleEmail()
        );

        service.Process(goodOrder);
        Console.WriteLine("Final status: " + goodOrder.Status);

        Console.WriteLine();

        service.Process(badOrder);
        Console.WriteLine("Final status: " + badOrder.Status);
    }
}