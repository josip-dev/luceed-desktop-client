using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Tomsoft.DeveloperTask.Data.Models.Articles;
using Tomsoft.DeveloperTask.Data.Models.Articles.NamePartSearch;
using Tomsoft.DeveloperTask.DesktopApp.Models;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels.Pages;

public class ArticlesPageViewModel : PageViewModelBase
{
    private string _lastSearch = string.Empty;
    
    private string _articleNamePart = string.Empty;
    public string ArticleNamePart
    {
        get => _articleNamePart;
        set => this.RaiseAndSetIfChanged(ref _articleNamePart, value);
    }

    public ObservableCollection<Article> Articles { get; } = new();

    [ObservableAsProperty]
    public bool AnyArticlesPresent { get; }

    public ReactiveCommand<Unit, Unit> SearchArticlesCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearNamePartCommand { get; }

    public ArticlesPageViewModel() : base(Page.Articles)
    {
        SearchArticlesCommand = ReactiveCommand.CreateFromTask(SearchArticles,
            this.WhenAnyValue(x => x.ArticleNamePart).Select(namePart => namePart != _lastSearch));
        ClearNamePartCommand = ReactiveCommand.CreateFromTask(ClearNamePart,
            this.WhenAnyValue(x => x.ArticleNamePart).Select(namePart => namePart != string.Empty));

        Articles.ToObservableChangeSet(x => x)
            .ToCollection()
            .Select(items => items.Any())
            .ToPropertyEx(this, x => x.AnyArticlesPresent);
    }

    public override void Initialize()
    {
        SearchArticlesCommand.Execute().Subscribe();
    }

    private async Task ClearNamePart(CancellationToken cancellationToken)
    {
        ArticleNamePart = string.Empty;
        
        if (_lastSearch == string.Empty)
        {
            return;
        }
        
        await SearchArticles(cancellationToken);
    }

    private async Task SearchArticles(CancellationToken cancellationToken)
    {
        var response = await ApiClient
            .WithRouteParts("artikli", "naziv", ArticleNamePart)
            .Get<NamePartSearchArticlesResponse>(cancellationToken);

        if (response is null)
        {
            return;
        }
        
        _lastSearch = ArticleNamePart;

        Articles.Clear();
        Articles.AddRange(response.Articles);
    }
}