using System;
using System.Collections.Generic;

// Порушення LSP
namespace Lab22.Bad
{
    // Базовий клас "Співробітник" припускає, що всі співробітники отримують зарплату
    public class Employee
    {
        public string Name { get; set; }

        public Employee(string name) => Name = name;

        public virtual decimal CalculateSalary()
        {
            return 5000m; // Базова ставка
        }
    }

    // Волонтер — це теж ніби співробітник, але...
    public class Volunteer : Employee
    {
        public Volunteer(string name) : base(name) { }

        // Порушення LSP
        public override decimal CalculateSalary()
        {
            throw new NotImplementedException("Волонтери працюють безкоштовно! Не можна рахувати зарплату.");
        }
    }

    public class SalaryProcessor
    {
        public void ProcessSalaries(List<Employee> employees)
        {
            foreach (var emp in employees)
            {
                try
                {
                    Console.WriteLine($"Співробітник: {emp.Name}, Зарплата: {emp.CalculateSalary()} грн");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ПОМИЛКА] Співробітник {emp.Name}: {ex.Message}");
                }
            }
        }
    }
}

// Код з дотриманням LSP
namespace Lab22.Good
{
    // Спільний базовий клас із загальними даними
    public abstract class StaffMember
    {
        public string Name { get; set; }
        public StaffMember(string name) => Name = name;
    }

    // Інтерфейс для тих, хто отримує зарплату
    public interface IPayable
    {
        decimal CalculateSalary();
    }

    //Звичайний співробітник
    public class Employee : StaffMember, IPayable
    {
        public Employee(string name) : base(name) { }

        public decimal CalculateSalary()
        {
            return 5000m; 
        }
    }

    // Волонтер
    public class Volunteer : StaffMember
    {
        public Volunteer(string name) : base(name) { }
        
    }
}

// Main
namespace Lab22.App
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // --- ДЕМОНСТРАЦІЯ ПОРУШЕННЯ LSP ---
            Console.WriteLine("=== Bad implementation (LSP Violation) ===");
            var badEmployees = new List<Lab22.Bad.Employee>
            {
                new Lab22.Bad.Employee("Іван (Штатний)"),
                new Lab22.Bad.Volunteer("Марія (Волонтер)") // Волонтер прикидається Employee
            };

            var badProcessor = new Lab22.Bad.SalaryProcessor();
            // Тут програма "спіткнеться" на волонтері
            badProcessor.ProcessSalaries(badEmployees);


            Console.WriteLine("\n-----------------------------\n");


            // Демонстрація коректного коду
            Console.WriteLine("=== Good implementation (Correct Logic) ===");
            
            // Список всього персоналу
            var allStaff = new List<Lab22.Good.StaffMember>
            {
                new Lab22.Good.Employee("Олег (Штатний)"),
                new Lab22.Good.Volunteer("Ольга (Волонтер)")
            };

            Console.WriteLine("-> Список персоналу:");
            foreach (var person in allStaff)
            {
                Console.WriteLine($"- {person.Name}");
                // person.CalculateSalary(); // Помилка компіляції! Ми не можемо випадково викликати це у волонтера.
            }

            Console.WriteLine("\n-> Розрахунок зарплат:");
            
            // Фільтруємо або створюємо список саме тих, кому треба платити
            List<Lab22.Good.IPayable> payableStaff = new List<Lab22.Good.IPayable>();

            foreach (var person in allStaff)
            {
                if (person is Lab22.Good.IPayable payable)
                {
                    payableStaff.Add(payable);
                }
            }

            // Обробка зарплат
            foreach (var staff in payableStaff)
            {
                Console.WriteLine($"Виплата: {staff.CalculateSalary()} грн для об'єкта {staff.GetType().Name}");
            }

            Console.ReadLine();
        }
    }
}