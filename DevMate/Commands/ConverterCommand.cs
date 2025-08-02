// © Copyright 2025 Alan Dutton
// SPDX-License-Identifier: MIT

namespace DevMate.Commands;

using System;
using System.IO;
using System.Xml;
using System.Text.Json;
using System.CommandLine;
using System.Collections.Generic;

public class ConverterCommand : Command
{
    private const string CommandName = "convert";
    private const string CommandDescription = "Convert data between formats like XML and JSON.";

    private readonly Argument<string> _from = new ("from") { Description = "Source data format." };
    private readonly Argument<string> _to = new ("to") { Description = "Destination data format." };
    
    private readonly Option<string> _input = new ("-i", "--input") { Description = "Input file", Required = true };
    private readonly Option<string> _output = new ("-o", "--output") { Description = "Output file", Required = true };
    private readonly Option<bool> _pretty = new ("-p", "--pretty") { Description = "Output formatted (JSON only)" };
    
    public ConverterCommand()
        : base(CommandName, CommandDescription)
    {
        Arguments.Add(_from);
        Arguments.Add(_to);
        Options.Add(_input);
        Options.Add(_output);
        Options.Add(_pretty);
        SetAction(Convert);
    }

    private void Convert(ParseResult parseResult)
    {
        var from = parseResult.GetValue(_from).ToLower().Trim();
        var to = parseResult.GetValue(_to).ToLower().Trim();
        var input = parseResult.GetValue(_input);
        var output = parseResult.GetValue(_output);
        var pretty = parseResult.GetValue(_pretty);
        
        if (string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to) ||
            string.IsNullOrWhiteSpace(input)) return;

        var fileContent = File.ReadAllText(input);
        
        switch (from)
        {
            case "xml" when to == "json":
                var jsonResult = ConvertXmlToJson(fileContent, pretty);

                if (string.IsNullOrWhiteSpace(jsonResult))
                {
                    Console.WriteLine("Converting xml to json failed.");
                }

                Console.WriteLine(jsonResult);

                if (!string.IsNullOrWhiteSpace(output))
                {
                    File.WriteAllText(output, jsonResult);
                }
                break;
            case "json" when to == "xml":
                var xmlResult = ConvertJsonToXml(fileContent);
                if (string.IsNullOrWhiteSpace(xmlResult))
                {
                    Console.WriteLine("Converting json to xml failed.");
                }

                Console.WriteLine(xmlResult);
                
                if (!string.IsNullOrWhiteSpace(output))
                {
                    File.WriteAllText(output, xmlResult);
                }
                break;
        }
    }

    private string ConvertXmlToJson(string xml, bool pretty)
    {
        var doc = new XmlDocument();
        doc.LoadXml(xml);

        var root = doc.DocumentElement!;
        var result = new Dictionary<string, object>();

        foreach (XmlNode node in root.ChildNodes)
        {
            if (node.NodeType != XmlNodeType.Element) continue;
            var key = node.Name;
            var value = node.InnerText;
            
            if (int.TryParse(value, out var intVal))
                result[key] = intVal;
            else if (double.TryParse(value, out var dblVal))
                result[key] = dblVal;
            else if (bool.TryParse(value, out var boolVal))
                result[key] = boolVal;
            else
                result[key] = value;
        }

        var options = new JsonSerializerOptions();
        options.WriteIndented = pretty;
        return JsonSerializer.Serialize(result, options);
    }

    private string ConvertJsonToXml(string json)
    {
        using var document = JsonDocument.Parse(json);
        var xmlDoc = new XmlDocument();
        var root = xmlDoc.CreateElement("root");
        xmlDoc.AppendChild(root);
        
        AppendJsonToXml(document.RootElement, xmlDoc, root);
        
        return xmlDoc.OuterXml;
    }
    
    private static void AppendJsonToXml(JsonElement json, XmlDocument xmlDoc, XmlElement currentElement)
    {
        switch (json.ValueKind)
        {
            case JsonValueKind.Object:
                foreach (var property in json.EnumerateObject())
                {
                    var child = xmlDoc.CreateElement(property.Name);
                    AppendJsonToXml(property.Value, xmlDoc, child);
                    currentElement.AppendChild(child);
                }
                break;

            case JsonValueKind.Array:
                foreach (var item in json.EnumerateArray())
                {
                    var itemElement = xmlDoc.CreateElement("item");
                    AppendJsonToXml(item, xmlDoc, itemElement);
                    currentElement.AppendChild(itemElement);
                }
                break;

            case JsonValueKind.String:
                currentElement.InnerText = json.GetString()!;
                break;

            case JsonValueKind.Number:
                currentElement.InnerText = json.GetRawText();
                break;

            case JsonValueKind.True:
            case JsonValueKind.False:
                currentElement.InnerText = json.GetBoolean().ToString().ToLower();
                break;

            case JsonValueKind.Null:
                currentElement.IsEmpty = true;
                break;
            case JsonValueKind.Undefined:
                break;
        }
    }
}