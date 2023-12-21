using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Tomsoft.DeveloperTask.DesktopApp.Models;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    [ObservableAsProperty] public bool ShowNavigationBar { get; }

    public new static StateViewModel State { get; } = new();

    public ReactiveCommand<Unit, Unit> GoToArticlesPageCommand { get; }
    public ReactiveCommand<Unit, Unit> GoToWarehousesPageCommand { get; }
    public ReactiveCommand<Unit, Unit> SignOutCommand { get; }

    public MainWindowViewModel()
    {
        GoToArticlesPageCommand = ReactiveCommand.Create(() => ChangePage(Page.Articles));
        GoToWarehousesPageCommand = ReactiveCommand.Create(() => ChangePage(Page.Warehouses));
        SignOutCommand = ReactiveCommand.Create(() => ChangePage(Page.Credentials));

        State.WhenAnyValue(x => x.Page)
            .Select(x => x.Page != Page.Credentials)
            .ToPropertyEx(this, x => x.ShowNavigationBar);
    }
}