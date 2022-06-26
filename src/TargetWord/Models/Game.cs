using System.Diagnostics;

namespace TargetWord.Models
{
    public class Game
    {
        private readonly Random _rnd = new();

        public GameState GameSession { get; set; }

        //public Settings Settings { get; set; }

        //public Statistics Statistics { get; set; }

        public GameDictionary GameDictionary { get; set; }



        public GameState CreateNewGame(GameDifficulty difficulty)
        {
            string gameletters;
            List<string> gameWords;
            List<string> extendedGameWords;

            // create a random game set of word lists
            RandomGame(difficulty, out gameletters, out gameWords, out extendedGameWords);

            // create a new game
            var newGame = new GameState();
            newGame.GameDifficulty = difficulty;
            newGame.GameLetters = StringToGameLetterList(gameletters);
            GameSession = newGame;
            AddWordsToGameWordList(gameWords, extendedGameWords);

            var activeWordCount = GameSession.WordList.Count(o => o.UncommonWord == false);
            if (difficulty == GameDifficulty.Hard)
                activeWordCount = GameSession.WordList.Count;

            GameSession.WordsRequiredToProgress = CalculateWordsRequiredToProgress(activeWordCount,
                                                                                   GameSession.ChainLength);

            return newGame;
        }

        public void ProceedToNextLevel()
        {
            // increment the chain information
            GameSession.ChainLength++;

            GameSession.SelectedLetter.Clear();

            string gameletters = string.Empty;
            var commonWords = new List<string>();
            var extendedGameWords = new List<string>();


            RandomGame(GameSession.GameDifficulty, out gameletters, out commonWords, out extendedGameWords);
            AddWordsToGameWordList(commonWords, extendedGameWords);

            GameSession.GameLetters = StringToGameLetterList(gameletters);
            GameSession.CanProgressChain = false;
            GameSession.CurrentGameScore = 0;
            GameSession.GameInProgress = true;

            var activeWordCount = GameSession.WordList.Count(o => o.UncommonWord == false);
            if (GameSession.GameDifficulty == GameDifficulty.Hard)
                activeWordCount = GameSession.WordList.Count;

            GameSession.WordsRequiredToProgress = CalculateWordsRequiredToProgress(activeWordCount,
                                                                                   GameSession.ChainLength);
        }

        private static int GetMinimumWordLength(GameDifficulty difficulty)
        {
            return difficulty switch
            {
                GameDifficulty.Easy => 3,
                GameDifficulty.Medium => 4,
                GameDifficulty.Hard => 4,
                _ => 4,
            };
        }

        private int CalculateWordsRequiredToProgress(int totalWordCount, int chainLength)
        {


            int percentRequired = chainLength * 5;
            if (percentRequired > 100) percentRequired = 100;

            int wordsRequired = totalWordCount * percentRequired / 100;

            if (wordsRequired < 5) wordsRequired = 5;

            return wordsRequired;
        }

        private void AddWordsToGameWordList(List<string> commonWords, List<string> extendedWords)
        {
            // reset the word list
            GameSession.WordList = new List<GameWord>();
            GameSession.TotalPossibleScore = 0;

            // common words
            foreach (var word in commonWords)
            {
                var score = WordScore(word);
                GameSession.WordList.Add(new GameWord { Word = word, WordScore = score, UncommonWord = false });
                GameSession.TotalPossibleScore += score;
                Debug.WriteLine(word);
            }

            // extended words
            foreach (var word in extendedWords)
            {
                // double points for extended words
                var score = WordScore(word) * 2;
                GameSession.WordList.Add(new GameWord { Word = word, WordScore = score, UncommonWord = true });
                GameSession.TotalPossibleScore += score;
                Debug.WriteLine(word);
            }
        }


        public SubmitResponse SelectLetter(GameLetter letter)
        {
            // if letter is already selected
            if (letter.Selected)
            {
                if (GameSession.SelectedLetter.Count > 0)
                {
                    if (GameSession.SelectedLetter[GameSession.SelectedLetter.Count - 1] == letter)
                    {
                        GameSession.SelectedLetter.Remove(letter);
                        letter.Selected = false;
                    }
                }
            }
            else
            {
                letter.Selected = true;
                GameSession.SelectedLetter.Add(letter);
            }
            return new SubmitResponse { Acceptable = true };
        }


