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
        static string[] segmentation(string text)
        {
            return text.Split(new char[] { '.' });
        }//done

        /// <summary>
        /// split into words
        /// </summary>
        /// <param name="line"></param>
        /// <returns></returns>
        static string[] tokenize(string line)
        {
            return line.Split(new char[] { ' ', ',' });
        }//done

        /// <summary>
        /// remove unnecessary options (and empty strings)
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        static string[] removeStopwords(string[] tokens)
        {
            List<string> stopWords = new List<string>() { ",", "", "\n" };

            var list = tokens.ToList();

            list.RemoveAll(t => stopWords.FindIndex(s => t == s) >= 0);
            return list.ToArray();
        }//done (needs extra data)

        /// <summary>
        /// get the root word of the given words
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        static string[] stemWords(string[] tokens)
        {
            var list = tokens.ToList();
            List<string> stemWords = new List<string>();

            foreach(var stem in stemWords)
            {
                int index = 0;
                if((index = list.FindIndex((k) => k.Contains(stem))) >= 0)
                    list[index] = stem;
            }
            return list.ToArray();
        }//testing

        /// <summary>
        /// gets the base word tenses  (i.e. (am, are, is) -> be)
        /// </summary>
        /// <returns></returns>
        static string[] lemmanize()
        {

            return null;
        }//wip

        /// <summary>
        /// gets if word is noun, verb etc. 
        /// </summary>
        /// <returns></returns>
        static string[] speachTag()
        {

            return null;
        }//wip

        /// <summary>
        /// not sure if I'll need this
        /// </summary>
        static void addExtendedReferences()
        {

        }


        static void Main(string[] args)
        {
            //First we segment the string in separate the string sections
            var segments = segmentation("this is a line of text. it should have 2 segments, and 14 words");

            //then get each word per section
            List<string[]> tokenSentenceList = new List<string[]>();
            foreach(var a in segments)
                tokenSentenceList.Add(tokenize(a));

            //remove any word that is unnecessary
            for(int a = 0; a < tokenSentenceList.Count; ++a)
                tokenSentenceList[a] = removeStopwords(tokenSentenceList[a]);


             //we then find the base words if they can be found
             for(int a = 0; a < tokenSentenceList.Count; ++a)
                 tokenSentenceList[a] = stemWords(tokenSentenceList[a]);

            int count = 0;
            foreach(var sentence in tokenSentenceList)
            {
                foreach(var word in sentence)
                    Console.Write($"[{count++}]{word} ");

                Console.WriteLine("");
            }

            Console.ReadKey();
        }
    }
}
