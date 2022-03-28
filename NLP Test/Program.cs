using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Natural_Language_Processing_Test
{

    /// <summary>
    /// By the original author:
    ///		Stemmer, implementing the Porter Stemming Algorithm
    /// 
    ///		The Stemmer class transforms a word into its root form.  The input
    ///		word can be provided a character at time (by calling add()), or at once
    ///		by calling one of the various stem(something) methods.
    /// </summary>
    /// <remarks>
    /// source, licence, etc for this class available here:
    /// http://www.tartarus.org/martin/PorterStemmer/
    /// http://www.tartarus.org/martin/PorterStemmer/csharp4.txt
    /// </remarks>
    public class PorterStemmer
    {
        private char[] wordArray;       // character array copy of the given string
        private int stem, end;          // indices to the current end (last letter) of the stem and the word in the array

        // Get the stem of a word at least three letters long:
        public string StemWord(string word)
        {
            if(string.IsNullOrWhiteSpace(word) || word.Length <= 3)
                return word;

            wordArray = word.ToArray();
            stem = 0;
            end = word.Length - 1;

            Step1();
            Step2();
            Step3();
            Step4();
            Step5();
            //   Step6();

            return new string(wordArray, 0, end + 1);
        }

        // Step 1: remove basic plurals and -ed/-ing:
        private void Step1()
        {
            if(wordArray[end] == 's')
            {
                if(EndsWith("sses"))
                    Truncate(2);
                else if(EndsWith("ies"))
                    OverwriteEnding("i");
                else if(wordArray[end - 1] != 's')
                    Truncate();
            }

            if(EndsWith("eed"))
            {
                if(ConsonantSequenceCount() > 0)
                    Truncate();
            }
            else if((EndsWith("ed") || EndsWith("ing")) && VowelInStem())
            {
                end = stem;
                if(EndsWith("at"))
                    OverwriteEnding("ate");
                else if(EndsWith("bl"))
                    OverwriteEnding("ble");
                else if(EndsWith("iz"))
                    OverwriteEnding("ize");
                else if(EndsWithDoubleConsonant())
                {
                    if(!"lsz".Contains(wordArray[end - 1]))
                        Truncate();
                }
                else if(ConsonantSequenceCount() == 1 && PrecededByCVC(end))
                    OverwriteEnding("e");
            }
        }

        // Step 2: change a terminal 'y' to 'i' if there is another vowel in the stem:
        private void Step2()
        {
            if(EndsWith("y") && VowelInStem())
                OverwriteEnding("i");
        }

        // Step 3: fold double suffixes to single suffix, e.g., -ization = -ize + -ation -> -ize:
        private void Step3()
        {
            switch(wordArray[end - 1])
            {
            case 'a':
                if(ReplaceEnding("ational", "ate")) break;
                ReplaceEnding("tional", "tion"); break;
            case 'c':
                if(ReplaceEnding("enci", "ence")) break;
                ReplaceEnding("anci", "ance"); break;
            case 'e':
                ReplaceEnding("izer", "ize"); break;
            case 'l':
                if(ReplaceEnding("bli", "ble")) break;
                if(ReplaceEnding("alli", "al")) break;
                if(ReplaceEnding("entli", "ent")) break;
                if(ReplaceEnding("eli", "e")) break;
                ReplaceEnding("ousli", "ous"); break;
            case 'o':
                if(ReplaceEnding("ization", "ize")) break;
                if(ReplaceEnding("ation", "ate")) break;
                ReplaceEnding("ator", "ate"); break;
            case 's':
                if(ReplaceEnding("alism", "al")) break;
                if(ReplaceEnding("iveness", "ive")) break;
                if(ReplaceEnding("fulness", "ful")) break;
                ReplaceEnding("ousness", "ous"); break;
            case 't':
                if(ReplaceEnding("aliti", "al")) break;
                if(ReplaceEnding("iviti", "ive")) break;
                ReplaceEnding("biliti", "ble"); break;
            case 'g':
                ReplaceEnding("logi", "log"); break;
            }
        }

        // Step 4: replace -ic-, -full, -ness, etc. with simpler endings:
        private void Step4()
        {
            switch(wordArray[end])
            {
            case 'e':
                if(ReplaceEnding("icate", "ic")) break;
                if(ReplaceEnding("ative", "")) break;
                ReplaceEnding("alize", "al"); break;
            case 'i':
                ReplaceEnding("iciti", "ic"); break;
            case 'l':
                if(ReplaceEnding("ical", "ic")) break;
                ReplaceEnding("ful", ""); break;
            case 's':
                ReplaceEnding("ness", ""); break;
            }
        }

        // Step 5: remove -ant, -ence, etc.:
        private void Step5()
        {
            switch(wordArray[end - 1])
            {
            case 'a':
                if(EndsWith("al")) break; return;
            case 'c':
                if(EndsWith("ance")) break;
                if(EndsWith("ence")) break; return;
            case 'e':
                if(EndsWith("er")) break; return;
            case 'i':
                if(EndsWith("ic")) break; return;
            case 'l':
                if(EndsWith("able")) break;
                if(EndsWith("ible")) break; return;
            case 'n':
                if(EndsWith("ant")) break;
                if(EndsWith("ement")) break;
                if(EndsWith("ment")) break;
                if(EndsWith("ent")) break; return;
            case 'o':
                if(EndsWith("ion") && stem >= 0 && (wordArray[stem] == 's' || wordArray[stem] == 't')) break;
                if(EndsWith("ou")) break; return;
            case 's':
                if(EndsWith("ism")) break; return;
            case 't':
                if(EndsWith("ate")) break;
                if(EndsWith("iti")) break; return;
            case 'u':
                if(EndsWith("ous")) break; return;
            case 'v':
                if(EndsWith("ive")) break; return;
            case 'z':
                if(EndsWith("ize")) break; return;
            default:
                return;
            }

            if(ConsonantSequenceCount() > 1)
                end = stem;
        }

        // Step 6: remove final 'e' if necessary:
        private void Step6()
        {
            stem = end;
            if(wordArray[end] == 'e')
            {
                var m = ConsonantSequenceCount();
                if(m > 1 || m == 1 && !PrecededByCVC(end - 1))
                    Truncate();
            }

            if(wordArray[end] == 'l' && EndsWithDoubleConsonant() && ConsonantSequenceCount() > 1)
                Truncate();
        }

        private void Truncate(int n = 1)
        {
            end -= n;
        }

        // Count the number of CVC sequences:
        private int ConsonantSequenceCount()
        {
            int m = 0, index = 0;
            for(; index <= stem && IsConsonant(index); index++) ;
            if(index > stem)
                return 0;

            for(index++; ; index++)
            {
                for(; index <= stem && !IsConsonant(index); index++) ;
                if(index > stem)
                    return m;

                for(index++, m++; index <= stem && IsConsonant(index); index++) ;
                if(index > stem)
                    return m;
            }
        }

        // Return true if there is a vowel in the current stem:
        private bool VowelInStem()
        {
            for(var i = 0; i <= stem; i++)
                if(!IsConsonant(i))
                    return true;
            return false;
        }

        // Returns true if the character at the specified index is a consonant, with special handling for 'y':
        private bool IsConsonant(int index)
        {
            if("aeiou".Contains(wordArray[index]))
                return false;

            return wordArray[index] != 'y' || index == 0 || !IsConsonant(index - 1);
        }

        // Return true if the char. at the current index and the one preceeding it are the same consonant:
        private bool EndsWithDoubleConsonant()
        {
            return end > 0 && wordArray[end] == wordArray[end - 1] && IsConsonant(end);
        }

        // Check if the letters at i-2, i-1, i have the pattern: consonant-vowel-consonant (CVC) and the second consonant
        // is not w, x or y; used when restoring an 'e' at the end of a short word, e.g., cav(e), lov(e), hop(e), etc.:
        private bool PrecededByCVC(int index)
        {
            if(index < 2 || !IsConsonant(index) || IsConsonant(index - 1) || !IsConsonant(index - 2))
                return false;

            return !"wxy".Contains(wordArray[index]);
        }

        // Check if the given string appears at the end of the word:
        private bool EndsWith(string s)
        {
            int length = s.Length, index = end - length + 1;
            if(index >= 0)
            {
                for(var i = 0; i < length; i++)
                    if(wordArray[index + i] != s[i])
                        return false;

                stem = end - length;
                return true;
            }
            return false;
        }

        // Conditionally replace the end of the word:
        private bool ReplaceEnding(string suffix, string s)
        {
            if(EndsWith(suffix) && ConsonantSequenceCount() > 0)
            {
                OverwriteEnding(s);
                return true;
            }
            return false;
        }

        // Change the end of the word to a given string:
        private void OverwriteEnding(string s)
        {
            int length = s.Length, index = stem + 1;
            for(var i = 0; i < length; i++)
                wordArray[index + i] = s[i];
            end = stem + length;
        }
    }


    class Program
    {

        /// <summary>
        /// split text into sentences
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        static string[] segmentation(string text)
        {
            return text.Split(new string[] { " ", ".", ",", "\n", "\t" }, StringSplitOptions.RemoveEmptyEntries);
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
            var lemDic = File.ReadAllLines("stop_words_english.txt");
            foreach(string s1 in lemDic)
                stopWords.Add(s1);

            List<string> noRemove = new List<string> {
                "not",
                "don't",
                "away",
                "Yes       ",
                "Yea       ",
                "Yeah      ",
                "Yep       ",
                "Sure      ",
                "Great     ",
                "No        ",
                "Not today ",
                "Nah       ",
                "Nope      ",
                "Add       ",
                "And       ",
                "Also      ",
                "Plus      ",
                "Have      ",
                "Order     ",
                "Remove    ",
                "Delete    ",
                "Subtract  ",
                "Minus     ",
                "Erase     ",
                "No        ",

            };

            stopWords.RemoveAll(k => noRemove.FindIndex(l => k.Trim().ToLower() == l.Trim().ToLower()) >= 0);

            var list = tokens.ToList();

            list.RemoveAll(t => stopWords.FindIndex(s => t.Trim().ToLower() == s.Trim().ToLower()) >= 0);
            return list.ToArray();
        }//done 




        /// <summary>
        /// get the root word of the given words
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        static string[] stemWords(string[] tokens)
        {
            var list = tokens.ToList();
            List<string> stemWords = new List<string>();
            int index = -1;
            List<string> noRemove = new List<string> {
                "not",
                "don't",
                "away",
                "Yes       ",
                "Yea       ",
                "Yeah      ",
                "Yep       ",
                "Sure      ",
                "Great     ",
                "No        ",
                "Not today ",
                "Nah       ",
                "Nope      ",
                "Add       ",
                "And       ",
                "Also      ",
                "Plus      ",
                "Have      ",
                "Order     ",
                "Remove    ",
                "Delete    ",
                "Subtract  ",
                "Minus     ",
                "Erase     ",
                "No        ",

            };



            foreach(var stem in stemWords)
                if((index = list.FindIndex((k) => k.ToLower().Contains(stem.ToLower()))) >= 0)
                    list[index] = stem;

            for(index = 0; index < list.Count; ++index)
                if(noRemove.FindIndex(t => t.Trim().ToLower() == list[index].Trim().ToLower()) < 0)
                    list[index] = prefixStrip(suffixStrip(list[index]));

            return list.ToArray();
        }//done 

        static string suffixStrip(string word)
        {
            ////ed
            //if(word.Length - 3 >= 0 && word.Substring(word.Length - 3/*2 letters*/) == "ed")
            //    word = word.Substring(0, word.Length - 3);
            ////ly
            //if(word.Length - 3 >= 0 && word.Substring(word.Length - 3/*2 letters*/) == "ly")
            //    word = word.Substring(0, word.Length - 3);
            ////ing
            //if(word.Length - 4 >= 0 && word.Substring(word.Length - 4/*3 letters*/) == "ing")
            //    word = word.Substring(0, word.Length - 4);

            PorterStemmer stemmer = new PorterStemmer();


            return stemmer.StemWord(word);
        }

        static string prefixStrip(string word)
        {
            ////pre
            //if(word.Length >= 4 && word.Substring(0, 3) == "pre" && (word[3] != 'p'&&))
            //    word = word.Substring(3);



            return word;
        }




        /// <summary>
        /// gets the base word tenses  (i.e. (am, are, is) -> be)
        /// </summary>
        /// <returns></returns>
        static string[] lemmanize(string[] words)
        {
            Dictionary<string, string> lemmanizeWords = new Dictionary<string, string>();
            //this is where I got the list: https://github.com/michmech/lemmatization-lists
            var lemDic = File.ReadAllLines("lemmatization-en.txt");
            foreach(string lem in lemDic)
            {
                string s1 = lem.Substring(0, lem.LastIndexOf('\t')).Trim().ToLower(), s2 = lem.Substring(lem.LastIndexOf('\t') + 1).Trim().ToLower();

                lemmanizeWords[s2] = s1;
            }


            var list = words.ToList();
            int index = 0;
            string lemm;
            foreach(var word in words)
                try
                {
                    if(((lemm = lemmanizeWords[word.ToLower()]) != null ? lemm.Length : 0) > 0)
                        list[index++] = lemm;
                }
                catch { }

            return list.ToArray();
        }//testing

        /// <summary>
        /// checks for tag for each word etc. 
        /// </summary>
        /// <returns></returns>
        static List<(string, string[])> speachTag(string[] words)
        {
            List<(string, string)> wordTags = new List<(string, string)>();

            var speachTags = File.ReadAllLines("word_tags.txt");
            string tag = "";
            foreach(string s1 in speachTags)
            {
                if(s1.Contains(':'))
                {
                    tag = s1.Substring(0, s1.IndexOf(':'));
                    continue;
                }
                wordTags.Add((s1.Trim().ToLower(), tag));
            }
            List<(string, string[])> result = new List<(string, string[])>();
            foreach(string s1 in words)
            {
                List<string> tmp = new List<string>();
                foreach(var wtag in wordTags)
                    if(wtag.Item1 == s1.Trim().ToLower())
                        tmp.Add(wtag.Item2);

                result.Add((s1, tmp.ToArray()));
            }
            return result;
        }//wip

        /// <summary>
        /// not sure if I'll need this
        /// </summary>
        static void addExtendedReferenceTags()
        { }


        public interface ISentenceCheck
        {
            /// <summary>
            /// returns probability this is a matching sentence using Naive Bayes Algo.
            /// </summary>
            /// <param name="sentence"></param>
            /// <returns></returns>
            float Check(List<(string, string[])> sentence, float prob);
            float probability { get; }
            Dictionary<string, float> probabilityWeights { get; }


        }

        /// <summary>
        /// weather or not it's a food order
        /// </summary>
        public class Ordering : ISentenceCheck
        {
            public float probability => myProb;

            public Dictionary<string, float> probabilityWeights => new Dictionary<string, float> {
                {"food",0.9f },
                {"accept",0.0f },
                {"deny",0.0f },
                {"add",0.1f },
                {"remove",0.0f },

            };
            float myProb;
            public Ordering(List<(string, string[])> sentence, float prob = 1)
            {
                myProb = Check(sentence, prob);
            }
            ///<inheritdoc/>
            public float Check(List<(string, string[])> sentence, float prob)
            {
                float val = prob;

                const float bias = .001f;
                foreach(var weight in probabilityWeights)
                {
                    float times = sentence.FindAll(t => Array.FindIndex(t.Item2, u => u.ToLower() == weight.Key) >= 0).Count;
                    val *= weight.Value * times + bias;
                }
                return val;
            }
        }

        /// <summary>
        /// Are you canceling the order 
        /// </summary>
        public class CancelOrder : ISentenceCheck
        {

            public float probability => myProb;

            public Dictionary<string, float> probabilityWeights => new Dictionary<string, float> {
                {"food",0.4f },
                {"accept",0.0f },
                {"deny",0.6f },
                {"add",0 },
                {"remove",0 },
            };

            float myProb;
            public CancelOrder(List<(string, string[])> sentence, float prob = 1)
            {
                myProb = Check(sentence, prob);
            }
            ///<inheritdoc/>
            public float Check(List<(string, string[])> sentence, float prob)
            {
                float val = prob;

                const float bias = .001f;
                foreach(var weight in probabilityWeights)
                {
                    float times = sentence.FindAll(t => Array.FindIndex(t.Item2, u => u.ToLower() == weight.Key) >= 0).Count;
                    val *= weight.Value * times + bias;
                }
                return val;
            }
        }

        public class AddToOrder : ISentenceCheck
        {
            public float probability => myProb;

            public Dictionary<string, float> probabilityWeights => new Dictionary<string, float> {
                {"food",0.7f },
                {"accept",0.0f },
                {"deny",0.0f },
                {"add",0.3f },
                {"remove",0.0f},
            };

            float myProb;
            public AddToOrder(List<(string, string[])> sentence, float prob = 1)
            {
                myProb = Check(sentence, prob);
            }

            ///<inheritdoc/>
            public float Check(List<(string, string[])> sentence, float prob)
            {
                float val = prob;

                const float bias = .001f;
                foreach(var weight in probabilityWeights)
                {
                    float times = sentence.FindAll(t => Array.FindIndex(t.Item2, u => u.ToLower() == weight.Key) >= 0).Count;
                    val *= weight.Value * times + bias;
                }
                return val;
            }
        }

        public class RemoveFromOrder : ISentenceCheck
        {
            public float probability => myProb;

            public Dictionary<string, float> probabilityWeights => new Dictionary<string, float> {
                {"food",0.7f },
                {"accept",0.0f },
                {"deny",0.0f },
                {"add",0.0f },
                {"remove",0.3f},
            };
            float myProb;
            public RemoveFromOrder(List<(string, string[])> sentence, float prob = 1)
            {
                myProb = Check(sentence, prob);
            }

            ///<inheritdoc/>
            public float Check(List<(string, string[])> sentence, float prob)
            {
                float val = prob;

                const float bias = .001f;
                foreach(var weight in probabilityWeights)
                {
                    float times = sentence.FindAll(t => Array.FindIndex(t.Item2, u => u.ToLower() == weight.Key) >= 0).Count;
                    val *= weight.Value * times + bias;
                }
                return val;
            }
        }

        public class ConfirmOrder : ISentenceCheck
        {
            public float probability => myProb;
            float myProb;
            public Dictionary<string, float> probabilityWeights => new Dictionary<string, float> {
                {"food",0.0f },
                {"accept",1.0f },
                {"deny",0.0f },
                {"add",0.0f },
                {"remove",0.0f},
            };

            public ConfirmOrder(List<(string, string[])> sentence, float prob = 1)
            {
                myProb = Check(sentence, prob);
            }
            public float Check(List<(string, string[])> sentence, float prob)
            {
                float val = prob;

                const float bias = .001f;
                foreach(var weight in probabilityWeights)
                {
                    float times = sentence.FindAll(t => Array.FindIndex(t.Item2, u => u.ToLower() == weight.Key) >= 0).Count;
                    val *= weight.Value * times + bias;
                }
                return val;
            }
        }

        enum AIState
        {
            Greeting,
            TakingOrder,
            ConfirmingOrder,
            Goodbye
        }

        AIState krustyKrabAI = AIState.Greeting;
        static void Main(string[] args)
        {


            //First we segment the string in separate the string sections
            var segments = segmentation("take away the burger");

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

            //consolidate works with the same meaning
            for(int a = 0; a < tokenSentenceList.Count; ++a)
                tokenSentenceList[a] = lemmanize(tokenSentenceList[a]);

            List<List<(string, string[])>> tagedWords = new List<List<(string, string[])>>();
            for(int a = 0; a < tokenSentenceList.Count; ++a)
                tagedWords.Add(speachTag(tokenSentenceList[a]));

            float p1=0, p2=0;
            for(int a = 0; a < tagedWords.Count; ++a)
            {
                ISentenceCheck tmp = new Ordering(tagedWords[a]);

                p1 = tmp.probability;
                 tmp = new RemoveFromOrder(tagedWords[a]);
                p2 = tmp.probability;
            }

            //print result
            int count = 0;
            foreach(var sentence in tagedWords)
            {
                foreach(var word in sentence)
                {
                    Console.Write($"[{count++}]{word.Item1}");
                    foreach(var item in word.Item2)
                        Console.Write($"({item}), ");
                }
            }
                Console.WriteLine("");
                Console.WriteLine($"probability 1: {p1}");
                Console.WriteLine($"probability 2: {p2}");

            Console.ReadKey();
        }
    }
}
