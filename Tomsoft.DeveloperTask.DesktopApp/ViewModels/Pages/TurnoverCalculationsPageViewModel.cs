using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using DynamicData;
using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Tomsoft.DeveloperTask.Data.Models.Turnover;
using Tomsoft.DeveloperTask.Data.Models.Turnover.ByArticle;
using Tomsoft.DeveloperTask.Data.Models.Turnover.ByBusinessUnit;
using Tomsoft.DeveloperTask.Data.Models.Warehouses;
using Tomsoft.DeveloperTask.DesktopApp.Models;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels.Pages;

public class TurnoverCalculationsPageViewModel : PageViewModelBase
{
    public record PageParameters(Warehouse Warehouse, bool ForArticles);

    private const string BASE_ROUTE_PART = "mpobracun";
    private const string ARTICLES_ROUTE_PART = "artikli";
    private const string PAYMENTS_ROUTE_PART = "placanja";
    private const string DATE_FORMAT = "d.M.yyyy";

    private static readonly DateTimeOffset DefaultLookupStartDate = new(new DateTime(2013, 1, 1));

    private bool? _forArticles;
    public bool? ForArticles
    {
        get => _forArticles;
        set => this.RaiseAndSetIfChanged(ref _forArticles, value);
    }

    private string? _businessUnitUid;
    public string? BusinessUnitUid
    {
        get => _businessUnitUid;
        set => this.RaiseAndSetIfChanged(ref _businessUnitUid, value);
    }

    private string? _warehouse;
    public string? Warehouse
    {
        get => _warehouse;
        set => this.RaiseAndSetIfChanged(ref _warehouse, value);
    }

    private string? _type;
    public string? Type
    {
        get => _type;
        set => this.RaiseAndSetIfChanged(ref _type, value);
    }

    private DateTimeOffset _lookupStartDate = DefaultLookupStartDate;
    public DateTimeOffset LookupStartDate
    {
        get => _lookupStartDate;
        set => this.RaiseAndSetIfChanged(ref _lookupStartDate, value);
    }

    private DateTimeOffset? _lookupEndDate;
    public DateTimeOffset? LookupEndDate
    {
        get => _lookupEndDate;
        set => this.RaiseAndSetIfChanged(ref _lookupEndDate, value);
    }

    public ObservableCollection<TurnoverCalculation> TurnoverCalculations { get; } = new();
    public ObservableCollection<TurnoverCalculationByArticle> TurnoverCalculationsByArticle { get; } = new();

    [ObservableAsProperty] public bool ShowClearFiltersButton { get; }

    [ObservableAsProperty] public bool ShowForArticles { get; }

    [ObservableAsProperty] public bool ShowForPayments { get; }

    [ObservableAsProperty] public bool AnyTurnoverCalculationsPresentForArticles { get; }

    [ObservableAsProperty] public bool AnyTurnoverCalculationsPresentForPayments { get; }

    public ReactiveCommand<Unit, Unit> FetchTurnoverCalculationsCommand { get; }
    public ReactiveCommand<Unit, Unit> ClearFiltersCommand { get; }

    public TurnoverCalculationsPageViewModel() : base(Page.TurnoverCalculations)
    {
        FetchTurnoverCalculationsCommand = ReactiveCommand.CreateFromTask(FetchTurnoverCalculations);
        ClearFiltersCommand = ReactiveCommand.Create(() =>
        {
            LookupStartDate = DefaultLookupStartDate;
            LookupEndDate = null;
        });

        this.WhenAnyValue(x => x.ForArticles)
            .Select(forArticles => forArticles == true)
            .ToPropertyEx(this, x => x.ShowForArticles);

        this.WhenAnyValue(x => x.ForArticles)
            .Select(forArticles => forArticles == false)
            .ToPropertyEx(this, x => x.ShowForPayments);

        TurnoverCalculations.ToObservableChangeSet(x => x)
            .ToCollection()
            .Select(items => items.Any())
            .ToPropertyEx(this, x => x.AnyTurnoverCalculationsPresentForPayments);

        TurnoverCalculationsByArticle.ToObservableChangeSet(x => x)
            .ToCollection()
            .Select(items => items.Any())
            .ToPropertyEx(this, x => x.AnyTurnoverCalculationsPresentForArticles);

        this.WhenAnyValue(
                x => x.LookupStartDate,
                x => x.LookupEndDate,
                x => x.ShowForArticles,
                x => x.ShowForPayments,
                x => x.AnyTurnoverCalculationsPresentForPayments,
                x => x.AnyTurnoverCalculationsPresentForArticles)
            .Select(tuple =>
            {
                var (start, end, forArticles, forPayments, anyDataForPayments, anyDataForArticles) = tuple;
                
                bool anyDataPresent;
                if (!forArticles && !forPayments)
                {
                    anyDataPresent = false;
                }
                else
                {
                    anyDataPresent = forArticles ? anyDataForArticles : anyDataForPayments;
                }
                
                return !anyDataPresent && (start != DefaultLookupStartDate || end is not null);
            })
            .ToPropertyEx(this, x => x.ShowClearFiltersButton);
    }

    public override void Initialize()
    {
        this.WhenAnyValue(x => x.LookupStartDate,
                x => x.LookupEndDate,
                x => x.BusinessUnitUid,
                x => x.ForArticles)
            .Where(x => x.Item3 is not null && x.Item4 is not null)
            .Select(_ => Unit.Default)
            .InvokeCommand(FetchTurnoverCalculationsCommand)
            .DisposeWith(State.CompositeDisposable);
    }

    public override void BeforeNavigation(object? pageData = null)
    {
        if (pageData is not PageParameters pageParameters)
        {
            throw new ArgumentException("No page parameters were provided. Business Unit UID is required in order to view turnover calculations.", nameof(pageData));
        }

        var (warehouse, forArticles) = pageParameters;

        Warehouse = $"{warehouse.Name} - {warehouse.BusinessUnitName}";
        BusinessUnitUid = warehouse.BusinessUnitUid;
        ForArticles = forArticles;
        Type = forArticles ? "by articles" : "by payments";
    }

    private async Task FetchTurnoverCalculations(CancellationToken cancellationToken)
    {
        if (!ForArticles.HasValue || BusinessUnitUid is null)
        {
            return;
        }

        if (ForArticles.Value)
        {
            var response = await ApiClient
                .WithRouteParts(BASE_ROUTE_PART, ARTICLES_ROUTE_PART, BusinessUnitUid, LookupStartDate.ToString(DATE_FORMAT), LookupEndDate?.ToString(DATE_FORMAT))
                .Get<TurnoverCalculationsByArticleResponse>(cancellationToken);

            if (response?.Calculations is null)
            {
                return;
            }

            TurnoverCalculationsByArticle.Clear();
            TurnoverCalculationsByArticle.AddRange(response.Calculations);
        }
        else
        {
            var response = await ApiClient
                .WithRouteParts(BASE_ROUTE_PART, PAYMENTS_ROUTE_PART, BusinessUnitUid, LookupStartDate.ToString(DATE_FORMAT), LookupEndDate?.ToString(DATE_FORMAT))
                .Get<TurnoverCalculationsResponse>(cancellationToken);

            if (response?.Calculations is null)
            {
                return;
            }

            TurnoverCalculations.Clear();
            TurnoverCalculations.AddRange(response.Calculations);
        }
    }
}