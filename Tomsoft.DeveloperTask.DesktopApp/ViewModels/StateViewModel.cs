using System;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using ReactiveUI;
using Tomsoft.DeveloperTask.Data;
using Tomsoft.DeveloperTask.DesktopApp.Models;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Pages;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels;

public class StateViewModel : ViewModelBase, IDisposable
{
    private LuceedApiClient? _apiClient;

    public new LuceedApiClient? ApiClient
    {
        get => _apiClient;
        set
        {
            _apiClient?.Dispose();
            _apiClient = value;
        }
    }

    public CompositeDisposable CompositeDisposable { get; } = new();

    private static readonly PageViewModelBase[] PossiblePages = typeof(PageViewModelBase).Assembly.GetTypes()
        .Where(type => type.IsAssignableTo(typeof(PageViewModelBase)) && type != typeof(PageViewModelBase))
        .Select(type => (PageViewModelBase)Activator.CreateInstance(type)!)
        .ToArray();

    private PageViewModelBase _page;
    public PageViewModelBase Page
    {
        get => _page;
        set => this.RaiseAndSetIfChanged(ref _page, value);
    }
    
    public object? PageData { get; set; }

    private string? _username;
    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    public StateViewModel()
    {
        var credentialsPage = FindPage<CredentialsPageViewModel>();
        _page = credentialsPage;
        
        Task.Run(async () =>
        {
            var credentials = new LuceedApiCredentials();
            await credentials.Load();

            var canConnect = await CredentialsPageViewModel.TestCredentials(credentials);

            if (credentials.Username is not null)
            {
                credentialsPage.Username = credentials.Username;
            }

            if (credentials.Password is not null)
            {
                credentialsPage.Password = credentials.Password;
            }

            return canConnect;
        }).ContinueWith(task =>
        {
            var canConnect = task.Result;
            if (canConnect)
            {
                ChangePage(Models.Page.Articles);
            }
        }, TaskScheduler.FromCurrentSynchronizationContext());
    }

    public new void ChangePage(Page page, object? data = null)
    {
        PageData = data;
        
        var pageViewModel = FindPage(page);
        
        if (!pageViewModel.WasInitialized)
        {
            pageViewModel.Initialize();
            pageViewModel.WasInitialized = true;
        }

        pageViewModel.BeforeNavigation(data);

        Page = pageViewModel;
    }

    private static PageViewModelBase FindPage(Page page)
        => PossiblePages.First(x => x.Page == page);

    private static T FindPage<T>()
        where T : PageViewModelBase
    {
        var pageType = typeof(T);
        return (T)PossiblePages.First(page => page.GetType() == pageType);
    }

    public void Dispose()
    {
        _apiClient?.Dispose();
        CompositeDisposable.Dispose();
        GC.SuppressFinalize(this);
    }
}