
namespace DevMate.Commands.HttpRequestCommands;

using System.CommandLine;

public abstract class BaseHttpRequestCommand : Command
{
    public BaseHttpRequestCommand(string name, string description)
        : base(name, description)
    {
        InitializeOptions();
    }

    private void InitializeOptions()
    {
        Options.Add(new Option<string?>("-b", "JSON payload for POST/PUT"));
        Options.Add(new Option<string[]>("-h", "Custom headers (format: Key: Value)") { Arity = ArgumentArity.ZeroOrMore });
        Options.Add(new Option<string?>("-o", "Write response body to a file"));
        Options.Add(new Option<bool>("--minify", "Output JSON in minified format"));
        Options.Add(new Option<bool>("--status-only", "Only print the HTTP status code"));
    }
}