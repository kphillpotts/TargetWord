using TargetWord.Pages;

namespace TargetWord;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();
		Routing.RegisterRoute("gamepage", typeof(GamePage));
	}
}
