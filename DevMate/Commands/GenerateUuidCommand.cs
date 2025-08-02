// © Copyright 2025 Alan Dutton
// SPDX-License-Identifier: MIT

namespace DevMate.Commands;

using System;
using System.CommandLine;

public class GenerateUuidCommand : Command
{
    private const string CommandName = "uuid";
    private const string CommandDescription = "Generates an uuid.";
    
    public GenerateUuidCommand()
        : base(CommandName, CommandDescription)
    {
        SetAction(a => Console.WriteLine(Guid.NewGuid().ToString()));
    }
}