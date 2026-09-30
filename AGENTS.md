# AGENTS.md

Guidance for agents working in this repository. Read `docs/` for the full
explanation of anything below — these notes are the short version and the
gotchas only.

## Project

Desktop app for modelling computer-mouse / USB report-descriptor behaviour.
C# 14, .NET 10, Avalonia UI 12.1.3, MVVM (CommunityToolkit.Mvvm).

## Commands

The solution file is `src/MouseLabSolution.slnx` (new `.slnx` format) and lives
in `src/`, **not** at the repo root. There is no root-level `.sln`.

```bash
# build everything (must stay 0 warnings / 0 errors)
dotnet build src\MouseLabSolution.slnx

# build one project
dotnet build src\MouseLabAvaloniaApp\MouseLabAvaloniaApp.csproj

# run the app
dotnet run --project src\MouseLabAvaloniaApp\MouseLabAvaloniaApp.csproj

# NativeAOT publish — MUST produce zero IL2026/IL3050 warnings
dotnet publish src\MouseLabAvaloniaApp\MouseLabAvaloniaApp.csproj `
  -c Release -r win-x64 -p:PublishAot=true

# inspect the translation code ProTranslate.SourceGenerator emits
# (needs -t:Rebuild; an incremental build will not re-emit it)
dotnet build src\MouseLabAvaloniaApp\MouseLabAvaloniaApp.csproj -t:Rebuild `
  -p:EmitCompilerGeneratedFiles=true -p:CompilerGeneratedFilesOutputPath=obj\gen
```

There are **no tests, no CI, no pre-commit hooks, no `.editorconfig`, no
`Directory.Build.props`, no `Directory.Packages.props`**. Package versions are
pinned inline per csproj. `MouseLabAvaloniaApp` is `WinExe`, so `dotnet test`
and stdout-based verification do not apply — verify behaviour by running the
app or by a throwaway headless Avalonia harness in a temp directory.

## Project boundaries

Only **one** project reference exists today: `MouseLab.Services` → `MouseLab.Core`.
`MouseLabAvaloniaApp` has **no** `ProjectReference` to either. The domain
projects are not wired into the UI yet — do not assume they are.

- `MouseLab.Core` — value types (`Point`, `Size`, `Vector`), models (`IMatrix`),
  and the matrix factory interface `IMatrixProvider`. Note the factory interface
  lives here, not in `Services`.
- `MouseLab.Services` — operation interfaces (`IMatrixCutter`,
  `IMatrixRandomizer`). No implementations yet.
- `MouseLabAvaloniaApp` — all views, view models, DI, localization. This is the
  only UI project.

`IMatrixCutter` uses default interface methods: **3** overloads are abstract
(ending in `;`) and **6** are concrete defaults (with bodies) that forward to
them. Implementors supply only the 3 abstract ones.

## Localization — the rules that break silently

Full detail in `docs/localization.md`, `docs/architecture.md`,
`docs/troubleshooting.md`.

1. **Never use `{Translate ...}` or `{Format ...}` in XAML.** They are broken in
   Avalonia: the adapter raises `PropertyChanged` with the WPF indexer name
   `"Item[]"`, which Avalonia 12.1.3 ignores, so the text freezes at the startup
   language. They are also reflection-based and therefore AOT-unsafe. Use
   `{Binding Strings.SomeKey}` instead. This is verified, not theoretical.
2. **Translation catalogs are compiled into the assembly.** They are declared as
   `AdditionalFiles` in the app csproj and deliberately excluded from
   `AvaloniaResource`. They are not read at runtime, so editing a `.json`
   requires a rebuild.
3. **`Strings.*` is generated per-project.** `ProTranslateStrings` only exists in
   `MouseLabAvaloniaApp.dll`. If view models move to `MouseLab.Core` or
   `MouseLab.Services`, those projects need `ProTranslate.Core` +
   `SourceGenerator` + `Analyzers` and their own `AdditionalFiles` glob — but
   **not** `ProTranslate.Avalonia` (it would create a dependency cycle).
4. **JSON catalogs cannot contain comments.** `//` or `/* */` makes the
   generator emit `PTSG001` and emit *nothing*, so every `ProTranslateKeys` /
   `ProTranslateStrings` reference fails to compile. Conventions live in
   `src/MouseLabAvaloniaApp/Assets/Translations/README.md` instead.
5. **View-model-computed translated properties must be re-raised by hand.**
   `Strings.*` refresh themselves; `GreetingText => Translations.Format(...)`
   is a plain CLR property and goes stale unless `OnPropertyChanged` is called
   in `OnCultureChanged`. This is the most common regression.
6. **A missing key renders the key text** (e.g. `App.Greeting`) — that is the
   visible symptom of a typo or a missing entry in a catalog. Empty string is a
   valid translation, not a miss.
7. Every key must exist in **all** catalog files; a gap yields `PTA003` at build
   time and falls back to English at runtime.

## Startup wiring (`App.axaml.cs`) — order is load-bearing

- The `ServiceProvider` is a **field**, disposed in `desktop.Exit`. Using
  `using ServiceProvider` disposes every singleton (including the main window's
  `DataContext`) the moment `OnFrameworkInitializationCompleted` returns. This
  bug shipped once; don't reintroduce it.
- `UseProTranslateAvalonia()` **must** be called after `BuildServiceProvider()`
  and before any view is constructed. Skipping it leaves the adapter pointing at
  an empty default provider.
- `IApplicationSettingsService` is registered as a pre-built instance because the
  culture must be known before DI exists.
- `App.axaml.cs` does **not** call `AvaloniaXamlLoader` for views; only
  `App.Initialize()` loads `App.axaml`.

## AOT constraints

`PublishAot=true` plus `<AotAssemblies>True</AotAssemblies>`, and
`AvaloniaUseCompiledBindingsByDefault=true`. The publish must stay warning-free.
Consequences already enforced in code:

- `System.Text.Json` must go through the generated `AppSettingsJsonContext`;
  the reflection-based `Serialize`/`Deserialize` overloads are `IL2026`/`IL3050`
  and would silently break settings in an AOT binary.
- `Themes` is annotated `[JsonConverter(typeof(JsonStringEnumConverter<Themes>))]`
  — the **generic** converter. The non-generic one needs runtime codegen (`IL3050`).
- `ViewLocator` uses a plain switch. Do not restore the template's
  `Type.GetType` + `Activator.CreateInstance` name lookup; the trimmer removes it
  and the view silently fails to resolve. Add one case per new secondary view.

## Conventions

- **Code comments in `MouseLabAvaloniaApp` are written in Russian.** This was an
  explicit project decision. Match it; keep comments explaining *why*, not what.
- **Namespace style is inconsistent by design of history:** `MouseLabAvaloniaApp`
  uses file-scoped namespaces, `MouseLab.Core` and `MouseLab.Services` use
  block-scoped. Follow the file you are editing rather than "fixing" it.
- C# 14 features are in use: the `field` keyword in
  `Services/AppSettings/ApplicationSettingsService.cs`, collection expressions
  in `MainWindowViewModel.cs`.
- `ViewLocator` is registered in `App.axaml` `DataTemplates` but has no mappings
  yet; `MainWindowViewModel` is deliberately excluded because the main window is
  assigned directly.
- Settings persist to `%LOCALAPPDATA%\MouseLab\settings.json`. `AppSettingsStore`
  swallows all exceptions by design so a corrupt file cannot block startup — a
  real format bug will therefore be silent.
- The theme setting is persisted and applied, but there is no theme picker in the
  UI yet (`Strings.Settings.Theme` exists unused).
