using System.ComponentModel.DataAnnotations;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;
using Tomsoft.DeveloperTask.Data;
using Tomsoft.DeveloperTask.DesktopApp.Models;
using Tomsoft.DeveloperTask.DesktopApp.ViewModels.Base;

namespace Tomsoft.DeveloperTask.DesktopApp.ViewModels.Pages;

public class CredentialsPageViewModel : PageViewModelBase
{
    private string _username = string.Empty;
    [Required]
    public string Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private string _password = string.Empty;
    [Required]
    public string Password
    {
        get => _password;
        set => this.RaiseAndSetIfChanged(ref _password, value);
    }

    public ReactiveCommand<Unit, Unit> SignInCommand { get; }
    
    public CredentialsPageViewModel() : base(Page.Credentials)
    {
        SignInCommand = ReactiveCommand.CreateFromTask(SignIn, this.WhenAnyValue(
            x => x.Username,
            x => x.Password)
            .Select(x => !string.IsNullOrWhiteSpace(x.Item1) && !string.IsNullOrWhiteSpace(x.Item2)));
    }

    private async Task SignIn(CancellationToken cancellationToken)
    {
        var credentials = new LuceedApiCredentials(Username, Password);
        await credentials.Save(cancellationToken);
        if (await TestCredentials(credentials, cancellationToken))
        {
            ChangePage(Page.Articles);
        }
    }

    public static async Task<bool> TestCredentials(LuceedApiCredentials credentials, CancellationToken cancellationToken = default)
    {
        var apiClient = new LuceedApiClient(credentials);
        var canConnect = await apiClient.CanConnect(cancellationToken);
        if (!canConnect)
        {
            return false;
        }
        
        State.ApiClient = apiClient;
        State.Username = credentials.Username;
        return true;
    }
}