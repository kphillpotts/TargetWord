namespace TargetWord.Services
{
    public class DictionaryService
    {

        private readonly Random _rnd = new Random();

        private List<string> _commonWordList = new List<string>();
        private List<string> _obscureWordList = new List<string>();

        public List<string> CommonWordList
        {
            get => _commonWordList;
            set => _commonWordList = value;
        }

        public List<string> ObscureWordList
        {
            get { return _obscureWordList; }
            set { _obscureWordList = value; }
        }

        public async Task LoadDictionary()
        {
            CommonWordList = await LoadWordListFromFile(@"CommonWords.txt");
            ObscureWordList = await LoadWordListFromFile(@"ObscureWords.txt");
        }

        public async Task<List<string>> LoadWordListFromFile(string path)
        {
            //TODO: Write methods which pull from a dictionary
            var returnList = new List<string>();

            using var stream = await FileSystem.OpenAppPackageFileAsync(path);
            using var reader = new StreamReader(stream);
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    returnList.Add(line);
                }

            }
            return returnList;
        }
    }
}