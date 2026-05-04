namespace ConsoleApp1;

using System;

public class Example
{
    // const: Compile-time constant, cannot be changed once set
    public const double Pi = 3.14159;

    // readonly: Runtime constant, can only be set in constructor or during declaration
    public readonly int YearOfBirth;

    // static: Belongs to the class itself, shared across all instances
    public static int InstanceCount = 0;

    public Example(int year)
    {
        YearOfBirth = year; // Can set readonly during construction
        InstanceCount++;     // Increment static variable
    }

    public void Display()
    {
        Console.WriteLine($"Pi: {Pi}");                     // const is accessible
        Console.WriteLine($"Year of Birth: {YearOfBirth}");  // readonly can vary per instance
        Console.WriteLine($"Instance Count: {InstanceCount}"); // static is shared
    }
}

public class Program
{
    public static void Main()
    {
        // Create instances of Example
        var example1 = new Example(1990);
        var example2 = new Example(1985);

        // Display information for both instances
        example1.Display();
        example2.Display();

        // Access static variable directly from class
        Console.WriteLine($"Total Instances Created: {Example.InstanceCount}");

        // Trying to change const will result in an error
        // Example.Pi = 3.14; // Error: Cannot assign to 'Pi' because it is a 'const'

        // Trying to change readonly will result in an error (outside constructor)
        // example1.YearOfBirth = 2000; // Error: Cannot assign to 'YearOfBirth' because it is readonly
    }
}