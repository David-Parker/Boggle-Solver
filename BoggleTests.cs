using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Diagnostics;

namespace Boggle
{
    /// <summary>
    /// This is just a simple and quick unit test class for my Boggle game
    /// </summary>
    public class BoggleTests
    {
        private static List<Exception> failedTests = null;

        public static void StartTests()
        {
            // All private static methods are assumed to be tests methods, all of them are invoked
            Type myType = (typeof(BoggleTests));
            MethodInfo[] myArrayMethodInfo = myType.GetMethods(BindingFlags.NonPublic | BindingFlags.Static);

            foreach(var method in myArrayMethodInfo)
            {
                method.Invoke(null, null);
            }

            if(failedTests != null)
            {
                Console.ForegroundColor = ConsoleColor.Red;

                foreach (var test in failedTests)
                {
                    Console.WriteLine(test.Message);
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("All tests passed!");
            }

            Console.ForegroundColor = ConsoleColor.White;
        }

        /* Test Methods */
        private static void EmptyBoard()
        {
            char[,] board =
            {
                { }
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 0);
        }

        private static void SmallBoard()
        {
            char[,] board =
            {
                { 'a', 'b' },
                { 'c', 'e'}
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 2);
        }

        private static void UnityBoard()
        {
            char[,] board =
            {
                { 'd','z','x' },
                { 'e','a','i' },
                { 'q','u','t' }
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 25);
        }

        private static void NonSquare()
        {
            char[,] board =
            {
                { 'd','z','x' },
                { 'e','a','i' }
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 6);
        }

        private static void RandomBoardSmall()
        {
            char[,] board =
            {
                {'a','e','q','i'},
                {'h','e','f','h'},
                {'d','y','n','s'},
                {'q','x','d','y'},
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 43);
        }

        private static void RandomBoardMedium()
        {
            char[,] board =
            {
                {'a','e','q','i','d','g'},
                {'h','e','f','h','y','l'},
                {'d','y','n','s','m','s'},
                {'q','x','d','y','m','f'},
                {'k','a','q','c','d','h'},
                {'n','k','e','q','v','e'}
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 71);
        }

        private static void RandomBoardLarge()
        {
            char[,] board =
            {
                {'a','e','q','i','d','g','h','z'},
                {'h','e','f','h','y','l','o','x'},
                {'d','y','n','s','m','s','c','m'},
                {'q','x','d','y','m','f','a','f'},
                {'k','a','q','c','d','h','u','o'},
                {'n','k','e','q','v','e','w','d'},
                {'a','j','o','w','v','g','z','t'}
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 177);
        }

        private static void UltimateTest()
        {
            char[,] board = new char[250, 250];
            Random rand = new Random(12345);

            for(int i = 0; i < 250; ++i)
            {
                for(int j = 0; j < 250; ++j)
                {
                    board[i, j] = (char)(rand.Next() % 26 + 'a');
                }
            }

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            AssertEqual(results.Score, 84461);
        }
        /* End of Test Methods */

        /// <summary>
        /// Acts like an assert, but does not crash program, instead adds to an exception list of failed tests
        /// </summary>
        public static void Assert(bool expression)
        {
            if(!expression)
            {
                if(null == failedTests)
                {
                    failedTests = new List<Exception>();
                }

                StackTrace stackTrace = new StackTrace();
                MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

                failedTests.Add(new Exception(String.Format("Failed Test: {0}", methodBase.Name)));
            }
        }

        /// <summary>
        /// Allows comparison of generic types
        /// </summary>
        public static void AssertEqual<T>(T lhs, T rhs)
        {
            if (!lhs.Equals(rhs))
            {
                if (null == failedTests)
                {
                    failedTests = new List<Exception>();
                }

                StackTrace stackTrace = new StackTrace();
                MethodBase methodBase = stackTrace.GetFrame(1).GetMethod();

                failedTests.Add(new Exception(String.Format("Failed Test: {0}, Expected value: {1}, got {2}", methodBase.Name, lhs, rhs)));
            }
        }
    }
}
