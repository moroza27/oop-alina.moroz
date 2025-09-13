using System;

public class Time
{
    private int hour = 0;
    private int minutes = 0;
    private int seconds = 0;

    public int Hour
    {
        get { return hour; }
        set
        {
            if (0 <= value && value < 24)
                hour = value;
        }
    }

    public int Minutes
    {
        get { return minutes; }
        set
        {
            if (0 <= value && value < 60)
                minutes = value;
        }
    }

    public int Seconds
    {
        get { return seconds; }
        set
        {
            if (0 <= value && value < 60)
                seconds = value;
        }
    }
    public Time(int h = 0, int m = 0, int s = 0)
    {
        Hour = h;
        Minutes = m;
        Seconds = s;
    }

    public static Time operator +(Time t, int m)
    {
        int totalMinutes = t.hour * 60 + t.minutes + m;
        int newHour = (totalMinutes / 60) % 24;
        int newMinutes = totalMinutes % 60;
        return new Time(newHour, newMinutes, t.seconds);
    }

    public static Time operator -(Time t, int m)
    {
        int totalMinutes = t.hour * 60 + t.minutes - m;
        if (totalMinutes < 0) totalMinutes += 24 * 60;
        int newHour = (totalMinutes / 60) % 24;
        int newMinutes = totalMinutes % 60;
        return new Time(newHour, newMinutes, t.seconds);
    }

    public void ShowTime()
    {
        Console.WriteLine($"Time is {Hour:D2}:{Minutes:D2}:{Seconds:D2}");
    }
}
class Program
{
    static void Main()
    {
        Time t = new Time(10, 45, 30);
        t.ShowTime();

        t = t + 30;
        t.ShowTime();

        t = t - 50;
        t.ShowTime();
    }
}