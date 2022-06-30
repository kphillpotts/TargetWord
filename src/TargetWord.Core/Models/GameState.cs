using TargetWord.Core.Models;

namespace TargetWord.Core.Models
{
    public class GameState
    {
        #region Constructor

        public GameState()
        {
            SelectedLetter = new List<GameLetter>();
            GameLetters = new List<GameLetter>();
            WordList = new List<GameWord>();
            GameInProgress = true;
            TotalPossibleScore = 0;
            CurrentGameScore = 0;
            ChainScore = 0;
            ChainLength = 1;
            CanProgressChain = false;
            GameWordsFound = 0;
        }

        #endregion

        #region Level Words and Letters

        /// <summary>
        ///     List of letters available to choose words from
        /// </summary>
        public List<GameLetter> GameLetters { get; set; }

        /// <summary>
        ///     List of words in the Game
        /// </summary>
        public List<GameWord> WordList { get; set; }

        /// <summary>
        ///     Number of words requried to progress to next level
        /// </summary>
        public int WordsRequiredToProgress { get; set; }

        /// <summary>
        ///     Gets or Sets the number of words found in the game
        /// </summary>
        public int GameWordsFound { get; set; }

/// <summary>
/// Indicates the difficulty of the game
/// </summary>
        public GameDifficulty GameDifficulty { get; set; }

        #endregion

        #region Level User State

        /// <summary>
        ///     Ordered list of letters that are currently selected
        /// </summary>
        public List<GameLetter> SelectedLetter { get; set; }

        public bool IsTargetLetterFound
        {
            get
            {
                GameWord wordFound =
                    WordList.FirstOrDefault(o => o.Found && o.IsHintWord == false && o.Word.Length == 9);
                return wordFound != null;
            }
        }

        /// <summary>
        ///     Indicates if the player can proceed to next level
        /// </summary>
        public bool CanProgressChain { get; set; }

        /// <summary>
        ///     Indicates if the game is currently in progress
        /// </summary>
        public bool GameInProgress { get; set; }

        #endregion

        #region Scoring

        /// <summary>
        ///     Indicates the current level of the game
        /// </summary>
        public int ChainLength { get; set; }

        /// <summary>
        ///     Indicates the score in the current level
        /// </summary>
        public int CurrentGameScore { get; set; }

        /// <summary>
        ///     Total possible score
        /// </summary>
        public int TotalPossibleScore { get; set; }

        /// <summary>
        ///     indicates the current score of the game
        /// </summary>
        public int ChainScore { get; set; }

        #endregion

        public bool NewGameMessageDisplayed { get; set; }
    }

}
