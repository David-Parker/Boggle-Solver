﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    /// <summary>
    /// Implements the neccessary logic for solving all words on a Boggle grid.
    /// </summary>
    public class Solver : ISolver
    {
        /// <summary>
        /// Prefix tree, will allow for fast substring matches on potential matched words
        /// </summary>
        private class TrieNode
        {
            public TrieNode(string elem)
            {
                this.elem = elem;
            }

            public string elem;
            public TrieNode[] children = new TrieNode[26];
        }

        private HashSet<string> words;
        private TrieNode root;

        public Solver(HashSet<string> words)
        {
            if(null == words)
            {
                throw new ArgumentNullException("The words dictionary cannot be null.");
            }

            this.words = words;
            this.root = new TrieNode("");

            // Build the trie tree recursively
            foreach(var word in words)
            {
                BuildTrie(this.root, word, 0);
            }
        }

        public IResults FindWords(char[,] board)
        {
            if(null == board)
            {
                throw new ArgumentNullException("The board cannot be null.");
            }

            Results results = new Results();

            results.SetWords(GetAllWords(board));

            // Score the words
            foreach(var word in results.Words)
            {
                results.AddScore(GetScoreForWord(word));
            }

            return results;
        }

        /// <summary>
        /// Builds a trie tree, word is assumed to be upper case
        /// </summary>
        private void BuildTrie(TrieNode root, string word, int elementIndex)
        {
            if (elementIndex == word.Length)
            {
                return;
            }

            char c = word.ElementAt(elementIndex);
            int index = c - 'A';

            // Element doesn't exist, build it
            if (null == root.children[index])
            {
                TrieNode newNode = new TrieNode(c.ToString());
                root.children[index] = newNode;
            }

            BuildTrie(root.children[index], word, ++elementIndex);
        }

        /// <summary>
        /// Iterates over each character in the board, and returns all the words that can be found starting at these characters
        /// </summary>
        private List<string> GetAllWords(char[,] board)
        {
            HashSet<string> foundWordCache = new HashSet<string>();
            List<string> returnList = new List<string>();

            bool[,] visitedNodes = new bool[board.GetLength(0), board.GetLength(1)]; // Temporary lock on the characters in the board already visited

            for (int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    foreach (var word in GetWord(i,j,board, "", visitedNodes, foundWordCache))
                    {
                        returnList.Add(word);
                    }
                }
            }

            return returnList;
        }

        /// <summary>
        /// Recursive backtracking co-routine method, yields the return for every found word. Words are generated by appending each character in every
        /// direction of the board to a string accumulator. Once an acceptable word is found, it is returned to the caller.
        /// </summary>
        private IEnumerable<string> GetWord(int row, int col, char[,] board, string currWord, bool[,] visitedNodes, HashSet<string> foundWordCache)
        {
            // We visited this node, don't return here again
            visitedNodes[row, col] = true;

            char c = board[row, col];

            currWord = AppendChar(currWord, c);
            
            if(!foundWordCache.Contains(currWord) && IsScoredBoggleWord(currWord))
            {
                foundWordCache.Add(currWord);

                yield return currWord;
            }

            // Try all board directions
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    if (PointIsInBounds(row + i, col + j, board) && !visitedNodes[row + i, col + j])
                    {
                        // Peek the next character, if no word can be created with this new character, don't attempt
                        char nextCharacter = board[row + i, col + j];
                        string newWord = AppendChar(currWord, nextCharacter);

                        bool canMakeWord = CouldMakeWord(newWord.ToUpper());

                        if (canMakeWord)
                        {
                            foreach (var word in GetWord(row + i, col + j, board, currWord, visitedNodes, foundWordCache))
                            {
                                yield return word;
                            }
                        }
                    }
                }
            }

            // Recursive backtrack, remove the node "lock"
            visitedNodes[row, col] = false;
        }

        /// <summary>
        /// Returns true if it still possible to make a word from our dictionary given the input word
        /// </summary>
        private bool CouldMakeWord(string word)
        {
            return CouldMakeWordHelper(root, word, 0);
        }

        /// <summary>
        /// Uses a prefix tree (trie tree) to validate that this word is still a potential candidate to create a new word
        /// </summary>
        private bool CouldMakeWordHelper(TrieNode root, string word, int elementIndex)
        {
            // We made it all the way down the tree with this word
            if(elementIndex == word.Length)
            {
                return true;
            }

            char c = word.ElementAt(elementIndex);
            int index = c - 'A';

            if(null == root.children[index])
            {
                return false;
            }
            else
            {
                return CouldMakeWordHelper(root.children[index], word, ++elementIndex);
            }

        }

        /// <summary>
        /// Ensures that the position specified is withing the boundaries of the board
        /// </summary>
        private bool PointIsInBounds(int row, int col, char[,] board)
        {
            return (row < board.GetLength(0) && row >= 0 && col < board.GetLength(1) && col >= 0);
        }

        /// <summary>
        /// Returns true if the input word is an accepted word of the dictionary as well as all other constraints set by Boggle
        /// </summary>
        private bool IsScoredBoggleWord(string word)
        {
            if(word.Length < 3)
            {
                return false;
            }

            if (!words.Contains(word.ToUpper()))
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Points are defined by the rules of Boggle
        /// </summary>
        private int GetScoreForWord(string word)
        {
            if(word.Length < 3)
            {
                return 0;
            }

            if(word.Length == 3 || word.Length == 4)
            {
                return 1;
            }

            if(word.Length == 5)
            {
                return 2;
            }

            if(word.Length == 6)
            {
                return 3;
            }

            if(word.Length == 7)
            {
                return 5;
            }

            if(word.Length >= 8)
            {
                return 11;
            }

            return 0;
        }

        private string AppendChar(string currWord, char c)
        {
            // 'q' represents 'qu' in Boggle, since q is almost ubiquitously followed by a u
            if (c == 'q')
            {
                currWord += "qu";
            }
            else
            {
                currWord += c;
            }

            return currWord;
        }
    }
}
