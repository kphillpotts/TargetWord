using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TargetWord.Core.Models;

public class GameWord
{
    /// <summary>
    ///     Actual word
    /// </summary>
    public string Word { get; set; }

    /// <summary>
    ///     What is the total score of the word based on scrabble scores
    /// </summary>
    public int WordScore { get; set; }

    /// <summary>
    ///     Is this considered a standard or extended word
    /// </summary>
    public bool UncommonWord { get; set; }

    /// <summary>
    ///     Has the word been discovered, either by hint or by user guess
    /// </summary>
    public bool Found { get; set; }

    /// <summary>
    ///     Indicates if the word was found via a hint
    ///     This is important because we need to store not only if a word is found
    ///     but whether the reason it was found was because of a hint
    /// </summary>
    public bool IsHintWord { get; set; }

}
