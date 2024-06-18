using System;

namespace Security.infrastructure;

public class ConsoleInputReader : InputReader
{
    public string Read(string inputRequestMessage)
    {
        Console.WriteLine(inputRequestMessage);
        return Console.ReadLine();
    }
}