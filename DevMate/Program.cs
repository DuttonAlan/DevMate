// © Copyright 2025 Alan Dutton
// SPDX-License-Identifier: MIT

namespace DevMate;

using System.CommandLine;
using Commands;

class Program
{
    private const string RootCommandDescription = 
        "DevMate is a modular CLI toolbox for developers with usefull commands for the development.";
    
    static int Main(string[] args)
    {
        var rootCommand = new RootCommand(RootCommandDescription);
        rootCommand.Subcommands.Add(new GenerateUuidCommand());
        rootCommand.Subcommands.Add(new HttpRequestCommand());
        rootCommand.Subcommands.Add(new ConverterCommand());
        
        return rootCommand.Parse(args).Invoke();
    }
}
