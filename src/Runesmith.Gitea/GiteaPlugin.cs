using System.Composition;
using HammerUI.Services;
using Runesmith.Sdk.Plugins;
using Runesmith.Sdk.Settings;
using Runesmith.Sdk.Shell;
using Runesmith.Sdk.VersionControl;

namespace Runesmith.Gitea;

/// <summary>What sets Gitea apart: gitea.com as its default server, and Gitea servers, which answer <c>/api/v1/version</c> but not
/// Forgejo's version endpoint. Its accounts and tokens stay where the Gitea plugin kept them.</summary>
internal static class GiteaService
{
    /// <summary>The environment variable that replaces every Gitea server with made-up answers and accounts, for tests and screenshots only:
    /// <c>signed-in</c> or <c>signed-out</c>. Nothing reaches a server while it is set.</summary>
    public const string FakeVariable = "RUNESMITH_GITEA_FAKE";

    public static GiteaFlavor Flavor { get; } = new("gitea", "Gitea", GiteaIcons.Gitea, ServerKind.Gitea, GiteaServer.Of("gitea.com"), "gitea.com", FakeVariable);
}

/// <summary>What sets Forgejo apart: Codeberg as its default server, and Forgejo servers, which answer <c>/api/forgejo/v1/version</c>.
/// Its accounts are kept apart from Gitea's, and the list the Forgejo plugin kept is taken over once.</summary>
internal static class ForgejoService
{
    /// <summary>The environment variable that replaces every Forgejo server with made-up answers and accounts, for tests and screenshots
    /// only: <c>signed-in</c> or <c>signed-out</c>. Nothing reaches a server while it is set.</summary>
    public const string FakeVariable = "RUNESMITH_FORGEJO_FAKE";

    public static GiteaFlavor Flavor { get; } = new("forgejo", "Forgejo", GiteaIcons.Forgejo, ServerKind.Forgejo, GiteaServer.Of("codeberg.org"), "Codeberg", FakeVariable,
        Section: "forgejo", FormerPluginId: "runesmith.forgejo");
}

/// <summary>The Gitea accounts, for the clone dialog, the Pull Requests window and links.</summary>
[Export(typeof(IRepositoryHostProvider))]
[Export]
[Shared]
[method: ImportingConstructor]
internal sealed class GiteaProvider(
    [Import(AllowDefault = true)] ISecretStore? secrets,
    [Import(AllowDefault = true)] ISettingsService? settings,
    [Import(AllowDefault = true)] INotificationService? notifications,
    [Import(AllowDefault = true)] ILauncher? launcher,
    [Import(AllowDefault = true)] IDialogService? dialogs)
    : GiteaPluginProvider(GiteaService.Flavor, secrets, settings, notifications, launcher, dialogs);

/// <summary>The Forgejo accounts, for the clone dialog, the Pull Requests window and links.</summary>
[Export(typeof(IRepositoryHostProvider))]
[Export]
[Shared]
[method: ImportingConstructor]
internal sealed class ForgejoProvider(
    [Import(AllowDefault = true)] ISecretStore? secrets,
    [Import(AllowDefault = true)] ISettingsService? settings,
    [Import(AllowDefault = true)] INotificationService? notifications,
    [Import(AllowDefault = true)] ILauncher? launcher,
    [Import(AllowDefault = true)] IDialogService? dialogs)
    : GiteaPluginProvider(ForgejoService.Flavor, secrets, settings, notifications, launcher, dialogs);

/// <summary>Registers the icons and reads the tokens of the Forgejo and Gitea accounts when Runesmith starts.</summary>
[Export(typeof(IPlugin))]
[method: ImportingConstructor]
internal sealed class GiteaPlugin(GiteaProvider gitea, ForgejoProvider forgejo) : IPlugin
{
    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        GiteaIcons.Register();
        foreach (var provider in new GiteaHostProvider[] { gitea, forgejo })
        {
            try
            {
                await provider.LoadAsync(cancellationToken).ConfigureAwait(true);
            }
            catch (Exception exception) when (exception is IOException or InvalidOperationException)
            {
            }
        }
    }

    public void Dispose()
    {
    }
}

/// <summary>Gives Git the token of a signed-in Gitea account for its server's HTTPS remotes.</summary>
[Export(typeof(IGitCredentialSource))]
[method: ImportingConstructor]
internal sealed class GiteaCredentials(GiteaProvider provider) : GiteaPluginCredentials(provider);

/// <summary>Gives Git the token of a signed-in Forgejo account for its server's HTTPS remotes.</summary>
[Export(typeof(IGitCredentialSource))]
[method: ImportingConstructor]
internal sealed class ForgejoCredentials(ForgejoProvider provider) : GiteaPluginCredentials(provider);

/// <summary>Lists the Gitea accounts at the top of Settings › Gitea.</summary>
[Export(typeof(ISettingsSectionProvider))]
[method: ImportingConstructor]
internal sealed class GiteaSettingsSection(GiteaProvider provider) : GiteaPluginSection(provider);

/// <summary>Lists the Forgejo accounts at the top of Settings › Forgejo.</summary>
[Export(typeof(ISettingsSectionProvider))]
[method: ImportingConstructor]
internal sealed class ForgejoSettingsSection(ForgejoProvider provider) : GiteaPluginSection(provider);

/// <summary>The Gitea settings: the OAuth2 client IDs per server.</summary>
[Export(typeof(ISettingContributor))]
internal sealed class GiteaSettings() : GiteaPluginSettings(GiteaService.Flavor);

/// <summary>The Forgejo settings: the OAuth2 client IDs per server.</summary>
[Export(typeof(ISettingContributor))]
internal sealed class ForgejoSettings() : GiteaPluginSettings(ForgejoService.Flavor);
