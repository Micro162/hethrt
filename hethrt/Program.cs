using System;
using System.IO;
using System.Threading;

public class Bank
{
    private int money;
    private string name;
    private int percent;

    private readonly string filePath;
    private readonly object fileLock = new object(); 

    public Bank(string name, int money, int percent, string filePath = "bank_data.txt")
    {
        this.name = name;
        this.money = money;
        this.percent = percent;
        this.filePath = filePath;
    }

    public int Money
    {
        get => money;
        set
        {
            if (money == value) return;
            money = value;
            SaveToFileAsync();
        }
    }

    public string Name
    {
        get => name;
        set
        {
            if (name == value) return;
            name = value;
            SaveToFileAsync();
        }
    }

    public int Percent
    {
        get => percent;
        set
        {
            if (percent == value) return;
            percent = value;
            SaveToFileAsync();
        }
    }

    private void SaveToFileAsync()
    {
        int snapshotMoney = money;
        string snapshotName = name;
        int snapshotPercent = percent;

        Thread saveThread = new Thread(() => WriteToFile(snapshotMoney, snapshotName, snapshotPercent))
        {
            IsBackground = true
        };
        saveThread.Start();
    }

    private void WriteToFile(int m, string n, int p)
    {
        lock (fileLock) 
        {
            try
            {
                using StreamWriter writer = new StreamWriter(filePath, false);
                writer.WriteLine($"Name: {n}");
                writer.WriteLine($"Money: {m}");
                writer.WriteLine($"Percent: {p}");
                writer.WriteLine($"Updated: {DateTime.Now:HH:mm:ss.fff}");

                Console.WriteLine($"[Потік {Thread.CurrentThread.ManagedThreadId}] Дані збережено у {filePath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Помилка запису у файл: {ex.Message}");
            }
        }
    }
}

class Program
{
    static void Main()
    {
        Bank bank = new Bank("Приватбанк", 1000, 5);

        bank.Money = 1500; 
        bank.Percent = 7;
        bank.Name = "Ощадбанк";

        Console.WriteLine("Головний потік продовжує роботу...");
        Thread.Sleep(500); 
        Console.ReadLine();
    }
}