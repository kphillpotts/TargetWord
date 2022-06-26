using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TargetWord.Services;

namespace TargetWord.Models
{
    public class GameDictionary
    {
        #region Private Members

        private readonly Random _rnd = new Random();

        private List<string> _commonWordList = new List<string>();
        private List<string> _obscureWordList = new List<string>();

        #endregion

        #region Public Access to Word Lists

        /// <summary>
        /// List of Common Use Words
        /// </summary>
        public List<string> CommonWordList
        {
            get { return _commonWordList; }
            set { _commonWordList = value; }
        }

        /// <summary>
        /// List of words considered Obscure
        /// This list is an extension to the Common Word List
        /// </summary>
        public List<string> ObscureWordList
        {
            get { return _obscureWordList; }
            set { _obscureWordList = value; }
        }

        #endregion

        /// <summary>
        /// Load the word lists from file into the Word List collections
        /// </summary>
        /// <returns></returns>
        public async Task LoadDictionary()
        {
            CommonWordList = await LoadWordListFromFile(@"Assets\Dictionary\CommonWords.txt");
            ObscureWordList = await LoadWordListFromFile(@"Assets\Dictionary\ObscureWords.txt");
        }

        private async Task<List<string>> LoadWordListFromFile(string path)
        {
            var returnList = new List<string>();

            GameDictionary dict = new GameDictionary();
            returnList = await dict.LoadWordListFromFile(path);

            return returnList;
        }
    }
}
