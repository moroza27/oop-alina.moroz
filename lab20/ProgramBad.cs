using System;

namespace lab20;

public class BadOrderExample
{
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

    public class OrderProcessor
    {
        public void ProcessOrder(Order order)
        {
            Console.WriteLine("Processing order " + order.Id);

            if (order.TotalAmount <= 0)
            {
                Console.WriteLine("Order invalid");
                order.Status = "Cancelled";
                return;
            }

            Console.WriteLine("Order saved");
            Console.WriteLine("Email sent to " + order.CustomerName);

            order.Status = "Processed";
            Console.WriteLine("Order done");
        }
    }
}
