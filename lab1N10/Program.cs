using System;

public class Computer
{
  private string cpu;
  private int ram;

  public int Storage { get; set; }

  public Computer(string cpu, int ram, int storage)
  {
    this.cpu = cpu;
    this.ram = ram;
    Storage = storage;
    Console.WriteLine("Computer created.");
  }

  ~Computer()
  {
    Console.WriteLine("Computer destroyed.");
  }

  public void RunBenchmark()
  {
    Console.WriteLine($"Running benchmark on {cpu} with {ram} GB RAM and {Storage} GB storage...");
    Console.WriteLine("Benchmark completed!");
  }
}
