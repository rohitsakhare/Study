using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

public class Program
{
    public static async Task Main(string[] args)
    {
        var ids = new List<int> { 1, 2, 3, 4, 5 };

        Console.WriteLine("Starting Sequential Execution...\n");
        await RunSequential(ids);

        Console.WriteLine("\n---------------------------------\n");

        Console.WriteLine("Starting Parallel Execution...\n");
        await RunParallel(ids);
    }

    // ❌ Sequential: await inside loop
    static async Task RunSequential(List<int> ids)
    {
        var sw = Stopwatch.StartNew();

        foreach (var id in ids)
        {
            var result = await GetDataAsync(id);
            Console.WriteLine($"Processed {result}");
        }

        sw.Stop();
        Console.WriteLine($"\nSequential Total Time: {sw.ElapsedMilliseconds} ms");
    }

    // ✅ Parallel: Task.WhenAll
    static async Task RunParallel(List<int> ids)
    {
        var sw = Stopwatch.StartNew();

        var tasks = ids.Select(async id =>
        {
            var result = await GetDataAsync(id);
            Console.WriteLine($"Processed {result}");
            return result;
        });

        var results = await Task.WhenAll(tasks);

        sw.Stop();
        Console.WriteLine($"\nParallel Total Time: {sw.ElapsedMilliseconds} ms");
    }

    // Simulated slow operation (like API/DB call)
    static async Task<string> GetDataAsync(int id)
    {
        await Task.Delay(1000); // simulate 1 second work
        return $"Data-{id}";
    }
}