using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boggle
{
    /// <summary>
    /// Packet of data that represents the results of all solutions on a Boggle grid.
    /// </summary>
    public class Results : IResults
    {
        private List<string> words = new List<string>();
        private int score;

        public IEnumerable<string> Words {
            get
            {
                return words;
            }
        }

        public int Score {
            get
            {
                return score;
            }
        }

        public void SetWords(List<string> words)
        {
            this.words = words;
        }

        public void AddScore(int points)
        {
           score += points;
        }
    }
}
