// © Copyright 2025 Alan Dutton
// SPDX-License-Identifier: MIT

namespace DevMate.Commands;

using System.CommandLine;
using HttpRequestCommands;

public class SendHttpRequestCommand : Command
{
    private const string CommandName = "uuid";
    private const string CommandDescription = "Generates an uuid.";
    
    public SendHttpRequestCommand() 
        : base(CommandName, CommandDescription)
    {
        InitializeSubCommands();
    }

    private void InitializeSubCommands()
    {
        Subcommands.Add(new GetHttpRequestCommand());
    }
}