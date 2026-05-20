using System;
using System.Collections.Generic;
using System.Linq;

namespace IndependentWork24
{
    public interface IOrderComponent
    {
        decimal CalculateTotal();
    }

    public class Product : IOrderComponent
    {
        public string Name { get; }
        private readonly decimal _price;

        public Product(string name, decimal price)
        {
            Name = name;
            _price = price;
        }

        public decimal CalculateTotal() => _price;
    }

    // Composite
    public class Box : IOrderComponent
    {
        private readonly List<IOrderComponent> _items = new List<IOrderComponent>();

        public void Add(IOrderComponent component) => _items.Add(component);
        public void Remove(IOrderComponent component) => _items.Remove(component);

        public decimal CalculateTotal()
        {
            System.Threading.Thread.Sleep(10); 
            return _items.Sum(item => item.CalculateTotal());
        }
    }

    // Базовий декоратор
    public abstract class OrderDecorator : IOrderComponent
    {
        protected readonly IOrderComponent _component;

        protected OrderDecorator(IOrderComponent component)
        {
            _component = component;
        }

        public virtual decimal CalculateTotal() => _component.CalculateTotal();
    }

    // Декоратори
    public class DiscountDecorator : OrderDecorator
    {
        private readonly decimal _discountPercentage;

        public DiscountDecorator(IOrderComponent component, decimal discountPercentage) 
            : base(component)
        {
            _discountPercentage = discountPercentage;
        }

        public override decimal CalculateTotal()
        {
            decimal total = base.CalculateTotal();
            return total - (total * (_discountPercentage / 100m));
        }
    }

    // Proxy
    public class SecureCachedOrderProxy : IOrderComponent
    {
        private readonly IOrderComponent _realOrder;
        private readonly string _userRole;
        private decimal? _cachedTotal = null;

        public SecureCachedOrderProxy(IOrderComponent realOrder, string userRole)
        {
            _realOrder = realOrder;
            _userRole = userRole;
        }

        public decimal CalculateTotal()
        {
            if (_userRole != "Manager" && _userRole != "Admin")
            {
                throw new UnauthorizedAccessException("У вас немає прав для перегляду вартості цього замовлення.");
            }

            if (_cachedTotal == null)
            {
                _cachedTotal = _realOrder.CalculateTotal();
            }

            return _cachedTotal.Value;
        }
    }
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система розрахунку вартості замовлень ===");
            Console.WriteLine("Архітектура завантажена успішно.");
            Console.WriteLine("Для перевірки логіки запустіть проєкт із тестами (IndependentWork24.Tests).");
            
            // Невеличка демонстрація для консолі
            var rootBox = new Box();
            rootBox.Add(new Product("Тестовий ноутбук", 3000m));
            
            var proxy = new SecureCachedOrderProxy(rootBox, "Manager");
            Console.WriteLine($"\nВартість через Proxy: {proxy.CalculateTotal()} грн");

            Console.WriteLine("\nНатисніть будь-яку клавішу для виходу...");
            Console.ReadKey();
        }
    }
}