// See https://aka.ms/new-console-template for more information

//Console.WriteLine("Hello, World!");

string homePath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

if (homePath != "C:\\Users\\hunte")
{
    Console.WriteLine(@"if (homePath != ""C:\\Users\\hunte"")");
    return;
}

//using StreamWriter writer = new StreamWriter("C:\\Users\\hunte\\Repos\\file.txt");
var myPath = "C:\\Users\\hunte\\Repos\\file.txt";
File.WriteAllText(myPath, string.Empty);

using StreamReader reader = new StreamReader(Console.OpenStandardInput());

while (!reader.EndOfStream) // I'm getting the warning: "Do not use 'reader.EndOfStream' in an async method"
{
    string line = await reader.ReadLineAsync();
    File.AppendAllText(myPath, line);
    File.AppendAllText(myPath, "\n====\n");
    // Process line
}
