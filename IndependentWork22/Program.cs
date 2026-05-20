using System;
using System.Collections.Generic;

namespace IndependentWork22
{
    // ПАТЕРН COMPOSITE

    public interface IComponent
    {
        string GetDescription();
    }

    public class SingleIngredient : IComponent
    {
        public string Name { get; set; }
        public string Quantity { get; set; }

        public SingleIngredient(string name, string quantity)
        {
            Name = name;
            Quantity = quantity;
        }

        public string GetDescription()
        {
            return $"{Name} ({Quantity})";
        }
    }

    public class Recipe : IComponent
    {
        public string Name { get; set; }
        private List<IComponent> _components = new List<IComponent>();

        public Recipe(string name)
        {
            Name = name;
        }

        public void Add(IComponent component)
        {
            _components.Add(component);
        }

        public void Remove(IComponent component)
        {
            _components.Remove(component);
        }

        public string GetDescription()
        {
            string result = $"[Рецепт: {Name}]\n";
            foreach (var component in _components)
            {
                string componentDescription = component.GetDescription();
                string[] lines = componentDescription.Split('\n');
                foreach (var line in lines)
                {
                    if (!string.IsNullOrEmpty(line))
                    {
                        result += $"   - {line}\n";
                    }
                }
            }
            return result.TrimEnd();
        }
    }

    // ПАТЕРН DECORATOR

    // Абстрактний клас Decorator
    public abstract class Decorator : IComponent
    {
        protected IComponent _component;

        public Decorator(IComponent component)
        {
            _component = component;
        }

        public virtual string GetDescription()
        {
            return _component.GetDescription();
        }
    }

    // Декоратори
    public class OrganicDecorator : Decorator
    {
        public OrganicDecorator(IComponent component) : base(component) { }

        public override string GetDescription()
        {
            return "Organic " + base.GetDescription();
        }
    }

    public class SpicyDecorator : Decorator
    {
        public SpicyDecorator(IComponent component) : base(component) { }

        public override string GetDescription()
        {
            return base.GetDescription() + " (Spicy)";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Система інгредієнтів та рецептів ===\n");

            IComponent tomato = new SingleIngredient("Помідор", "2 шт.");
            IComponent garlic = new SingleIngredient("Часник", "1 зубчик");
            IComponent pepper = new SingleIngredient("Перець чилі", "10 г");
            IComponent pasta = new SingleIngredient("Паста", "200 г");

            IComponent organicTomato = new OrganicDecorator(tomato);
            IComponent spicyPepper = new SpicyDecorator(pepper);
            
            IComponent organicSpicyGarlic = new SpicyDecorator(new OrganicDecorator(garlic));

            Console.WriteLine("--- Окремі (декоровані) інгредієнти ---");
            Console.WriteLine(organicTomato.GetDescription());
            Console.WriteLine(organicSpicyGarlic.GetDescription());
            Console.WriteLine();

            Recipe spicySauce = new Recipe("Гострий томатний соус");
            spicySauce.Add(organicTomato);
            spicySauce.Add(organicSpicyGarlic);
            spicySauce.Add(spicyPepper);

            Recipe mainDish = new Recipe("Паста Фарфале");
            mainDish.Add(pasta);
            mainDish.Add(spicySauce);

            Console.WriteLine("--- Вивід складної структури (Composite) ---");
            Console.WriteLine(mainDish.GetDescription());
            Console.WriteLine();

            IComponent organicMainDish = new OrganicDecorator(mainDish);

            Console.WriteLine("--- Вивід декорованого рецепта (Decorator applied to Composite) ---");
            Console.WriteLine(organicMainDish.GetDescription());

            Console.WriteLine("\nРоботу завершено. Натисніть будь-яку клавішу...");
            Console.ReadKey();
        }
    }
}