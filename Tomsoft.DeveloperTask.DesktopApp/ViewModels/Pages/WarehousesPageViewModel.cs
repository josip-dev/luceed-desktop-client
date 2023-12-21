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
using Tomsoft.DeveloperTask.Data.Models.Warehouses;
using Tomsoft.DeveloperTask.Data.Models.Warehouses.List;
using Tomsoft.DeveloperTask.DesktopApp.Models;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels.Pages;

public class WarehousesPageViewModel : PageViewModelBase
{
    public ObservableCollection<Warehouse> Warehouses { get; } = new();

    [ObservableAsProperty] public bool AnyWarehousesPresent { get; }

    public ReactiveCommand<Unit, Unit> FetchWarehousesCommand { get; }
    public ReactiveCommand<Warehouse, Unit> ViewTurnoverCalculationsForArticlesCommand { get; }
    public ReactiveCommand<Warehouse, Unit> ViewTurnoverCalculationsForPaymentsCommand { get; }
    
    public WarehousesPageViewModel() : base(Page.Warehouses)
    {
        FetchWarehousesCommand = ReactiveCommand.CreateFromTask(FetchWarehouses);
        ViewTurnoverCalculationsForArticlesCommand = ReactiveCommand.Create<Warehouse, Unit>(warehouse =>
            ChangePage(Page.TurnoverCalculations, new TurnoverCalculationsPageViewModel.PageParameters(warehouse, true))
        );
        ViewTurnoverCalculationsForPaymentsCommand = ReactiveCommand.Create<Warehouse, Unit>(warehouse =>
            ChangePage(Page.TurnoverCalculations, new TurnoverCalculationsPageViewModel.PageParameters(warehouse, false))
        );

        Warehouses.ToObservableChangeSet(x => x)
            .ToCollection()
            .Select(items => items.Any())
            .ToPropertyEx(this, x => x.AnyWarehousesPresent);
    }

    public override void Initialize()
    {
        FetchWarehousesCommand.Execute().Subscribe();
    }

    private async Task FetchWarehouses(CancellationToken cancellationToken)
    {
        var response = await ApiClient
            .WithRouteParts("skladista", "lista")
            .Get<WarehouseListResponse>(cancellationToken);

        if (response is null)
        {
            return;
        }

        Warehouses.Clear();
        Warehouses.AddRange(response.Warehouses);
    }
}