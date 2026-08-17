using System.Collections.Generic;
using ServerCli.Commands;


string? command;

do
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("server");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write(":");
    Console.ForegroundColor = ConsoleColor.Blue;
    Console.Write("~");
    Console.ForegroundColor = ConsoleColor.Gray;
    Console.Write("$ ");


    command = Console.ReadLine();
    if (!string.IsNullOrEmpty(command))
    {
        var result = await CommandHandler.Execute(command.Split(' '));
        Console.WriteLine(result);
    }

    if (command == "clear")
        Console.Clear();


}
while (command != "exit");