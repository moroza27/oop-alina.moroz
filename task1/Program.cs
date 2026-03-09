List<int> numbers = new List<int>();

static IEnumerable<int> Range()
{
  for (int i=1; i<=10000; i++)
  {
    yield return i;
  }
}
var list = Range().ToList();
int count = list.Count(n => n % 3 == 0);
Console.WriteLine($"Кількість чисел, що діляться на 3 : {count}");
