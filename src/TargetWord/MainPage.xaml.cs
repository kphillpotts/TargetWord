using TargetWord.Services;

namespace TargetWord;

public partial class MainPage : ContentPage
{
    DictionaryService s = new DictionaryService();
    public MainPage()
	{
		InitializeComponent();
		this.BindingContext = App.MainViewModel;
	}

	private async void Button_Clicked(object sender, EventArgs e)
	{

		await s.LoadDictionary();
		await this.DisplayActionSheet("Dictionary", "cancel", "destriuction", new string[] { $"Common {s.CommonWordList.Count}", $"Obscure {s.ObscureWordList.Count}" }); ;

	}

	private void NewGameButton_Clicked(object sender, EventArgs e)
	{
		App.MainViewModel.MyText = Guid.NewGuid().ToString();
        App.MainViewModel.CurrentGame = new Core.Models.GameState();
		Shell.Current.GoToAsync("gamepage");
	}

	private void OptionsButton_Clicked(object sender, EventArgs e)
	{

	}
}

