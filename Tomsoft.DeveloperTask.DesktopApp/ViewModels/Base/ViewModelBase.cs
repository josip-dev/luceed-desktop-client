using System;
using System.Reactive;
using ReactiveUI;
using Tomsoft.DeveloperTask.Data;
using Tomsoft.DeveloperTask.DesktopApp.Models;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

public class ViewModelBase : ReactiveObject
{
    protected static StateViewModel State => MainWindowViewModel.State;

    protected static LuceedApiClient ApiClient
    {
        get
        {
            if (State.ApiClient is null)
            {
                throw new NullReferenceException($"API client wasn't set up yet...");
            }

            return State.ApiClient;
        }
    }

    protected static Unit ChangePage(Page page, object? data = null)
    {
        State.ChangePage(page, data);
        return Unit.Default;
    }
}