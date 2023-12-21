using Tomsoft.DeveloperTask.DesktopApp.Models;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

public class PageViewModelBase : ViewModelBase
{
    public bool WasInitialized { get; set; }
    
    public Page Page { get; }

    public PageViewModelBase(Page page)
    {
        Page = page;
    }
    
    public virtual void Initialize() {}
    
    public virtual void BeforeNavigation(object? pageData = null) {}
}