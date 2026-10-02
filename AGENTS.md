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
8. **A translated `ComboBox` item needs an `ItemTemplate`.** Without one Avalonia
   renders the item via `ToString()`, which is a one-time string snapshot, so
   labels freeze at the startup language until the window is recreated. Bind
   `Text="{Binding DisplayName.Value}"` against the `IObservableLocalizedString`
   from `Strings.Observe_*()`. Related trap: `nameof(SelectedTheme.DisplayName)`
   evaluates to plain `"DisplayName"` and raises `PropertyChanged` on the *view
   model*, not on the option — it can never refresh a list item.
9. **`ProTranslateStrings.Dispose()` does not dispose what `Observe_*()` handed
   out.** It only unsubscribes itself from `CultureChanged`. Whoever created the
   observable strings owns them; `ThemeOption` is `IDisposable` for that reason.

## Settings / theme flow

- Language and theme are applied **by `SaveSettingsCommand`**, not by the
  `Selected*` setters. The setters only `SetProperty`. To switch instantly,
  uncomment the two lines in `AppSettingsViewModel.SelectedCulture`.
- Theme reaches the UI via `IApplicationSettingsService.ThemeChanged` →
  `App.ApplyTheme` → `RequestedThemeVariant`. Nothing else applies it.
- `WindowsManagerService.ShowSettingsAsync` checks the open-window list **before**
  resolving anything, and disposes the view model in `window.Closed`. Keep both:
  the VM subscribes to a singleton's `CultureChanged`, so a missed unsubscribe
  pins it for the rest of the session. `ViewModelBase.Dispose` is `_disposed`
  guarded, so the DI container disposing the same transients again is safe.
- `ShowSettingsAsync` is deliberately **not** `async`. Re-enabling the modal
  `ShowDialog` path requires moving `openedWindows.Add`/`Show()` *after* the
  `await` — see the comment in the method.

## Startup wiring — order is load-bearing

- DI registration lives in **`AppServices`**, not in `App`. `App` only
  orchestrates: create `AppServices.CreateDefault()`, apply theme, build the main
  window, wire `desktop.Exit`. Don't move registrations back into `App`.
- `AppSettingsStore.Load()` + `ResolveCulture` happen **inside
  `AppServices.CreateDefault()`**, before the container is built. The culture
  must be known before DI exists, so `IApplicationSettingsService` is registered
  from the already-read snapshot.
- `UseProTranslateAvalonia()` lives in the same private method as
  `BuildServiceProvider()` so the "build first, then attach the adapter" order
  can't be broken from outside. Skipping it leaves the adapter pointing at an
  empty default provider.
