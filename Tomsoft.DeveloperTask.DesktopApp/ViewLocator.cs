using System;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

namespace Tomsoft.DeveloperTask.DesktopApp;

public class ViewLocator : IDataTemplate
{
    public Control Build(object? data)
    {
        if (data is null)
        {
            return new TextBlock { Text = "data was null" };
        }

        var name = data.GetType().FullName!.Replace("ViewModel", "View");
        var type = Type.GetType(name);

        if (type != null)
        {
            return (Control)Activator.CreateInstance(type)!;
        }

        return new TextBlock { Text = "Not Found: " + name };
    }

    public bool Match(object? data)
    {
        return data is ViewModelBase;
    }
}