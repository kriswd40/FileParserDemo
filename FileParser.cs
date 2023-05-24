using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FileParserConsoleApp
{
    public class FileParser
    {
        public enum LineType
        {
            NA, // Lines we don't care about like blank lines and directory lines. 
            Header,
            Footer,
            FileLine
        }

        private string lastDirectoryPath { get; set; }

        public FileParser()
        {
            this.lastDirectoryPath = String.Empty;
        }

        /// <summary>
        /// Parse a given file and return a result.
        /// </summary>
        public void DoParse(string fileLocation, DateTime startDate)
        {
            List<FileLine> fileLines = new List<FileLine>();
            int lineCount = 0;
            var lines = File.ReadLines(fileLocation).ToList();

            foreach (var line in lines)
            {
                string cleansedLine = this.CleanseLine(line);

                // Skip first two lines and lines at end. Could be handled more eligantly.  
                if (lineCount > 2 && lineCount < lines.Count() - 4)
                {
                    var lineType = this.GetLineType(cleansedLine);

                    switch(lineType)
                    {
                        case LineType.Header:
                            this.lastDirectoryPath = this.GetDirectoryFromLine(cleansedLine); 
                            break;

                        case LineType.FileLine:

                            var fileLine = this.LineToFileLine(cleansedLine);

                            // Only add files greater than the start date specified.  
                            if (fileLine.DateTime > startDate)
                            {
                                fileLines.Add(fileLine);
                            }

                            break;
                    }
                }

                lineCount++;
            }

            foreach(var fileLine in fileLines)
            {
                Console.WriteLine(fileLine.FullPath);
            }

            Console.WriteLine("Parsing file completed.");
        }

        /// <summary>
        /// Determine which of the line types we are currently working with. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private LineType GetLineType(string line)
        {
            // TODO - Would be more robust to handle these determinations via regexes, could cause problems if files or directories are named like the terms we're checking against. 

            // Blank line.
            if (string.IsNullOrEmpty(line))
            {
                return LineType.NA;
            }
            // Heading
            else if (line.StartsWith("Directory"))
            {
                return LineType.Header;
            }
            // Footer
            else if (line.Contains("File(s)"))
            {
                return LineType.NA;
            }
            // Directory
            else if (line.Contains("<DIR>"))
            {
                return LineType.NA;
            }

            // TODO - Could use more robust error checking here but for now assume everything left is a file. 
            return LineType.FileLine;
        }

        private string GetDirectoryFromLine(string line)
        {
            var lineParts = line.Split(' ');

            // Everything after the "Directorty Of" is filename. Could include spaces. 
            // A more robust solution could use regexes to parse this info. 
            return String.Join("", lineParts.Skip(2)); 
        }

        /// <summary>
        /// Map a line that belongs to a file to a fileLine object. 
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private FileLine LineToFileLine(string line)
        {
            var lineParts = line.Split(' ');
            var fileDate = DateTime.Parse(lineParts[0] + " " + lineParts[1] + " " + lineParts[2]); // Date, time, and AM/PM are split out.

            FileLine fileLine = new FileLine();

            // Everything after the file size is the filename. Could include spaces. 
            // A more robust solution could use regexes to parse this info. 
            fileLine.FileName = String.Join("", lineParts.Skip(4)); 
            fileLine.DateTime = fileDate;
            fileLine.DirectoryPath = this.lastDirectoryPath; // Setting of directory path always proceeds any file entries. 

            return fileLine;
        }

        /// <summary>
        /// Clean up line before we parse it.
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        private string CleanseLine(string line)
        {
            // Removing leading and trailing spaces. 
            line = line.Trim();

            // Combine multiple spaces into one. 
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex("[ ]{2,}", options);
            line = regex.Replace(line, " ");

            return line;
        }
    }
}
