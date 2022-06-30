using TargetWord.Core.ViewModels;

namespace TargetWord;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainViewModel = new MainViewModel();

		MainPage = new AppShell();
	}

	public static MainViewModel MainViewModel { get; set; }
}