        public void RemoveSelectecdLetter(GameLetter letter)
        {
            if (GameSession.SelectedLetter.Contains(letter))
            {
                GameSession.SelectedLetter.Remove(letter);
                letter.Selected = false;
            }
        }

        public void ClearSelectedLetters()
        {
            foreach (GameLetter item in GameSession.GameLetters)
            {
                RemoveSelectecdLetter(item);
            }
        }


        public int TotalWords()
        {
            return GameSession.WordList.Count;
        }

        public int FoundWordCount(bool includeHints)
        {
            if (includeHints)
                return GameSession.WordList.Where(o => o.Found).Count();
            else
                return GameSession.WordList.Where(o => o.Found && o.IsHintWord == false).Count();
        }


        public void ClearLastLetter()
        {
            int lastLetter = GameSession.SelectedLetter.Count - 1;

            if (lastLetter >= 0)
            {
                GameSession.SelectedLetter[lastLetter].Selected = false;
                GameSession.SelectedLetter.Remove(GameSession.SelectedLetter[lastLetter]);
            }
        }

        public SubmitResponse SubmitWord(string word)
        {
            if (GameSession.GameInProgress == false)
                return new SubmitResponse { Acceptable = false, ValidationMessage = "Game Over" };

            if ((String.IsNullOrEmpty(word)) || (word.Length < GetMinimumWordLength(GameSession.GameDifficulty)))
                return new SubmitResponse { Acceptable = false, ValidationMessage = "Word must be at least " + GetMinimumWordLength(GameSession.GameDifficulty) + " characters" };

            if (!word.ToLower().Contains(TargetLetter().Letter.ToLower()))
            {
                return new SubmitResponse { Acceptable = false, ValidationMessage = "You must use the target letter" };
            }

            GameWord result = GameSession.WordList.Where(w => w.Word.ToLower() == word.ToLower()).FirstOrDefault();
            if (result == null)
                return new SubmitResponse { Acceptable = false, ValidationMessage = "Word not in dictionary" };

            // check if the word is already in use
            if (result.Found)
                return new SubmitResponse { Acceptable = false, ValidationMessage = "Word already found" };

            // all good
            result.Found = true;

            //App.GameEngine.Statistics.AddWordToStats(word);

            GameSession.CurrentGameScore += result.WordScore;
            GameSession.ChainScore += result.WordScore;
            GameSession.GameWordsFound++;

            // if we found a nine letter word we can progress
            if (word.Length == 9)
            {
                if (!GameSession.CanProgressChain)
                {
                    GameSession.CanProgressChain = true;
                    //App.GameEngine.Statistics.AddLevelsCleared();
                }
            }
            else if (FoundWordCount(false) >= GameSession.WordsRequiredToProgress)
            {
                if (!GameSession.CanProgressChain)
                {
                    GameSession.CanProgressChain = true;
                    //App.GameEngine.Statistics.AddLevelsCleared();
                }
            }

            // if no  more words to find
            if (GameSession.WordList.Where(w => w.Found == false).Count() == 0)
            {
                GameSession.GameInProgress = false;
            }

            if (GameSession.CanProgressChain == false && GameSession.GameInProgress == false)
                FinishGame();

            return new SubmitResponse { Acceptable = true };
        }

        public GameWord GetWordHint()
        {
            int wordcount = GameSession.WordList.Where(w => (w.Found == false)).Count();
            if (wordcount == 0)
                return null;

            GameWord result =
                GameSession.WordList.Where(w => (w.Found == false)).ElementAtOrDefault(_rnd.Next(0, wordcount - 1));
            if (result == null)
                return null;
            else
            {
                result.IsHintWord = true;

                //App.GameEngine.Statistics.AddHintToStats();

                result.Found = true;
                GameSession.CurrentGameScore -= result.WordScore;
                GameSession.ChainScore -= result.WordScore;

                if (GameSession.WordList.Where(w => w.Found == false).Count() == 0)
                    GameSession.GameInProgress = false;

                if (GameSession.GameInProgress == false && GameSession.CanProgressChain == false)
                {
                    FinishGame();
                }

                return result;
            }
        }


