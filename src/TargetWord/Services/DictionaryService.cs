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
            CommonWordList = await LoadWordListFromFile(@"Assets\Dictionary\CommonWords.txt");
            ObscureWordList = await LoadWordListFromFile(@"Assets\Dictionary\ObscureWords.txt");
        }

        public async Task<List<string>> LoadWordListFromFile(string path)
        {
            //TODO: Write methods which pull from a dictionary
            var returnList = new List<string>();
            //StorageFile file = await Package.Current.InstalledLocation.GetFileAsync(path);

            //IInputStream stream = await file.OpenSequentialReadAsync();

            //var rdr = new StreamReader(stream.AsStreamForRead());

            //using (var sr = new StreamReader(stream.AsStreamForRead()))
            //{
            //    String line;
            //    while ((line = sr.ReadLine()) != null)
            //    {
            //        returnList.Add(line);
            //    }
            //}
            return returnList;
        }




    }
}