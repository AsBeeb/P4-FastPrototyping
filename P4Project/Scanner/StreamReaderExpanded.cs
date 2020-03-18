using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace ScannerLib
{
    public class StreamReaderExpanded : StreamReader
    {
        public StreamReaderExpanded(string InputFile) : base (InputFile)
        {
        }
        public char ReadChar()
        {
            return (char)Read();
        }
        public char PeekChar()
        {
            return (char)Peek();
        }
    }
}
