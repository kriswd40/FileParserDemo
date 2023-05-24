
using FileParserConsoleApp;

public class Program
{
    static void Main(string[] args)
    {
        string commonFileLocation = @"V:\Git-Source\FileParserDemo\common.txt"; // TODO - Don't hardcode this!
        DateTime startDate = DateTime.Parse("12/15/2003"); // TODO - Don't hardcode this!

        FileParser parser = new FileParser();
        parser.DoParse(commonFileLocation, startDate);
    }
}
