using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetWord.Models
{
    public class GameLetter
    {
        public string Letter { get; set; }

        public GameLetter(string letter)
        {
            Letter = letter;
        }

        public bool Selected { get; set; } = false;
    }
}
