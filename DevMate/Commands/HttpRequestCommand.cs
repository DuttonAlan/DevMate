// © Copyright 2025 Alan Dutton
// SPDX-License-Identifier: MIT

namespace DevMate.Commands;

using System;
using System.IO;
using System.Net.Http;
using System.CommandLine;

public class HttpRequestCommand : Command
{
    private const string CommandName = "http";
    private const string CommandDescription = "Send http request using the commandline.";

    private readonly Argument<string> _method = new ("method") { Description = "The http method." };
    private readonly Argument<string> _uri = new ("uri") { Description = "The request uri" };
    
    private readonly Option<string> _bodyOption = new ("-b", "--body") { Description = "Set request body" };
    private readonly Option<string[]> _headerOption = new ("-h", "--header") { Description = "Set request headers", Arity = ArgumentArity.ZeroOrMore };
    private readonly Option<bool> _statusOnlyOption = new ("--status-only") { Description = "Show http status only" };
    
    public HttpRequestCommand() 
        : base(CommandName, CommandDescription)
    {
        Arguments.Add(_method);
        Arguments.Add(_uri);
        Options.Add(_bodyOption);
        Options.Add(_headerOption);
        Options.Add(_statusOnlyOption);
        SetAction(SendHttpRequest);
    }

    private void SendHttpRequest(ParseResult parseResult)
    {
        var method = MapStringToMethod(parseResult.GetValue(_method));
        var uri = parseResult.GetValue(_uri);
        var body = parseResult.GetValue(_bodyOption);
        var headers = parseResult.GetValue(_headerOption);
        var statusOnly = parseResult.GetValue(_statusOnlyOption);
        
        var client = new HttpClient();
        var request = new HttpRequestMessage(method, uri);
        
        if (!string.IsNullOrEmpty(body))
        {
            request.Content = new StringContent(body);
        }
        
        if (headers is { Length: > 0 })
        {
            foreach (var header in headers)
            {
                var values = header.Split(':');
                request.Headers.Add(values[0].Trim(), values[1].Trim());
            }
        }
        
        var response = client.Send(request);

        Console.WriteLine(response.IsSuccessStatusCode
            ? "The HTTP request was sent successfully."
            : "The HTTP request was not sent successfully.");

        Console.WriteLine($"StatusCode: {response.StatusCode} {(int)response.StatusCode}");

        if (statusOnly) return;
        
        using var contentAsStream = response.Content.ReadAsStream();
        using var streamReader = new StreamReader(contentAsStream);
        var content = streamReader.ReadToEnd();

        if (!string.IsNullOrEmpty(content))
        {
            Console.WriteLine($"Payload: {content}");
        }
    }

    private HttpMethod MapStringToMethod(string method) => method.ToLower().Trim() switch
    {
        "get" => HttpMethod.Get,
        "post" => HttpMethod.Post,
        "put" => HttpMethod.Put,
        "patch" => HttpMethod.Patch,
        "delete" => HttpMethod.Delete,
        "head" => HttpMethod.Head,
        "options" => HttpMethod.Options,
        "connect" => HttpMethod.Connect,
        "trace" => HttpMethod.Trace,
        _ => throw new ArgumentOutOfRangeException()
    };
}