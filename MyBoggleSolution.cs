using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Boggle
{
    public interface IResults
    {
        IEnumerable<string> Words { get; } // unique found words
        int Score { get; } // total score for all words found
    }

    public interface ISolver
    {
        // this func may be called multiple times
        // board: 'q' represents the 'qu' Boggle cube
        IResults FindWords(char[,] board);
    }

    class MyBoggleSolution
    {
        // input dictionary is a file with one word per line
        public static ISolver CreateSolver(string dictionaryPath)
        {
            HashSet<string> words = new HashSet<string>();
            string line;

            // Load our scrabble dictionary into our hash table
            using (StreamReader reader = new StreamReader(dictionaryPath))
            {
                while ((line = reader.ReadLine()) != null)
                {
                    if(!words.Contains(line))
                    {
                        words.Add(line.ToUpper()); // Ensure that all words are upper case for consitency
                    }
                }
            }

            return new Solver(words);
        }
    }
}
