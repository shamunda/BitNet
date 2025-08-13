using System;
using System.CommandLine;
using BitNet.Core;

class Program
{
    static void Main(string[] args)
    {
        var modelOption = new Option<string>(
            name: "--model",
            description: "The path to the GGUF model file."
        );

        var promptOption = new Option<string>(
            name: "--prompt",
            description: "The prompt to generate text from."
        );

        var rootCommand = new RootCommand("A .NET implementation of BitNet");
        rootCommand.AddOption(modelOption);
        rootCommand.AddOption(promptOption);

        rootCommand.SetHandler((model, prompt) =>
        {
            var reader = new GgufReader(model);
            reader.Read();

            var runner = new InferenceRunner(reader);
            var result = runner.Generate(prompt);

            Console.WriteLine(result);
        }, modelOption, promptOption);

        rootCommand.Invoke(args);
    }
}
