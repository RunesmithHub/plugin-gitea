# Gitea and Forgejo for Runesmith

The official Gitea and Forgejo plugin for [Runesmith](https://github.com/RunesmithHub/Runesmith), `runesmith.gitea`. It signs you in to
[gitea.com](https://gitea.com), [Codeberg](https://codeberg.org) and any self-hosted Gitea or Forgejo server, through the browser or with
an access token. Through the [Git plugin](https://github.com/RunesmithHub/plugin-git) it adds a tab for each of your accounts to the clone
dialog, lists the open repository's pull requests, and points **Open on** and **Copy Link** at the server's pages.

Gitea and Forgejo each have their own accounts and settings, in **Settings › Gitea** and **Settings › Forgejo**: a server is added under the
service it runs. The [Gitea](https://runesmithhub.github.io/Runesmith/plugins/gitea) and
[Forgejo](https://runesmithhub.github.io/Runesmith/plugins/forgejo) documentation describes signing in, registering an OAuth2 application
and the settings.

## Capabilities

| Capability | Why |
| --- | --- |
| `network` | Talks to the Gitea and Forgejo servers you add to sign you in and list repositories. |
| `credentials` | Keeps your Gitea and Forgejo sign-ins in your system's secret store. |
| `process` | Opens your Gitea or Forgejo server in your browser to sign in. |
| `filesystem` | Remembers the accounts you added and caches avatars. |
| `environment` | Reads a variable that points it at a test server during development. |

## Install

Install it from Runesmith's plugin manager, or from [hub.runesmith.dev](https://hub.runesmith.dev), together with the Git plugin, which
shows what this plugin provides.

## Build and test

You need the .NET SDK that `global.json` names. Runesmith's packages come from the plugin hub's feed, which `nuget.config` sets up.

```bash
dotnet build plugin-gitea.slnx
dotnet test --solution plugin-gitea.slnx
dotnet build src/Runesmith.Gitea -t:InstallPlugin
```

`InstallPlugin` copies the built plugin into your Runesmith plugins folder; restart Runesmith to load it. [CONTRIBUTING.md](CONTRIBUTING.md)
has the checks a change must pass, and the [Runesmith documentation](https://runesmithhub.github.io/Runesmith/) covers writing plugins.

## License

[Apache License 2.0](LICENSE).
