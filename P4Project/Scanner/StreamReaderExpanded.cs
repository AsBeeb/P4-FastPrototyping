using System.IO;

namespace ScannerLib
{
    public class StreamReaderExpanded : StreamReader
    {
        public StreamReaderExpanded(string inputFile) : base (inputFile)
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
