public class Ticket
{
  public virtual void GetPrice()
  {
    Console.Write("The cost of the trip for 5 people ");
  }
}
public class BusTicket : Ticket
{
  public int price_b = 55;
  public int PriceOfTravel()
  {
    return price_b * 5;
  }
  public override void GetPrice()
  {
    base.GetPrice();
    Console.WriteLine($"by bus = {PriceOfTravel()} $");
  }
}
public class TrainTicket : Ticket
{
  public int price_t = 120;
  public int PriceOfBusTravel()
  {
    return price_t * 5;
  } 
  public override void GetPrice()
  {
    base.GetPrice();
    Console.WriteLine($"by train = {PriceOfBusTravel()} $");
  }
}
public class Program
{
  static void Main()
  {
    Ticket bus = new BusTicket();
    Ticket train = new TrainTicket();
    bus.GetPrice();
    train.GetPrice();
  }
}