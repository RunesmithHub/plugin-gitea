# Contributing

Thanks for helping. For a bug, open an issue with the steps to reproduce it; for a security problem, follow [SECURITY.md](SECURITY.md)
instead. For a larger change, open an issue first so the approach is agreed on before you write the code.

## Build and test

You need the .NET SDK that `global.json` names. Runesmith's packages come from the plugin hub's feed, which `nuget.config` sets up.

```bash
dotnet build plugin-gitea.slnx -warnaserror
dotnet format plugin-gitea.slnx --verify-no-changes
dotnet test --solution plugin-gitea.slnx
```

Restoring updates the `packages.lock.json` files when a dependency changes; commit them with the change.

## Check the plugin as the hub does

Install the hub tool from the plugin hub's feed, which this repository's `nuget.config` names, and run its check. It builds, packages and
analyses the plugin the way the hub does before a release:

```bash
dotnet tool install --global RunesmithHub.Tool --version 0.1.8
rshub check .
```

## Commits

Write each commit message as a [Conventional Commit](https://www.conventionalcommits.org/en/v1.0.0/), such as
`fix: keep the selection after a refresh`. The types are `feat`, `fix`, `perf`, `refactor`, `test`, `docs`, `ci`, `chore` and
`revert`.

## License

The plugin is licensed under the [Apache License 2.0](LICENSE). Contributions you submit are licensed under the same terms, as section 5 of
the license describes.
