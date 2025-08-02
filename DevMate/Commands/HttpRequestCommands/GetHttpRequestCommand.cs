using System.CommandLine;

namespace DevMate.Commands.HttpRequestCommands;

public class GetHttpRequestCommand : BaseHttpRequestCommand
{
    private const string CommandName = "get";
    private const string CommandDescription = "Sends a HttpRequest with the HttpMethod Get.";
    
    public GetHttpRequestCommand() 
        : base(CommandName, CommandDescription)
    {
    }
}
