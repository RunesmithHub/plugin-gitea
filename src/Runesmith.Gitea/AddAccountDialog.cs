using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Threading;
using HammerUI.Controls;
using HammerUI.Services;
using Runesmith.Sdk.Shell;
using static Runesmith.Gitea.ViewHelpers;

namespace Runesmith.Gitea;

/// <summary>Asks for a server's address and checks through its version endpoints that it is a server the service takes. It closes with the
/// <see cref="GiteaServer"/>.</summary>
internal sealed class AddAccountDialog : Dialog, IDisposable
{
    // An address as an example, which reads the same in every language.
    private const string ExampleAddress = "git.example.com";

    private readonly GiteaContext context;
    private readonly TextBox address;
    private readonly TextBlock error = new() { TextWrapping = TextWrapping.Wrap, FontSize = 12, IsVisible = false };
    private readonly Button next = new() { Content = Strings.Continue, Classes = { "accent" }, MinWidth = 96, IsDefault = true };
    private CancellationTokenSource? checking;

    public AddAccountDialog(GiteaContext context)
    {
        this.context = context;
        var flavor = context.Flavor;
        Header = Strings.AddAccountTitle(flavor.Name);
        Description = Strings.AddAccountIntro(service: flavor.Name, example: flavor.DefaultServer.Key);
        Width = 480;
        ShowCloseButton = true;
        Brush(error, TextBlock.ForegroundProperty, "DangerBrush");

        address = new TextBox { Text = flavor.DefaultServer.Key, PlaceholderText = ExampleAddress };
        var mark = Icon(GiteaIcons.Get(GiteaIcons.Server), 16, "TextMutedBrush");
        mark.Margin = new Thickness(8, 0, 2, 0);
        address.InnerLeftContent = mark;
        var hint = Label(Strings.AddAccountHint, "TextMutedBrush", 12, wrap: true);
        Content = new StackPanel { Spacing = 8, Margin = new Thickness(0, 4, 0, 0), Children = { Label(Strings.ServerLabel, weight: FontWeight.Medium), address, hint, error } };

        var cancel = new Button { Content = Strings.Cancel, MinWidth = 84, IsCancel = true };
        cancel.Click += (_, _) => Close();
        next.Click += (_, _) => _ = CheckAsync();
        Footer = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 8, HorizontalAlignment = HorizontalAlignment.Right, Children = { cancel, next } };

        address.TextChanged += (_, _) => error.IsVisible = false;
        address.KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter)
            {
                e.Handled = true;
                _ = CheckAsync();
            }
        };
        address.AttachedToVisualTree += (_, _) => Dispatcher.UIThread.Post(() =>
        {
            address.Focus();
            address.SelectAll();
        }, DispatcherPriority.Background);
        DetachedFromVisualTree += (_, _) => Dispose();
    }

    protected override Type StyleKeyOverride => typeof(Dialog);

    public void Dispose()
    {
        checking?.Cancel();
        checking?.Dispose();
        checking = null;
    }

    /// <summary>Gets the message for a server of a kind the service does not take, or null when it takes it.</summary>
    internal static string? Problem(GiteaFlavor flavor, GiteaServer server, ServerKind kind) => kind switch
    {
        _ when flavor.Accepts(kind) => null,
        ServerKind.Forgejo => Strings.ServerRunsForgejo(server.Key),
        ServerKind.Gitea => Strings.ServerRunsGitea(server.Key),
        _ => Strings.ServerNotFound(service: flavor.Name, server: server.Key),
    };

    private async Task CheckAsync()
    {
        if (GiteaServer.Parse(address.Text) is not { } server)
        {
            ShowError(Strings.AddressInvalid);
            return;
        }

        checking?.Cancel();
        checking = new CancellationTokenSource();
        var token = checking.Token;
        next.IsEnabled = false;
        next.Content = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 6, Children = { Spinner(12), new TextBlock { Text = Strings.Checking } } };
        try
        {
            var info = await ServerProbe.ProbeAsync(context.Http, server, token);
            if (Problem(context.Flavor, server, info.Kind) is { } problem)
                ShowError(problem);
            else
                Close(server);
        }
        catch (OperationCanceledException) when (token.IsCancellationRequested)
        {
        }
        catch (GiteaException exception)
        {
            ShowError(exception.Message);
        }
        finally
        {
            next.IsEnabled = true;
            next.Content = Strings.Continue;
        }
    }

    private void ShowError(string message)
    {
        error.Text = message;
        error.IsVisible = true;
    }
}

/// <summary>Shows the plugin's dialogs with the window's dialog service, for the accounts and the provider.</summary>
internal sealed class GiteaPrompts(GiteaHostProvider provider, IDialogService? dialogs) : ISignInPrompt, IAddAccountPrompt
{
    public async Task<bool> SignInAsync(GiteaHost host, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(host);
        if (dialogs is null)
            return false;

        var user = await Dispatcher.UIThread.InvokeAsync(() => dialogs.ShowAsync(new SignInDialog(host))) as GiteaUser;
        if (user is null)
            return host.IsSignedIn;

        provider.Context.Notifications?.Notify(NotificationKind.Success, Strings.SignedInAs(service: host.Name, account: user.Login));
        return true;
    }

    public async Task<GiteaServer?> AskServerAsync(CancellationToken cancellationToken) =>
        dialogs is null ? null : await Dispatcher.UIThread.InvokeAsync(() => dialogs.ShowAsync(new AddAccountDialog(provider.Context))) as GiteaServer;

    /// <summary>Asks, then signs an account out and deletes its tokens; it stays in the list, signed out.</summary>
    public async Task SignOutAsync(GiteaHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        var confirmed = dialogs is null || await dialogs.ConfirmAsync(Strings.SignOutTitle(host.Name),
            Strings.SignOutMessage(account: host.Login ?? "", server: host.Server.Key), Strings.SignOut, isDestructive: true);
        if (confirmed)
            await host.SignOutAsync();
    }

    /// <summary>Asks, then signs an account out and takes it off the list.</summary>
    public async Task RemoveAsync(GiteaHost host)
    {
        ArgumentNullException.ThrowIfNull(host);
        var confirmed = dialogs is null || await dialogs.ConfirmAsync(Strings.RemoveAccountTitle(host.Login ?? host.Name),
            Strings.RemoveAccountMessage(host.Server.Key), Strings.Remove, isDestructive: true);
        if (confirmed)
            await provider.RemoveAsync(host);
    }
}
