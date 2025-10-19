public interface IPackage 
{
  double Weight { get; }
  string TrackingNumber { get; }
  double CalculateCost();
}
//Базовий абстрактний клас з наслідуванням інтерфейсу
public abstract class BasePackage : IPackage
{
  public double Weight { get; protected set; }
  public string TrackingNumber { get; protected set; }
  public BasePackage(string trackingNumber, double weight)
  {
    this.TrackingNumber = trackingNumber;
    this.Weight = weight;
  }
  public abstract double CalculateCost();
}
//Реалізація класу BasePackage
public class StandardPackage : BasePackage
{
  public StandardPackage(string trackingNumber, double weight)
      : base(trackingNumber, weight) { }
  public override double CalculateCost()
  {
    return this.Weight * 10;
  }
}
//Ще одна реалізація класу BasePackage
public class ExpressPackage : BasePackage
{
  public ExpressPackage(string trackingNumber, double weight) : base(trackingNumber, weight) { }
  public override double CalculateCost()
  {
    double baseCost = this.Weight * 25;
    return baseCost + 50;
  }
}
//Клас з демонстрацією агрегації
public class DeliveryService
{
  private List<IPackage> currentBatch;

  public DeliveryService()
  {
    this.currentBatch = new List<IPackage>();
  }
  public void AddPackageToBatch(IPackage package)
  {
    Console.WriteLine($"Прийнято посилку: {package.TrackingNumber}");
    this.currentBatch.Add(package);
  }
  public double CalculateAverageCost()
  {
    if (currentBatch.Count == 0) return 0;
    double totalCost = 0;
    foreach (var package in currentBatch)
    {
      totalCost += package.CalculateCost();
    }
    return totalCost / currentBatch.Count;
  }
  public double GetMaxCost()
  {
    if (currentBatch.Count == 0) return 0;

    double maxCost = 0;
    foreach (var package in currentBatch)
    {
      double cost = package.CalculateCost();
      if (cost > maxCost)
      {
        maxCost = cost;
      }
    }
    return maxCost;
  }
  public void ShowAllPackagesInBatch()
  {
    Console.WriteLine($"\n--- Поточна партія (всього {currentBatch.Count} посилок) ---");
    foreach (var package in currentBatch)
    {
      Console.WriteLine($" - ID: {package.TrackingNumber}, Вага: {package.Weight} кг, Вартість: {package.CalculateCost()} грн");
    }
  }
}
//Демонстрація
public class Program
{
    public static void Main(string[] args)
    {
        DeliveryService service = new DeliveryService();
        IPackage p1 = new StandardPackage("UA1001", 2.5); 
        IPackage p2 = new ExpressPackage("UA1002", 1.0);  
        IPackage p3 = new StandardPackage("UA1003", 5.0); 
        service.AddPackageToBatch(p1);
        service.AddPackageToBatch(p2);
        service.AddPackageToBatch(p3);
        service.ShowAllPackagesInBatch();
        Console.WriteLine($"\nМаксимальна вартість: {service.GetMaxCost()} грн"); 
        Console.WriteLine($"Середня вартість: {service.CalculateAverageCost():F2} грн");
    }
}