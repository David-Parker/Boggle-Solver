using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Boggle;

namespace Boggle
{
    class Program
    {
        static void Main(string[] args)
        {
            char[,] board =
            {
                { 'd','z','x' },
                { 'e','a','i' },
                { 'q','u','t' }
            };

            ISolver solver = MyBoggleSolution.CreateSolver("../../dictionary.txt");
            IResults results = solver.FindWords(board);

            foreach (var result in results.Words)
            {
                Console.WriteLine(result);
            }

            Console.WriteLine(String.Format("Final Score: {0}", results.Score));
        }
    }
}
