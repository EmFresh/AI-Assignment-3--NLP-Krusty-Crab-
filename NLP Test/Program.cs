using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Natural_Language_Processing_Test
{
    class Program
    {

        /// <summary>
        /// split text into sentences
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        string[] segmentation(string text)
        {
            return text.Split(new char[] { '.', ',' });
        }

        string[] tokenize(string line)
        {
            return line.Split(' ');
        }

        string[] removeStopwords(string[] tokens)
        {
            List<string> stopWords = new List<string>();
            tokens.ToList().RemoveAll((t) => stopWords.FindIndex((n) => t == n) <= 0);
            return tokens;
        }



        static void Main(string[] args)
        {
        }
    }
}
