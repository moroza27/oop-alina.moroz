using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
public class SeatAlreadyBookedException : Exception
{
    public SeatAlreadyBookedException(string message) : base(message) { }
}
public class InvalidSeatException : Exception
{
    public InvalidSeatException(string message) : base(message) { }
}
public class Matrix<T>
{
    private readonly T[,] _data;

    public int Rows { get; }
    public int Cols { get; }

    public Matrix(int rows, int cols)
    {
        if (rows <= 0 || cols <= 0)
        {
            throw new ArgumentException("Розміри матриці мають бути додатними.");
        }
        Rows = rows;
        Cols = cols;
        _data = new T[rows, cols];
    }
    public T this[int row, int col]
    {
        get
        {
            ValidateBounds(row, col);
            return _data[row, col];
        }
        set
        {
            ValidateBounds(row, col);
            _data[row, col] = value;
        }
    }
    private void ValidateBounds(int row, int col)
    {
        if (row < 0 || row >= Rows || col < 0 || col >= Cols)
        {
            throw new IndexOutOfRangeException($"Індекси ({row}, {col}) виходять за межі матриці ({Rows}x{Cols}).");
        }
    }
}
public enum SeatStatus
{
    Free,
    Booked
}
public class Seat
{
  public int Row { get; }
  public int Number { get; }
  public decimal Price { get; }
  public SeatStatus Status { get; private set; }

  public Seat(int row, int number, decimal price)
  {
    Row = row;
    Number = number;
    Price = price;
    Status = SeatStatus.Free;
  }
  public void Book()
  {
    // Контроль стану
    if (Status == SeatStatus.Booked)
    {
      throw new SeatAlreadyBookedException($"Місце {Row}-{Number} вже заброньоване.");
    }
    Status = SeatStatus.Booked;
  }

  public override string ToString()
  {
    // Змінено для кращої візуалізації (X - зайнято, 3,5 - вільно)
    return $"[{(Status == SeatStatus.Free ? $"{Row},{Number}" : " X ")}]";
  }
}
//Клас з бронюванням
public class Reservation
{
    public Guid Id { get; }
    public string CustomerName { get; }
    public int Row { get; }
    public int SeatNumber { get; }
    public decimal PricePaid { get; }

    public Reservation(string customerName, int row, int seatNumber, decimal pricePaid)
    {
        Id = Guid.NewGuid();
        CustomerName = customerName;
        Row = row;
        SeatNumber = seatNumber;
        PricePaid = pricePaid;
    }
}
// Клас зал
public class Hall
{
    public string Name { get; }

    // Приклад композиції
    private readonly Matrix<Seat> _seats;

    // Приклад агрегації
    private readonly List<Reservation> _reservations;
    public IReadOnlyCollection<Reservation> Reservations => _reservations.AsReadOnly();

    public Hall(string name, int rows, int seatsPerRow, decimal defaultPrice)
    {
        Name = name;
        _reservations = new List<Reservation>();

        _seats = new Matrix<Seat>(rows, seatsPerRow);
        for (int r = 0; r < rows; r++)
        {
            for (int s = 0; s < seatsPerRow; s++)
            {
                _seats[r, s] = new Seat(r + 1, s + 1, defaultPrice);
            }
        }
    }
    //Метод для бронювання місць
    public Reservation BookSeat(int row, int seatNumber, string customerName)
    {
        Seat seat = GetSeat(row, seatNumber);
        seat.Book(); 
        var reservation = new Reservation(customerName, row, seatNumber, seat.Price);
        _reservations.Add(reservation);
        Console.WriteLine($"Успіх! {customerName} забронював(ла) місце {row}-{seatNumber}.");
        return reservation;
    }
    public Seat GetSeat(int row, int seatNumber)
    {
        int rowIndex = row - 1;
        int colIndex = seatNumber - 1;
        if (rowIndex < 0 || rowIndex >= _seats.Rows || colIndex < 0 || colIndex >= _seats.Cols)
        {
            throw new InvalidSeatException($"Місця {row}-{seatNumber} не існує у залі '{Name}'.");
        }

        return _seats[rowIndex, colIndex];
    }

    // Обчислення
    public decimal GetTotalRevenue()
    {
        return _reservations.Sum(r => r.PricePaid);
    }
    // Обчислення завантаженості залу (у відсотках)
    public double GetOccupancyPercentage()
    {
        int totalSeats = _seats.Rows * _seats.Cols;
        int bookedSeats = _reservations.Count;

        if (totalSeats == 0) return 0;

        return (double)bookedSeats / totalSeats * 100.0;
    }
    /// Обчислення найпопулярніших рядів
    public List<int> GetMostPopularRows()
    {
        if (!_reservations.Any())
        {
            return new List<int>(); 
        }

        var rowCounts = _reservations
            .GroupBy(r => r.Row) 
            .Select(g => new { Row = g.Key, Count = g.Count() }) 
            .OrderByDescending(g => g.Count) 
            .ToList();

        int maxCount = rowCounts.First().Count;

        return rowCounts
            .Where(r => r.Count == maxCount)
            .Select(r => r.Row)
            .ToList();
    }

    // Допоміжний метод для візуалізації
    public void PrintLayout()
    {
        Console.WriteLine($"\n--- Схема залу '{Name}' ---");
        for (int r = 0; r < _seats.Rows; r++)
        {
            for (int s = 0; s < _seats.Cols; s++)
            {
                Console.Write(_seats[r, s].ToString().PadRight(6));
            }
            Console.WriteLine();
        }
        Console.WriteLine("---------------------------\n");
    }
}

// Демонстрація
class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        var mainHall = new Hall(name: "Зал 1 (IMAX)", rows: 5, seatsPerRow: 8, defaultPrice: 150m);

        Console.WriteLine($"Створено зал: {mainHall.Name}");
        mainHall.PrintLayout();

        try
        {
            mainHall.BookSeat(row: 3, seatNumber: 5, customerName: "Іван");
            mainHall.BookSeat(row: 3, seatNumber: 6, customerName: "Марія");
            mainHall.BookSeat(row: 4, seatNumber: 1, customerName: "Петро");
            mainHall.BookSeat(row: 1, seatNumber: 1, customerName: "Ольга");
            mainHall.BookSeat(row: 3, seatNumber: 4, customerName: "Андрій"); // 3-й ряд популярний

            Console.WriteLine("\nCпроба подвійного бронювання");
            
            mainHall.BookSeat(row: 3, seatNumber: 5, customerName: "Віктор");
        }
        catch (SeatAlreadyBookedException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"ПОМИЛКА БРОНЮВАННЯ: {ex.Message}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Загальна помилка: {ex.Message}");
            Console.ResetColor();
        }

        try
        {
            Console.WriteLine("\nСпроба забронювати неіснуюче місце");
            
            mainHall.BookSeat(row: 99, seatNumber: 99, customerName: "Привид");
        }
        catch (InvalidSeatException ex)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"ПОМИЛКА ВВОДУ: {ex.Message}");
            Console.ResetColor();
        }

        mainHall.PrintLayout();

        Console.WriteLine("СТАТИСТИКА ЗАЛУ");

        decimal revenue = mainHall.GetTotalRevenue();
        Console.WriteLine($"Загальний дохід: {revenue:C}"); 

        double occupancy = mainHall.GetOccupancyPercentage();
        Console.WriteLine($"Завантаженість залу: {occupancy:F2}%"); 

        var popularRows = mainHall.GetMostPopularRows();
        Console.WriteLine($"Найпопулярніші ряди: {string.Join(", ", popularRows)}");
    }
}