        internal SubmitResponse SelectLetter(string p)
        {
            // check if this is the targetletter
            if (p.ToLower() == TargetLetter().Letter.ToLower())
            {
                if (!TargetLetter().Selected)
                {
                    return SelectLetter(TargetLetter());
                }
            }


            GameLetter result =
                GameSession.GameLetters.Where(o => (o.Letter.ToLower() == p.ToLower()) && (o.Selected == false))
                           .FirstOrDefault();
            if (result != null)
            {
                return SelectLetter(result);
            }

            return new SubmitResponse
            {
                Acceptable = false,
                ValidationMessage = "Letter is not in puzzle or is already selected"
            };
        }

        public GameLetter TargetLetter()
        {
            if ((GameSession.GameLetters == null) || (GameSession.GameLetters.Count < 5))
                throw new InvalidOperationException("Game contains less than 5 letters, unable to get taret letter");

            return GameSession.GameLetters[4];
        }


        private static List<GameLetter> StringToGameLetterList(string letters)
        {
            var gameChars = letters.ToCharArray();
            return gameChars.Select(gameChar => new GameLetter(gameChar.ToString())).ToList();
        }


        private int WordScore(string word)
        {
            int score = 0;

            // work out the points for the word
            char[] chars = word.ToCharArray();

            foreach (char item in chars)
            {
                score = score + LetterPointValue(item.ToString());
            }
            return score;
        }

        internal void ShuffleLetters()
        {
            // get the non target letters out into an array

            GameLetter targetLetter = GameSession.GameLetters[4];
            GameSession.GameLetters.RemoveAt(4);

            IOrderedEnumerable<GameLetter> newlist = GameSession.GameLetters.OrderBy(a => Guid.NewGuid());

            var newLetterList = new List<GameLetter>();

            foreach (GameLetter item in newlist)
            {
                newLetterList.Add(item);
            }


            newLetterList.Insert(4, targetLetter);

            GameSession.GameLetters = newLetterList;
        }


        public int LetterPointValue(string character)
        {
            return character.ToLower() switch
            {
                "a" => 1,
                "b" => 3,
                "c" => 3,
                "d" => 2,
                "e" => 1,
                "f" => 4,
                "g" => 2,
                "h" => 4,
                "i" => 1,
                "j" => 8,
                "k" => 5,
                "l" => 1,
                "m" => 3,
                "n" => 1,
                "o" => 1,
                "p" => 3,
                "q" => 10,
                "r" => 1,
                "s" => 1,
                "t" => 1,
                "u" => 1,
                "v" => 4,
                "w" => 4,
                "x" => 8,
                "y" => 4,
                "z" => 10,
                _ => 0,
            };
        }


        internal void QuitGame()
        {
            FinishGame();
        }

        public void FinishGame()
        {
            GameSession.GameInProgress = false;
            ProcessEndGame();
        }


        public void ProcessEndGame()
        {
            GameSession.GameInProgress = false;
            GameSession.CanProgressChain = false;

            //App.GameEngine.Statistics.AddGamesPlayed();

            // write score to high score table
            WriteScoreToTable(GameSession.ChainLength - 1, GameSession.ChainScore, GameSession.GameWordsFound);
        }


        private void UpdateScoreTable(List<int> scoreTable, int value)
        {
            scoreTable.Add(value);
            scoreTable.Sort();
            scoreTable.Reverse();

            if (scoreTable.Count() > 5)
                scoreTable.RemoveAt(5);
        }

        private void WriteScoreToTable(int level, int score, int words)
        {
            //UpdateScoreTable(Settings.TopHighestLevel, level);
            //UpdateScoreTable(Settings.TopScores, score);
            //UpdateScoreTable(Settings.TopMostWords, words);
        }