- The `ServiceProvider` is a **field** of `AppServices`, disposed by
  `App.Shutdown()` on `desktop.Exit`. Using `using ServiceProvider` disposes every
  singleton (including the main window's `DataContext`) the moment
  `OnFrameworkInitializationCompleted` returns. This bug shipped once; don't
  reintroduce it.
- **`AppServices` has no finalizer, on purpose.** `App` owns it and disposes it
  deterministically. A finalizer would run `ServiceProvider.Dispose()` on the
  finalizer thread at an arbitrary moment, and an exception there kills the
  process without a usable stack.
- `App.axaml.cs` does **not** call `AvaloniaXamlLoader` for views; only
  `App.Initialize()` loads `App.axaml`.

## Designer support (`AppServices.Instance`)

- `#if DEBUG` only. Views resolve their design-time VM in code-behind with
  `Design.SetDataContext(this, AppServices.Instance.Provider.GetRequiredService<...>())`.
- **Never** put a view model in `<Design.DataContext>` in XAML. It needs ctor
  dependencies, so it cannot be instantiated, and the XAML form wins over
  `Design.SetDataContext`, making the code-behind path dead. This broke
  `MainWindow` before.
- `Instance` **must** stay cached in a static field. Without the cache every
  access built a new container, re-read `settings.json`, and re-set ProTranslate's
  process-wide binding source.
- The designer container is deliberately **not** the runtime container: it has
  its own `IApplicationSettingsService`, which `App` is not subscribed to, so
  theme switching in the preview never reaches `App.ApplyTheme`. Don't share it.
- Do not dispose it — it dies with the previewer process.

## AOT constraints

`PublishAot=true` plus `<AotAssemblies>True</AotAssemblies>`, and
`AvaloniaUseCompiledBindingsByDefault=true`. The publish must stay warning-free.
Trim/AOT analyzers are enabled app-wide (`IsTrimmable`, `EnableTrimAnalyzer`,
`EnableAotAnalyzer`), so a plain `dotnet build` already catches most of this.

Consequences already enforced in code:

- `System.Text.Json` must go through the generated `AppSettingsJsonContext`;
  the reflection-based `Serialize`/`Deserialize` overloads are `IL2026`/`IL3050`
  and would silently break settings in an AOT binary.
- `Themes` is annotated `[JsonConverter(typeof(JsonStringEnumConverter<Themes>))]`
  — the **generic** converter. The non-generic one needs runtime codegen (`IL3050`).
- **`ViewLocator` is generated by `StaticViewLocator` (0.7.0).** It resolves views
  through `static Dictionary<Type, Func<Control>>`, so `Build`/`Match` are
  generated — never write them by hand or you bypass the table. Keep
  `GenerateRuntimeTypeFallbackMethods = false` (its base/interface walk is
  reflection again). Add new pairs with `[StaticViewMapping]`; **never** by
  computed type name, because `Type.GetType(name)` + `Activator.CreateInstance`
  produces **no build warning** (the name is computed, so the trimmer removes the
  constructor) and fails only at runtime.
- **Never enable `InvariantGlobalization`.** It's a common AOT size tip, and it
  breaks this app outright: `ru-RU` degrades to invariant and translation
  lookup plus fallback stop working.
- No referenced package ships `ILLink.Descriptors.xml` — Avalonia relies on
  `[DynamicallyAccessedMembers]` inside its own assemblies. So a clean publish is
  Avalonia's annotations doing the work: **re-run the publish after any Avalonia
  upgrade**, a new version can introduce warnings on its own.
- Analyzers cannot see: computed type names, XAML bindings without `x:DataType`,
  and runtime-only AOT failures (FluentTheme resources, `WithInterFont()`,
  ProTranslate's static binding source). Publish once and actually launch the exe.
- `MouseLab.Core` and `MouseLab.Services` carry `IsTrimmable` so the analyzers
  cover them the day a `ProjectReference` appears.

## Conventions

- **Code comments in `MouseLabAvaloniaApp` are written in Russian.** This was an
  explicit project decision. Match it; keep comments explaining *why*, not what.
- **Namespace style is inconsistent by design of history:** `MouseLabAvaloniaApp`
  uses file-scoped namespaces, `MouseLab.Core` and `MouseLab.Services` use
  block-scoped. Follow the file you are editing rather than "fixing" it.
- C# 14 features are in use: the `field` keyword in
  `Services/AppSettings/ApplicationSettingsService.cs`, collection expressions
  in `MainWindowViewModel.cs`.
- `ViewLocator` is registered in `App.axaml` `DataTemplates`; its mappings come
  from `StaticViewLocator` (`AppSettingsViewModel` and `SettingsWindowViewModel`
  via `[StaticViewMapping]`). `MainWindowViewModel` is deliberately excluded
  because the main window is assigned directly as `desktop.MainWindow`.
  Nothing currently routes a view model through a `ContentControl`, so the
  locator is forward-looking, not load-bearing.
- Settings persist to `%LOCALAPPDATA%\MouseLab\settings.json`. `AppSettingsStore`
  swallows all exceptions by design so a corrupt file cannot block startup — a
  real format bug will therefore be silent.
- The theme setting is persisted and applied, but there is no theme picker in the
  UI yet (`Strings.Settings.Theme` exists unused).
