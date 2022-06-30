namespace TargetWord.Pages;

public partial class GamePage : ContentPage
{
	public GamePage()
	{
		InitializeComponent();
		this.BindingContext = App.MainViewModel;
	}
}