namespace Runesmith.Gitea.Tests;

public sealed class ServiceTests : IDisposable
{
    private readonly string state = Directory.CreateTempSubdirectory("runesmith-gitea-state-").FullName;

    public void Dispose() => Directory.Delete(state, recursive: true);

    [Fact]
    public void GiteaStartsWithGiteaCom()
    {
        var flavor = GiteaService.Flavor;

        Assert.Equal("Gitea", flavor.Name);
        Assert.Equal("gitea.com", flavor.DefaultServer.Key);
        Assert.Equal("gitea.com", flavor.ServerName(flavor.DefaultServer));
        Assert.Equal("Gitea (git.example.com)", flavor.ServerName(GiteaServer.Of("git.example.com")));
        Assert.Equal("gitea.oauthClientIds", flavor.ClientIdsSetting);
    }

    [Fact]
    public void ForgejoStartsWithCodeberg()
    {
        var flavor = ForgejoService.Flavor;

        Assert.Equal("Forgejo", flavor.Name);
        Assert.Equal("codeberg.org", flavor.DefaultServer.Key);
        Assert.Equal("Codeberg", flavor.ServerName(flavor.DefaultServer));
        Assert.Equal("Forgejo (git.example.com)", flavor.ServerName(GiteaServer.Of("git.example.com")));
        Assert.Equal("forgejo.oauthClientIds", flavor.ClientIdsSetting);
    }

    [Fact]
    public void EachServiceTakesItsOwnServersOnly()
    {
        Assert.True(GiteaService.Flavor.Accepts(ServerKind.Gitea));
        Assert.False(GiteaService.Flavor.Accepts(ServerKind.Forgejo));
        Assert.False(GiteaService.Flavor.Accepts(ServerKind.Unknown));
        Assert.True(ForgejoService.Flavor.Accepts(ServerKind.Forgejo));
        Assert.False(ForgejoService.Flavor.Accepts(ServerKind.Gitea));
        Assert.False(ForgejoService.Flavor.Accepts(ServerKind.Unknown));
    }

    [Fact]
    public void EachServiceOffersItsClientIdSettingInItsOwnCategory()
    {
        var gitea = Assert.Single(new GiteaSettings().Settings);
        var forgejo = Assert.Single(new ForgejoSettings().Settings);

        Assert.Equal(("gitea.oauthClientIds", "Gitea", ""), (gitea.Key, gitea.Category, gitea.DefaultValue));
        Assert.Equal(("forgejo.oauthClientIds", "Forgejo", ""), (forgejo.Key, forgejo.Category, forgejo.DefaultValue));
    }

    [Fact]
    public void GiteaKeepsItsSecretsAndAccountsWhereTheGiteaPluginDid()
    {
        var context = new GiteaContext(GiteaService.Flavor, new HttpClient(new FakeServer()), null, null, null, null);

        Assert.Equal("runesmith.gitea/gitea.com/alice", context.SecretKey(GiteaServer.Of("gitea.com"), "alice"));
        Assert.Equal(Path.Combine(state, "runesmith.gitea"), GiteaService.Flavor.FolderIn(state));
    }

    [Fact]
    public void ForgejoKeepsItsSecretsAndAccountsApartFromGiteas()
    {
        var context = new GiteaContext(ForgejoService.Flavor, new HttpClient(new FakeServer()), null, null, null, null);

        Assert.Equal("runesmith.gitea/forgejo/codeberg.org/alice", context.SecretKey(GiteaServer.Of("codeberg.org"), "alice"));
        Assert.Equal(Path.Combine(state, "runesmith.gitea", "forgejo"), ForgejoService.Flavor.FolderIn(state));
    }

    [Fact]
    public void ForgejoTakesOverTheAccountsOfTheForgejoPluginOnce()
    {
        new AccountStore(Path.Combine(state, "runesmith.forgejo", "accounts.json")).Save([new StoredAccount("codeberg.org", "alice", "Alice Liddell", null)]);

        var first = AccountStore.Open(ForgejoService.Flavor, state);
        first.Save([]);
        var second = AccountStore.Open(ForgejoService.Flavor, state);

        Assert.True(File.Exists(Path.Combine(state, "runesmith.gitea", "forgejo", "accounts.json")));
        Assert.Empty(second.Load());
    }

    [Fact]
    public void ForgejoStartsWithTheAccountsOfTheForgejoPlugin()
    {
        var account = new StoredAccount("codeberg.org", "alice", "Alice Liddell", null);
        new AccountStore(Path.Combine(state, "runesmith.forgejo", "accounts.json")).Save([account]);

        Assert.Equal(account, Assert.Single(AccountStore.Open(ForgejoService.Flavor, state).Load()));
        Assert.Empty(AccountStore.Open(GiteaService.Flavor, state).Load());
    }
}
