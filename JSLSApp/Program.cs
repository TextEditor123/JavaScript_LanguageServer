// See https://aka.ms/new-console-template for more information
//Console.WriteLine("Hello, World!");

using (StreamReader reader = new StreamReader(Console.OpenStandardInput()))
{
    // I'm getting the warning: "Do not use 'reader.EndOfStream' in an async method"
    while (!reader.EndOfStream)
    {
        string line = await reader.ReadLineAsync();
        // Process line
    }
}
