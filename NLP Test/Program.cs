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

        /// <summary>
        /// split into words
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        string[] tokenize(string line)
        {
            return line.Split(' ');
        }

        /// <summary>
        /// remove unnecessary options (and empty strings)
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        string[] removeStopwords(string[] tokens)
        {
            List<string> stopWords = new List<string>();

            var list = tokens.ToList();
            list.RemoveAll(t => t == " ");
            list.RemoveAll(t => t == "");
            list.RemoveAll((t) => stopWords.FindIndex((n) => t == n) <= 0);
            return list.ToArray();
        }

        /// <summary>
        /// get the root word of the given words
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        string[] stemWords(string[] tokens)
        {

            return null;
        }

        /// <summary>
        /// gets the base word tenses  (i.e. (am, are, is) -> be)
        /// </summary>
        /// <returns></returns>
        string[] lemmanize()
        {

            return null;
        }

        /// <summary>
        /// gets if word is noun, verb etc. 
        /// </summary>
        /// <returns></returns>
        string[] speachTag()
        {

            return null;
        }

        /// <summary>
        /// not sure if I'll need this
        /// </summary>
        void addExtendedReferences()
        {

        }
        static void Main(string[] args)
        {
        }
    }
}
