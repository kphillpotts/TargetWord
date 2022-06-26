using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetWord.Models
{
    public class SubmitResponse
    {
        /// <summary>
        ///     Was the word accepted
        /// </summary>
        public bool Acceptable { get; set; }

        /// <summary>
        ///     If the word was not acceptable what was the reason
        /// </summary>
        public string ValidationMessage { get; set; }
    }
}