        public void RandomGame(GameDifficulty difficulty,
                       out string gameLetters,
                       out List<string> commonWords,
                       out List<string> extendedGameWords)
        {
            var wordAcceptable = false;
            var nineLetterWord = string.Empty;
            var randomLetters = string.Empty;
            var targetLetter = '0';

            while (!wordAcceptable)
            {
                // get a random 9 letter word
                List<string> wordList = difficulty == GameDifficulty.Hard ? GameDictionary.ObscureWordList : GameDictionary.CommonWordList;
                IEnumerable<string> allNineLetterWords = wordList.Where(o => o.Length == 9);
                nineLetterWord = allNineLetterWords.ToList()[_rnd.Next(allNineLetterWords.Count())];

                // get a subset of all the possible words - regardless of target letter
                // we do this for performance to reduce the size of the dictionary we are working with
                // if we are in hard mode then get words from both dictionaries
                List<string> allPossibleWords = GetWordsForLetters(nineLetterWord, ' ', GameDictionary.CommonWordList, GetMinimumWordLength(difficulty));
                if (difficulty == GameDifficulty.Hard)
                    allPossibleWords.AddRange(GetWordsForLetters(nineLetterWord, ' ', GameDictionary.ObscureWordList, GetMinimumWordLength(difficulty)));

                // randomize the letter order of the word. We want to do this so that
                // when we try and calculate a target letter we don't have a bias for the first 
                // characters in the word
                randomLetters = new string(nineLetterWord.ToCharArray().OrderBy(s => (_rnd.Next(2) % 2) == 0).ToArray());

                // find the first letter that could be a target letter that produces at least 20 words
                targetLetter = CalculateTargetLetter(randomLetters, allPossibleWords, GetMinimumWordLength(difficulty));

                // check if we came back with a letter, if not move onto next word
                if (targetLetter == '0')
                    continue;

                // get all words for the targetletter
                var targetWords = GetWordsForLetters(nineLetterWord, targetLetter, allPossibleWords, GetMinimumWordLength(difficulty));

                // if we get more than 20 words we are good to go
                if (targetWords.Count >= 20)
                    wordAcceptable = true;
            }

            // construct values to return
            gameLetters = OrganiseLettersAroundTargetLetter(randomLetters, targetLetter);
            commonWords = GetWordsForLetters(nineLetterWord, targetLetter, GameDictionary.CommonWordList, GetMinimumWordLength(difficulty));
            extendedGameWords = GetWordsForLetters(nineLetterWord, targetLetter, GameDictionary.ObscureWordList, GetMinimumWordLength(difficulty));
        }


        private char CalculateTargetLetter(string puzzleLetters, List<string> allWords, int minimumLetterCount)
        {
            // go through each letter
            foreach (char letter in puzzleLetters)
            {
                // if we get at least 20 words, that'll do for the target letter
                List<string> wordsForLetter = GetWordsForLetters(puzzleLetters, letter, allWords, minimumLetterCount);

                if (wordsForLetter.Count >= 20)
                    return letter;
            }

            // indicate that we didn't find enought words
            return '0';
        }


        private static string OrganiseLettersAroundTargetLetter(string randomLetters, char targetLetter)
        {
            string output = string.Empty;
            bool passedTargetLetter = false;
            foreach (char character in randomLetters.ToCharArray())
            {
                if ((character == targetLetter) && (!passedTargetLetter))
                {
                    passedTargetLetter = true;
                    continue;
                }
                output = output + character;
            }

            output = output.Insert(4, targetLetter.ToString());
            return output;
        }


        private static List<string> GetWordsForLetters(string letters, char targetLetter, List<string> dictionary,
                                                int minWordLength)
        {

            var puzzleLetters = letters.ToCharArray();

            var resultWords = new List<string>();

            foreach (string item in dictionary.Where(o => (o.Length >= minWordLength) && (o.Length <= 9)))
            {
                bool isvalid = true;

                char[] wordletters = item.ToCharArray();

                // if we are looking for words with specific letter, check it contains it
                if (targetLetter != ' ')
                {
                    if (!wordletters.Contains(targetLetter))
                    {
                        isvalid = false;
                        continue;
                    }
                }


                foreach (char letter in wordletters)
                {
                    if (!puzzleLetters.Contains(letter))
                    {
                        isvalid = false;
                        break;
                    }
                    else
                    {
                        int letterCount = wordletters.Where(o => o == letter).Count();
                        int selectedLetterCoutn = puzzleLetters.Where(o => o == letter).Count();
                        if (letterCount > selectedLetterCoutn)
                        {
                            isvalid = false;
                            break;
                        }
                    }
                }


                // check that the word contains

                if (isvalid)
                {
                    resultWords.Add(item);
                }
            }

            return resultWords;
        }


    }

}
