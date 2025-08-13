using System;
using System.CommandLine;
using BitNet.Core;

class Program
{
    static void Main(string[] args)
    {
        var modelOption = new Option<string>(
            name: "--model",
            description: "The path to the GGUF model file.");

        var promptOption = new Option<string>(
            name: "--prompt",
            description: "The prompt for the model.");

        var tokenizerOption = new Option<string>(
            name: "--tokenizer-path",
            description: "The path to the tokenizer model.",
            getDefaultValue: () => "tokenizer.model");

        var rootCommand = new RootCommand("BitNet inference runner");
        rootCommand.AddOption(modelOption);
        rootCommand.AddOption(promptOption);
        rootCommand.AddOption(tokenizerOption);

        rootCommand.SetHandler((model, prompt, tokenizer) =>
        {
            var runner = new InferenceRunner(model, tokenizer);
            var result = runner.Run(prompt);
            Console.WriteLine(result);
        },
        modelOption, promptOption, tokenizerOption);

        rootCommand.Invoke(args);
    }
}
