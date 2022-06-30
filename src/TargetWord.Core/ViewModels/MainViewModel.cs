using Microsoft.Toolkit.Mvvm.ComponentModel;
using TargetWord.Core.Models;

namespace TargetWord.Core.ViewModels
{
    [INotifyPropertyChanged]
    public partial class MainViewModel 
    {
        [ObservableProperty]
        private GameState? currentGame;

        [ObservableProperty]
        private string? myText;
    }
}
