using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileParserConsoleApp
{
    public class FileLine
    {
        public string FileName { get; set; }

        public string DirectoryPath { get; set; }

        public DateTime DateTime { get; set; }

        public string FullPath
        {
            get { return Path.Combine(DirectoryPath, FileName); }
        }
    }
}
