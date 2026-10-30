# Graph Report - MouseWorkLab  (2026-10-30)

## Corpus Check
- 215 files · ~235,026 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 65 file(s) not represented in the graph (top: .csv 53, .axaml 7, (none) 2)

## Summary
- 2474 nodes · 4266 edges · 178 communities (114 shown, 64 thin omitted)
- Extraction: 91% EXTRACTED · 9% INFERRED · 0% AMBIGUOUS · INFERRED: 390 edges (avg confidence: 0.86)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- MoveFinder Benchmarks
- BM25 Search Core
- shadcn Installer Tests
- Design Tokens JSON Values
- Move Finder Manager
- Avalonia Dependencies
- Theme Token Values
- BM25 Context Formatting
- Console Formatting Tests
- Design Image Generation
- MouseLab Namespaces
- Design Catalog Validation
- App Settings Events
- Cutter Benchmarks
- Project Namespaces and Tests
- Design Token Values
- Brand Color Utilities
- Avalonia Views and Dock
- Design Diagnostics Tests
- App Settings View Models
- Slides Sub-skill
- Matrix Pooling
- CSS Validation Rules
- Tailwind Config Tests
- Matrix Randomizer Benchmarks
- Welcome View Model
- Geometry and Cutter Tests
- Accessibility Guidance Tests
- Design Token Colors
- Design System Generator
- Brand Guidelines Workflow
- Catalog Contract Tests
- Tailwind Config Generator
- Icon Generation CLI
- Brand Token Sync Tests
- Slide Generation Scripts
- Window Manager Services
- Brand Color Sync
- CIP Search and Briefs
- Dock Layout and Main View Model
- Color Mode Resolution
- SAD Providers
- App Startup and Theme
- Matrix Providers and Tests
- Background Image Utilities
- Design Token Sizes
- Brand Compliance Review
- Slide Decision System
- Matrix and Providers
- Banner Typography Rules
- Design System Skill
- UI Styling Skill
- App Services and DI
- Application Settings Persistence
- Asset Manifest CLI
- Brand Voice Review
- CIP Generation CLI
- Design Token Values
- Disposal Bug Analysis
- Domain Search CLI
- CIP Domain Search
- Reasoning Rules
- WCAG and Palette Tests
- Catalog Refresh Tests
- User Identity and Profile
- Banner and Social Specs
- Settings Handoff Rules
- Dock View Models
- Web Stack Freshness Tests
- Logo and CIP Workflow
- Button Component Specs
- Input and Card Tokens
- Palette Selection Tests
- Domain Detection Tests
- Search Domain Tests
- HTML Rendering CLI
- Text Layout Tests
- File Scanning CLI
- Design Tokens Starter
- Test Matrix Types
- Test Matrix Memory
- Banner Design Workflow
- CIP Prompt Structure
- AOT Analyzer Constraints
- Translation Binding Rules
- Brand Context Extraction
- Token Embedding Script
- Native Desktop Tests
- Style Taxonomy Tests
- Design Skill Routing
- Slides Skill
- Localization Architecture Docs
- Theme Options
- Token Export CLI
- Design Token Speeds
- SIMD Fill Operations
- Color Psychology Reference
- BM25 Algorithm
- BM25 Algorithm Copy
- Generated Config Tests
- Settings Service Base
- CIP Deliverables
- Globalization and Disposal
- View Locator
- CIP Art Direction Styles
- Design Sub-skill Routing
- Icon Design Reference
- Slide HTML and Charts
- Theme Change Flow
- Relevance Threshold Tests
- Startup Wiring
- Settings Storage Format
- Designer Container Rules
- Project Boundaries
- ProTranslate Dependencies
- Translation Key Workflow
- Style Identity Tests
- XAML Translation Bugs
- Avalonia Program Entry
- Design Token sm Values
- BM25 Core Tests
- Matrix Cutter Internals
- Banner Safe Zones
- Motion and Layout Tokens
- Design Token xl Values
- Design Token None Values
- Test Fixture Validation
- WCAG Contrast Helpers
- Benchmark Scenarios
- Typography Spacing Rules
- Dialog Component Tokens
- Compiled Translation Catalogs
- JSON Catalog Diagnostics
- Adding a New Language
- opencode Plugin Config
- opencode Plugin Schema
- Destructive Foreground Token
- Primary Token
- Primary Foreground Token
- Secondary Foreground Token
- Print Specifications
- Imagery Guidelines
- Alert Component Tokens
- Badge Component Tokens
- Card Component Tokens
- Table Component Tokens
- Spacing Scale
- Typography Scale
- Token Naming Conventions
- Token Generator Script
- Slide Background Treatments
- NativeAOT Requirements
- AOT Image Size
- Tailwind CSS Variables
- UI Styling Test Requirements

## God Nodes (most connected - your core abstractions)
1. `TailwindConfigGenerator` - 58 edges
2. `IMatrix` - 47 edges
3. `TestTailwindConfigGenerator` - 35 edges
4. `DesignSystemGenerator` - 35 edges
5. `ShadcnInstaller` - 34 edges
6. `WelcomeWindowViewModel` - 34 edges
7. `PooledMatrix` - 31 edges
8. `Vector` - 28 edges
9. `Size` - 27 edges
10. `TestShadcnInstaller` - 26 edges

## Surprising Connections (you probably didn't know these)
- `Theme Picker Missing From UI` --semantically_similar_to--> `Theme Setting Persisted But No UI Picker Yet`  [INFERRED] [semantically similar]
  docs/README.md → AGENTS.md
- `Color Palette Section (primary/secondary/neutral + WCAG)` --shares_data_with--> `Color Hierarchy (primary/secondary/neutral/semantic)`  [INFERRED]
  .opencode/skills/brand/references/brand-guideline-template.md → .opencode/skills/brand/references/color-palette-management.md
- `Type Scale (Major Third 1.25, 16px base)` --semantically_similar_to--> `Typography Section (font stack, type scale)`  [INFERRED] [semantically similar]
  .opencode/skills/brand/references/typography-specifications.md → .opencode/skills/brand/references/brand-guideline-template.md
- `Color Hierarchy (primary/secondary/neutral/semantic)` --shares_data_with--> `Starter Semantic Colors (success/warning/error/info)`  [INFERRED]
  .opencode/skills/brand/references/color-palette-management.md → .opencode/skills/brand/templates/brand-guidelines-starter.md
- `Type Scale (Major Third 1.25, 16px base)` --semantically_similar_to--> `Core Visual Elements (logo/color/type)`  [INFERRED] [semantically similar]
  .opencode/skills/brand/references/typography-specifications.md → .opencode/skills/brand/references/visual-identity.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Banner Design Pipeline (research to exported variants)** — _opencode_skills_banner_design_skill_banner_workflow, _opencode_skills_banner_design_references_banner_sizes_and_styles_pinterest_research_queries, _opencode_skills_banner_design_references_banner_sizes_and_styles_art_direction_styles, _opencode_skills_banner_design_skill_model_selection, _opencode_skills_banner_design_skill_html_css_composition, _opencode_skills_banner_design_skill_png_export, _opencode_skills_banner_design_skill_output_path_convention [EXTRACTED 1.00]
- **Brand Token Sync Chain (guidelines to tokens)** — _opencode_skills_brand_skill_brand_guidelines_source_of_truth, opencode_skills_brand_scripts_sync_brand_to_tokens, _opencode_skills_brand_skill_design_tokens, _opencode_skills_brand_references_update_sync_to_design_tokens, _opencode_skills_brand_references_update_verify_sync, _opencode_skills_brand_references_update_always_sync_all_three_files, _opencode_skills_brand_references_brand_guideline_template_extractable_fields [EXTRACTED 1.00]
- **Brand Compliance Review Family (visual, voice, technical, legal)** — _opencode_skills_brand_references_approval_checklist_assetapprovalchecklist, _opencode_skills_brand_references_approval_checklist_logo_usage_review, _opencode_skills_brand_references_approval_checklist_color_compliance_review, _opencode_skills_brand_references_approval_checklist_accessibility_review, _opencode_skills_brand_references_approval_checklist_technical_requirements_review, _opencode_skills_brand_references_approval_checklist_legal_compliance_review, _opencode_skills_brand_references_consistency_checklist_brandconsistencychecklist, _opencode_skills_brand_references_logo_usage_rules_logousagerules, _opencode_skills_brand_references_color_palette_management_wcag_contrast_ratios [INFERRED 0.85]
- **Slide generation pipeline (BM25 search → decision CSVs → strategy/layout/copy → tokenized HTML + validation)** — _opencode_skills_design_system_skill_slide_search_bm25, _opencode_skills_design_system_skill_contextual_decision_flow, _opencode_skills_slides_references_slide_strategies_deck_structure_catalog, _opencode_skills_slides_references_layout_patterns_layout_selection_by_use_case, _opencode_skills_slides_references_copywriting_formulas_formula_to_slide_mapping, _opencode_skills_slides_references_html_template, _opencode_skills_design_system_skill_slide_token_validator_py [INFERRED 0.85]
- **Interactive component state contract (spec pattern + states + priority + accessibility)** — _opencode_skills_design_system_skill_component_spec_pattern, _opencode_skills_design_system_references_component_specs_button, _opencode_skills_design_system_references_states_and_variants_interactive_states, _opencode_skills_design_system_references_states_and_variants_state_priority, _opencode_skills_design_system_references_states_and_variants_accessibility_requirements, _opencode_skills_design_system_references_component_tokens_button_tokens [EXTRACTED 1.00]
- **Three-layer design token hierarchy (primitive → semantic → component)** — _opencode_skills_design_system_references_primitive_tokens, _opencode_skills_design_system_references_semantic_tokens, _opencode_skills_design_system_references_component_tokens, _opencode_skills_design_system_skill_three_layer_token_structure, _opencode_skills_design_system_references_token_architecture_why_three_layers [EXTRACTED 1.00]
- **Industry Color Semantics Shared Across Logo and CIP** — _opencode_skills_design_references_logo_color_psychology_primary_color_meanings, _opencode_skills_design_references_logo_color_psychology_industry_color_combinations, _opencode_skills_design_references_cip_style_guide_cip_color_psychology, _opencode_skills_design_references_logo_design_industry_defaults, _opencode_skills_design_references_cip_style_guide_cip_design_styles [INFERRED 0.85]
- **AI Image Prompt Construction (structure + style keywords + negative prompts)** — _opencode_skills_design_references_logo_prompt_engineering_core_prompt_structure, _opencode_skills_design_references_logo_prompt_engineering_style_keyword_library, _opencode_skills_design_references_logo_prompt_engineering_negative_prompts, _opencode_skills_design_references_cip_prompt_engineering_base_prompt_structure, _opencode_skills_design_references_cip_prompt_engineering_deliverable_modifiers, _opencode_skills_design_references_cip_prompt_engineering_quality_modifiers [INFERRED 0.85]
- **HTML/CSS to PNG Render Pipeline (banner, social photos, CIP presentation)** — _opencode_skills_design_skill_html_screenshot_export_pattern, _opencode_skills_design_references_social_photos_design_html_screenshot_pipeline, _opencode_skills_design_references_social_photos_design_screenshot_export_options, _opencode_skills_design_references_social_photos_design_visual_qa_loop, _opencode_skills_design_references_cip_design_cip_html_presentation [INFERRED 0.85]
- **AOT Safe Avoidance Of Reflection** — agents_appsettingsjsoncontext, agents_generic_enum_json_converter, agents_static_view_locator, agents_no_invariant_globalization, agents_handwritten_form_validation, docs_architecture_aot_requirements, docs_architecture_no_computed_type_lookup [EXTRACTED 1.00]
- **Per User Settings Flow And Handoff** — agents_loadfrom_creates_file, agents_snapshot_carries_user, agents_temporary_settings_empty_snapshot, agents_keyed_singleton_temporary_settings, agents_settings_handoff_once, docs_architecture_two_settings_instances, docs_architecture_settings_handoff, docs_architecture_user_directory_name [EXTRACTED 1.00]
- **Dispose Ownership Chain Across ViewModels** — docs_dispose_call_chain, docs_dispose_case_a_b, docs_dispose_when_required_table, docs_dispose_observablestring_case, docs_dispose_release_chain_settings, docs_dispose_review_checklist, agents_windows_manager_dispose, agents_observablestring_ownership [EXTRACTED 1.00]

## Communities (178 total, 64 thin omitted)

### Community 0 - "MoveFinder Benchmarks"
Cohesion: 0.06
Nodes (20): MoveFinderBenchmark, Scenario, Scenarios, Scenario, PatchSize, Resolution, SearchRange, BaseMoveFinder (+12 more)

### Community 1 - "BM25 Search Core"
Cohesion: 0.06
Nodes (31): BM25, _contains_phrase(), detect_domain(), _domain_keywords(), _exact_match_diagnostic(), _exact_row_identity(), _exact_stack_identifier(), _file_signature() (+23 more)

### Community 3 - "Design Tokens JSON Values"
Cohesion: 0.05
Nodes (53): $type, $value, $type, $value, $type, $value, $type, $value (+45 more)

### Community 4 - "Move Finder Manager"
Cohesion: 0.08
Nodes (20): MoveFinderFactory, MoveFinderManager, ParallelBoundaryFinder, ParallelFinder, PatchSize, RelateSearchWithPatch, Resolution, SearchRange (+12 more)

### Community 5 - "Avalonia Dependencies"
Cohesion: 0.05
Nodes (40): Avalonia, Avalonia.Desktop, Avalonia.Fonts.Inter, Avalonia.Themes.Fluent, AvaloniaUI.DiagnosticsSupport, BenchmarkDotNet, CommunityToolkit.Mvvm, Dock.Avalonia (+32 more)

### Community 6 - "Theme Token Values"
Cohesion: 0.06
Nodes (45): $type, $value, $type, $value, bg, fg, font-size, hover-bg (+37 more)

### Community 7 - "BM25 Context Formatting"
Cohesion: 0.08
Nodes (17): format_context(), format_result(), main(), BM25, calculate_pattern_break(), detect_domain(), get_background_config(), get_color_for_emotion() (+9 more)

### Community 8 - "Console Formatting Tests"
Cohesion: 0.07
Nodes (14): ansi_ljust(), _detect_page_type(), format_ascii_box(), format_markdown(), format_master_md(), format_page_override_md(), generate_design_system(), _generate_intelligent_overrides() (+6 more)

### Community 9 - "Design Image Generation"
Cohesion: 0.09
Nodes (8): enhance_prompt(), generate_batch(), generate_logo(), load_env(), main(), main(), main(), main()

### Community 10 - "MouseLab Namespaces"
Cohesion: 0.17
Nodes (6): MouseLab.Core, MouseLab.Common.Services.MoveFinder.Scalars, MouseLab.Common.Services.MoveFinder.Parallels, MouseLab.Core.Services, MouseLab.Common.Services.MoveFinder, MouseLab.Common.Services.MoveFinder.Providers

### Community 11 - "Design Catalog Validation"
Cohesion: 0.12
Nodes (28): _catalog_date(), _check_app_interface_contract(), _check_catalog_contract(), _check_catalog_summary(), _check_core_data_contract(), _check_file(), _check_font_catalog(), _check_landing_claims() (+20 more)

### Community 12 - "App Settings Events"
Cohesion: 0.10
Nodes (7): MouseLabAvaloniaApp.Models, MouseLabAvaloniaApp.Services.AppSettings, AppSettingsJsonContext, CultureChangedEventArgs, NewCulture, ThemeChangedEventArgs, NewTheme

### Community 13 - "Cutter Benchmarks"
Cohesion: 0.12
Nodes (7): CoreCutterBenchmark, CutterBenchmark, Scenario, Scenarios, ImageCutter, IMatrixProvider, IMatrixCutter

### Community 14 - "Project Namespaces and Tests"
Cohesion: 0.12
Nodes (10): MouseLab.Core.Models, MouseLab.Core.Providers, MouseLab.Services.Tests, MouseLab.Common.Services.MatrixCutters.OldVersion, MouseLab.Matcher.Tests, MouseLab.Common.Models.PooledMatrix, MouseLab.Common.Services, MouseLab.Core.Tests (+2 more)

### Community 15 - "Design Token Values"
Cohesion: 0.06
Nodes (34): $type, $value, $type, $value, $type, $value, $type, $value (+26 more)

### Community 16 - "Brand Color Utilities"
Cohesion: 0.08
Nodes (25): Text-to-Image Ratio by Medium, Banner Output Path Convention (assets/banners/{campaign}), PNG Export via Screenshot at Exact Dimensions, Post-Approval Archival (manifest.json update), Color Compliance Review (60/30/10), Technical and Platform Requirements Review, Asset Cleanup Workflow, Asset Directory Structure (.assets/ vs assets/) (+17 more)

### Community 17 - "Avalonia Views and Dock"
Cohesion: 0.15
Nodes (10): MouseLabAvaloniaApp.ViewModels.Welcome, MouseLabAvaloniaApp.Views, MouseLabAvaloniaApp.ViewModels, MouseLabAvaloniaApp.Views.Dock, MouseLabAvaloniaApp.ViewModels.Settings, MouseLabAvaloniaApp.Services.WindowsManager, MouseLabAvaloniaApp, BlankView (+2 more)

### Community 18 - "Design Diagnostics Tests"
Cohesion: 0.07
Nodes (4): _write_persisted_file(), TestDiagnosticsContracts, TestTokenizer, TestMetricMath

### Community 19 - "App Settings View Models"
Cohesion: 0.08
Nodes (14): CultureOption, Culture, CultureName, DisplayName, BlankViewModel, MyValue, Name, AppSettingsViewModel (+6 more)

### Community 20 - "Slides Sub-skill"
Cohesion: 0.13
Nodes (27): AIDA (Attention-Interest-Desire-Action), Cost of Inaction Formula, FAB (Features-Advantages-Benefits), Formula-to-Slide Mapping with Emotion, Headline Patterns (power words, contrast, social proof), PAS (Problem-Agitate-Solution), Copywriting Formulas for Slides, Slides Creation Guide (+19 more)

### Community 21 - "Matrix Pooling"
Cohesion: 0.08
Nodes (12): MatrixPool, PooledMatrix, Bytes, Height, ImmutableBytes, Owner, ReadOnlySpanBytes, Returned (+4 more)

### Community 22 - "CSS Validation Rules"
Cohesion: 0.12
Nodes (12): get_context(), is_allowed_exception(), is_allowed_rgba(), is_inside_block(), load_css_variables(), main(), print_result(), print_summary() (+4 more)

### Community 24 - "Matrix Randomizer Benchmarks"
Cohesion: 0.10
Nodes (12): ImageRandomizer, RandomizeBenchmark, Resolution, IMatrix, Bytes, Height, ImmutableBytes, ReadOnlySpanBytes (+4 more)

### Community 25 - "Welcome View Model"
Cohesion: 0.08
Nodes (16): WelcomeWindowViewModel, AvailableCultures, AvailableThemes, FirstName, FirstNameError, Group, GroupError, HasFirstNameError (+8 more)

### Community 26 - "Geometry and Cutter Tests"
Cohesion: 0.17
Nodes (8): Point, X, Y, Zero, Size, Height, Width, MatrixCutterTests

### Community 27 - "Accessibility Guidance Tests"
Cohesion: 0.14
Nodes (11): read_rows(), TestAccessibilityGuidance, TestChartsTypographyAndIcons, TestCurrentReactGuidance, TestSemanticColors, _check_chart_contract(), _check_icon_contract(), _check_typography_contract() (+3 more)

### Community 28 - "Design Token Colors"
Cohesion: 0.08
Nodes (25): $type, $value, background, destructive, foreground, muted, muted-foreground, primary-hover (+17 more)

### Community 29 - "Design System Generator"
Cohesion: 0.12
Nodes (3): DesignSystemGenerator, _resolve_dial(), TestReasoningMatch

### Community 30 - "Brand Guidelines Workflow"
Cohesion: 0.12
Nodes (23): Brand Guidelines Template (placeholder skeleton), Template Usage (save as docs/brand-guidelines.md), Color Hierarchy (primary/secondary/neutral/semantic), Color Palette Management, Absolute Logo Donts, Approved Logo Backgrounds, Mission/Vision/Value Prop/Positioning Templates, Messaging Framework Cascade (Mission to Proof Points) (+15 more)

### Community 31 - "Catalog Contract Tests"
Cohesion: 0.13
Nodes (4): read_rows(), TestGeneratedCatalogContract, TestLandingAndStackContract, TestReasoningContract

### Community 33 - "Icon Generation CLI"
Cohesion: 0.15
Nodes (9): Icon Design Built-in Sub-skill, apply_color(), apply_viewbox_size(), extract_svgs(), generate_batch(), generate_icon(), generate_sizes(), load_env() (+1 more)

### Community 34 - "Brand Token Sync Tests"
Cohesion: 0.16
Nodes (5): test_sync_parses_bundled_starter_template(), main(), _run(), test_flags_hardcoded_hex_sharing_line_with_token(), test_token_only_line_reports_no_violation()

### Community 35 - "Slide Generation Scripts"
Cohesion: 0.13
Nodes (10): _e(), generate_chart_slide(), generate_cta_slide(), generate_deck(), generate_metrics_slide(), generate_problem_slide(), generate_solution_slide(), generate_testimonial_slide() (+2 more)

### Community 36 - "Window Manager Services"
Cohesion: 0.12
Nodes (5): IWindowsManagerService, WindowsManagerService, MainWindow, SettingsWindow, WelcomeWindow

### Community 37 - "Brand Color Sync"
Cohesion: 0.13
Nodes (14): Asset Organization Guide, Color Documentation Formats (markdown, CSS vars, Tailwind), CSS Variable and Tailwind Implementation, Sync to Design Tokens Step, Design Tokens (design-tokens.json / design-tokens.css), Starter Design Components (buttons, spacing, radii), adjustBrightness(), { execFileSync } (+6 more)

### Community 38 - "CIP Search and Briefs"
Cohesion: 0.12
Nodes (10): Complete Brand Package Workflow, scripts/cip/core.py (BM25 search engine), CIP Design Built-in Sub-skill, scripts/logo/core.py (BM25 search engine), Logo Design Built-in Sub-skill, format_brief(), format_results(), main() (+2 more)

### Community 39 - "Dock Layout and Main View Model"
Cohesion: 0.12
Nodes (5): DockFactory, DocumentDock, MainWindowViewModel, Layout, WindowsManager

### Community 40 - "Color Mode Resolution"
Cohesion: 0.15
Nodes (6): _filter_anti_patterns_for_mode(), _query_wants_dark(), _resolve_color_mode(), _style_is_dark_primary(), TestAntiPatternGating, TestModeResolution

### Community 41 - "SAD Providers"
Cohesion: 0.13
Nodes (3): ConstantProvider, IVectorProvider, PointerProvider

### Community 42 - "App Startup and Theme"
Cohesion: 0.18
Nodes (5): App, Services, IApplicationSettingsService, CurrentAppTheme, CurrentCultureName

### Community 43 - "Matrix Providers and Tests"
Cohesion: 0.22
Nodes (3): PooledMatrixProvider, MatrixProviderTests, MatrixRandomizerTests

### Community 44 - "Background Image Utilities"
Cohesion: 0.16
Nodes (9): generate_css_for_background(), get_background_image(), get_curated_images(), get_overlay_css(), get_pexels_search_url(), load_backgrounds_config(), load_brand_colors(), main() (+1 more)

### Community 45 - "Design Token Sizes"
Cohesion: 0.15
Nodes (19): $type, $value, lg, $type, $value, $type, $value, $type (+11 more)

### Community 46 - "Brand Compliance Review"
Cohesion: 0.14
Nodes (18): Asset Approval Checklist, Legal and Regulatory Compliance Review, Logo Usage Review Section, Pre-Approval Quick Review, Reviewer Sign-off and Final Approval, Logo Usage Section (variants, clear space, minimum size), Audit Frequency Cadence, Brand Consistency Checklist (+10 more)

### Community 47 - "Slide Decision System"
Cohesion: 0.16
Nodes (16): Contextual Decision Flow (CSV-driven slide generation), Slide Decision System CSVs (strategy/layout/type/color/background/copy/chart), slide-token-validator.py (slide HTML token compliance check), AIDA Formula (Attention-Interest-Desire-Action), Before-After-Bridge Formula, Cost of Inaction Formula, FAB Formula (Features-Advantages-Benefits), Formula-to-Slide Mapping with Emotion (+8 more)

### Community 48 - "Matrix and Providers"
Cohesion: 0.12
Nodes (9): Matrix, Bytes, Height, ImmutableBytes, ReadOnlySpanBytes, Size, SpanBytes, Width (+1 more)

### Community 49 - "Banner Typography Rules"
Cohesion: 0.13
Nodes (17): Banner Typography Reference (2 typefaces, 4.5:1, 7 words/line), Banner Typography Rules (max 2 fonts, 16px body, 32px headline), Accessibility Review (WCAG AA 4.5:1), Typography Review Section, Color Palette Section (primary/secondary/neutral + WCAG), Typography Section (font stack, type scale), Color Usage Dos and Donts, Relative Luminance and Contrast Ratio Functions (+9 more)

### Community 50 - "Design System Skill"
Cohesion: 0.15
Nodes (9): Component Specifications Reference, Component Tokens Reference, Primitive Tokens Reference, Semantic Tokens Reference, States and Variants Reference, Tailwind Integration Reference, Token Architecture Reference, Design System Skill (+1 more)

### Community 51 - "UI Styling Skill"
Cohesion: 0.15
Nodes (16): ui-styling MIT License, Canvas Design System, shadcn ui Accessibility Patterns, shadcn ui Component Reference, shadcn ui Theming And Customization, Tailwind CSS Customization, Tailwind CSS Responsive Design, Tailwind CSS Utility Reference (+8 more)

### Community 52 - "App Services and DI"
Cohesion: 0.12
Nodes (6): AppServices, Instance, Provider, ViewModelBase, Strings, Translations

### Community 53 - "Application Settings Persistence"
Cohesion: 0.15
Nodes (8): ApplicationSettingsService, AppSettingsSnapshot, Culture, Theme, User, AppSettingsStore, LegacySettingsFilePath, RootDirectory

### Community 54 - "Asset Manifest CLI"
Cohesion: 0.21
Nodes (13): checkManifest(), formatBytes(), formatOutput(), fs, main(), parseFilename(), path, RULES (+5 more)

### Community 55 - "Brand Voice Review"
Cohesion: 0.16
Nodes (15): Content Quality and Messaging Review, Voice and Tone Section (personality, voice chart, prohibited terms), Voice Consistency Audit (tone/language/messaging), Elevator Pitches (10/30/60 second), Message Architecture (primary plus 3-5 supporting messages), Message by Audience Matrix, Message Testing Questions, Brand Voice Framework (+7 more)

### Community 56 - "CIP Generation CLI"
Cohesion: 0.19
Nodes (7): build_cip_prompt(), check_logo_required(), generate_cip_set(), generate_with_nano_banana(), load_env(), load_logo_image(), main()

### Community 57 - "Design Token Values"
Cohesion: 0.12
Nodes (16): $type, $value, $type, $value, $type, $value, $type, $value (+8 more)

### Community 58 - "Disposal Bug Analysis"
Cohesion: 0.13
Nodes (11): WindowsManagerService Checks Open Windows Then Disposes VM On Closed, Who Releases What And When Three Steps, Bug Two SettingsWindowViewModel Did Not Dispose AppSettings, Bug Three Theme Strings Were Never Released, Bug One ShowSettingsAsync Checked Windows Too Late, Dispose Call Chain ViewModelBase Strings ProTranslateStrings, Release Chain For Settings Window ViewModels, SourceGenerator And Analyzers As Analyzer References (+3 more)

### Community 59 - "Domain Search CLI"
Cohesion: 0.17
Nodes (6): detect_domain(), _load_csv(), search(), search_all(), _search_csv(), _valid_confidence()

### Community 60 - "CIP Domain Search"
Cohesion: 0.19
Nodes (6): detect_domain(), get_cip_brief(), _load_csv(), search(), search_all(), _search_csv()

### Community 61 - "Reasoning Rules"
Cohesion: 0.17
Nodes (4): apply_decision_rules(), _object_without_duplicates(), parse_decision_rules(), _validate_action()

### Community 62 - "WCAG and Palette Tests"
Cohesion: 0.18
Nodes (4): _palette_is_dark(), _relative_luminance(), TestEndToEndCoherence, TestLuminance

### Community 64 - "User Identity and Profile"
Cohesion: 0.23
Nodes (6): UserProfile, FirstName, Group, LastName, MiddleName, UserIdentity

### Community 65 - "Banner and Social Specs"
Cohesion: 0.21
Nodes (7): Banner Size Matrix (social, display ads, web, print), Banner Sizes and Art Direction Styles Reference, Parallel Subagent Orchestration for Per-Size HTML, Screenshot Export Options (Chrome headless, chrome-devtools, Playwright, Puppeteer), Social Photo Platform Sizes, Social Photos Design Guide, Social Photo Typography Hierarchy

### Community 66 - "Settings Handoff Rules"
Cohesion: 0.14
Nodes (6): Settings Handoff Happens Once In AdvanceToMainWindow, Per User settings.json Path slug hash16, AppSettingsStore CleanupOlderThan 365 Days, Profile To Settings Path Handoff In AdvanceToMainWindow, Two Settings Instances Persistent And Temporary, User Directory Name Slug Plus SHA256 Prefix

### Community 69 - "Logo and CIP Workflow"
Cohesion: 0.21
Nodes (9): CIP Workflow (brief, mockups, render), 15 Icon Styles (outlined, filled, duotone ... animated-ready), Industry Defaults (style, colors, typography), Logo Design Reference, Logo Style Catalog (55+ styles in 4 categories), Logo Workflow (brief, generate, ask, preview), Core Logo Types (wordmark, lettermark, pictorial, abstract, mascot, emblem, combination), Logo Style Guide (+1 more)

### Community 70 - "Button Component Specs"
Cohesion: 0.17
Nodes (11): Button Component Spec (variants/sizes/states/anatomy), Button Component Tokens, Border Radius Scale, Interactive State Tokens (ring, opacity-disabled, transitions), Disabled State Treatment + ARIA, Focus Ring Spec (ring offset + ring color), Interactive State Definitions (default/hover/focus/active/disabled/loading), Loading States and Spinner Placement (+3 more)

### Community 71 - "Input and Card Tokens"
Cohesion: 0.15
Nodes (12): Input Component Spec (variants/sizes/states/anatomy), Card Component Tokens, Input Component Tokens, Primitive Color Scales (gray/blue/status), Shadow Scale, Color Semantics (primary/secondary/muted/accent/destructive/status), Dark Mode Overrides (.dark class), Error States and Messages (+4 more)

### Community 72 - "Palette Selection Tests"
Cohesion: 0.22
Nodes (4): _contrast_ratio(), _derive_dark_palette(), _select_palette_for_mode(), TestPaletteSelection

### Community 75 - "HTML Rendering CLI"
Cohesion: 0.23
Nodes (4): generate_html(), get_deliverable_info(), get_image_base64(), main()

### Community 76 - "Text Layout Tests"
Cohesion: 0.18
Nodes (3): read_rows(), TestTextLayoutDataContracts, TestTextLayoutRetrieval

### Community 77 - "File Scanning CLI"
Cohesion: 0.24
Nodes (11): extensions, formatReport(), fs, getFiles(), main(), parseArgs(), path, patterns (+3 more)

### Community 78 - "Design Tokens Starter"
Cohesion: 0.17
Nodes (11): $type, $value, dark, semantic, $schema, $type, $value, semantic (+3 more)

### Community 80 - "Test Matrix Types"
Cohesion: 0.17
Nodes (8): TestMatrix, Bytes, Height, ImmutableBytes, ReadOnlySpanBytes, Size, SpanBytes, Width

### Community 81 - "Test Matrix Memory"
Cohesion: 0.17
Nodes (8): TestMatrix, Bytes, Height, ImmutableBytes, ReadOnlySpanBytes, Size, SpanBytes, Width

### Community 82 - "Banner Design Workflow"
Cohesion: 0.18
Nodes (11): 22 Art Direction Styles, Banner Sizes & Styles Reference, CTA Rules Reference, Pinterest Research Queries for Art Direction, Visual Hierarchy 3-Zone Rule, Banner Design Workflow (5 steps), Banner Design Skill, CTA Rules (one per banner, bottom-right, min 44px) (+3 more)

### Community 83 - "CIP Prompt Structure"
Cohesion: 0.22
Nodes (9): CIP Base Prompt Structure, CIP Mockup Prompt Engineering, Lighting and Context Modifiers, CIP Quality Modifiers and Negative Prompts, Core Prompt Structure, Logo AI Prompt Engineering, Logo Negative Prompts (NOT photorealistic, no text, no busy background), Logo Prompt Templates (quick, detailed brief, variation) (+1 more)

### Community 84 - "AOT Analyzer Constraints"
Cohesion: 0.18
Nodes (7): Analyzers Cannot See Runtime AOT Failures, App.axaml.cs Does Not Call AvaloniaXamlLoader For Views, ViewLocator Generated By StaticViewLocator 0.7.0, ViewLocator Is Forward Looking Not Load Bearing, Trim And AOT Analyzers Enabled On Plain Build, Views Resolved Without Reflection Generated Table, Solution Layout Tree

### Community 85 - "Translation Binding Rules"
Cohesion: 0.24
Nodes (6): A Missing Key Renders The Key Text, ProTranslateStrings Raises PropertyChanged Per Key, Method One Binding Strings Key, Runtime Symptom To Cause Table, Translation Does Not Work Checklist, Empty String Is A Valid Translation

### Community 86 - "Brand Context Extraction"
Cohesion: 0.31
Nodes (10): extractColorsFromTable(), extractCoreAttributes(), extractHexColors(), extractImageStyle(), extractTypography(), extractVoice(), fs, generatePromptAddition() (+2 more)

### Community 87 - "Token Embedding Script"
Cohesion: 0.18
Nodes (8): args, fs, minimal, MINIMAL_TOKENS, path, projectRoot, tokensPath, wrapStyle

### Community 90 - "Design Skill Routing"
Cohesion: 0.29
Nodes (9): Design Routing Guide, Multi-Skill Workflows (new project, migration, component creation), Routing by Question Type, Routing by Task Type, When to Use Multiple Skills, Design Skill (Unified), New Design System Workflow (brand to tokens to implementation), External Sub-skills (brand, design-system, ui-styling) (+1 more)

### Community 91 - "Slides Skill"
Cohesion: 0.33
Nodes (10): search-slides.py (BM25 search + contextual recommendations), BM25 Slide Search (search-slides.py), Slide System (token-driven HTML decks), Copywriting Formulas Reference, Slides Create Subcommand, HTML Slide Template Reference, Layout Patterns Reference, Slide Strategies Reference (+2 more)

### Community 92 - "Localization Architecture Docs"
Cohesion: 0.24
Nodes (7): Localization Architecture File Role Tables, MainWindowViewModel Commented Out Unsubscribe Trap, Dispose Related Documents, Which ViewModels Need Dispose And Why, Method Four Translated ComboBox Items Need ItemTemplate, MouseLab Documentation Index, Six Rules That Must Not Be Broken

### Community 94 - "Theme Options"
Cohesion: 0.20
Nodes (7): ThemeOption, DisplayName, Theme, Themes, Dark, Default, Light

### Community 95 - "Token Export CLI"
Cohesion: 0.36
Nodes (9): flattenTokens(), fs, generateCSS(), generateTailwind(), main(), parseArgs(), path, resolveReference() (+1 more)

### Community 96 - "Design Token Speeds"
Cohesion: 0.20
Nodes (10): fast, normal, slow, $type, $value, $type, $value, duration (+2 more)

### Community 97 - "SIMD Fill Operations"
Cohesion: 0.47
Nodes (7): FillChunk(), FillParallel(), FillRange(), FillScalar(), FillVector128(), FillVector256(), Randomize()

### Community 98 - "Color Psychology Reference"
Cohesion: 0.31
Nodes (7): CIP Color Psychology Table, CIP Design Style Guide, Color Harmony Types (monochromatic, complementary, analogous, triadic), Color Combinations by Industry, Logo Color Psychology, Primary Color Meanings (blue, red, green, gold, purple, orange, black, white), Quick Reference Palettes

### Community 102 - "Settings Service Base"
Cohesion: 0.25
Nodes (6): AppSettingsServiceBase, CurrentAppTheme, CurrentCultureName, FilePath, User, TemporaryAppSettingsService

### Community 103 - "CIP Deliverables"
Cohesion: 0.29
Nodes (8): Banner Print Specs (300 DPI, 3-5mm bleed, CMYK), CIP Deliverable Guide, Deliverable Specifications (sizes, materials, formats), Logo Variation Set (horizontal, vertical, stacked, monochrome), CIP Design Reference, CIP Mockup Generation, Deliverable Categories (50+ items, 10 categories), Gemini Nano Banana Image Models (Flash/Pro)

### Community 104 - "Globalization and Disposal"
Cohesion: 0.29
Nodes (4): Leak Cases A Strings And B Whole ViewModel, Dispose Cost Analysis Memory And CPU, Translation And Culture Services Registered As Singletons, Settings Culture Versus Runtime Culture

### Community 106 - "CIP Art Direction Styles"
Cohesion: 0.33
Nodes (7): 22 Art Direction Styles, Print Finishes (matte, spot UV, foil, emboss, deboss), Deliverable-Specific Modifiers, CIP Design Styles (8 styles with colors, typography, materials, finishes), Style Keyword Library, Aesthetic Styles (minimalist, vintage, luxury, geometric, organic, gradient), Art Direction Styles Reused from Banner

### Community 107 - "Design Sub-skill Routing"
Cohesion: 0.33
Nodes (5): Pinterest Art Direction Research Queries, CIP HTML Presentation (base64 single-file, dark theme), Banner Design Built-in Sub-skill, Related Skills (frontend-design, ui-ux-pro-max, ai-multimodal, chrome-devtools), Social Photos Built-in Sub-skill

### Community 108 - "Icon Design Reference"
Cohesion: 0.33
Nodes (5): 12 Icon Categories, Icon Generator CLI Options, Icon Design Reference, Icon Multi-size Export (16,24,32,48), gemini-3.1-pro-preview (text-only SVG output)

### Community 109 - "Slide HTML and Charts"
Cohesion: 0.29
Nodes (7): Chart.js Integration for Slides, Slide Requirements (navigation, centered content, Chart.js), HTML Template Chart.js Integration, Slide Navigation Controls (keyboard/click/progress bar), Responsive Breakpoints (tablet 768px / mobile 480px), 16:9 Slide Deck Container (letterboxed), Layout CSS Structures (grid/flex with breakpoints)

### Community 110 - "Theme Change Flow"
Cohesion: 0.29
Nodes (5): Theme Flow ThemeChanged App ApplyTheme RequestedThemeVariant, Theme Setting Persisted But No UI Picker Yet, Language And Theme Change Flow Diagram, Current State en-US ru-RU Per User Settings, Theme Picker Missing From UI

### Community 114 - "Designer Container Rules"
Cohesion: 0.40
Nodes (3): Designer Container AppServices Instance, Bug Four Uncached Instance Plus Finalizer Destroyed Singletons, Dispose Review Checklist

### Community 115 - "Project Boundaries"
Cohesion: 0.33
Nodes (5): IMatrixCutter Default Interface Methods 3 Abstract 6 Default, MouseLab Project Overview, Project Boundaries Core / Services / AvaloniaApp, MouseLabSolution.slnx Lives In src/, MouseRepo README

### Community 116 - "ProTranslate Dependencies"
Cohesion: 0.47
Nodes (5): ProTranslate Package Dependency Graph, Moving ViewModels To Another Project, ProTranslate Adapters For Avalonia, ProTranslate Core UI Free Services, RTL Auto Flow Direction

### Community 117 - "Translation Key Workflow"
Cohesion: 0.40
Nodes (5): Adding A New Translation String, Four Generated Members Per Key, Method Three Translation Key Attached Property, Method Two Format In The ViewModel, Analyzer Diagnostics PTA001 To PTA005

### Community 118 - "Style Identity Tests"
Cohesion: 0.47
Nodes (3): split_values(), style_identities(), TestStyleIdentityContract

### Community 121 - "Design Token sm Values"
Cohesion: 0.60
Nodes (5): sm, sm, sm, $type, $value

### Community 124 - "Banner Safe Zones"
Cohesion: 0.50
Nodes (3): Complete Banner Sizes (social, display ads, web, print), Safe Zones (edge margins, YouTube/Meta safe areas), Banner Size Quick Reference (9 platforms)

### Community 125 - "Motion and Layout Tokens"
Cohesion: 0.50
Nodes (4): Motion / Duration Scale, Tailwind Animation / Duration Tokens, Slide Animation Classes (fade-up/count/scale/stagger), Layout Selection by Use Case (25 layouts)

### Community 127 - "Design Token xl Values"
Cohesion: 0.67
Nodes (4): xl, xl, $type, $value

### Community 128 - "Design Token None Values"
Cohesion: 0.67
Nodes (4): $type, $value, none, none

### Community 130 - "WCAG Contrast Helpers"
Cohesion: 0.50
Nodes (3): _check_color_contract(), contrast_ratio(), _relative_luminance()

### Community 131 - "Benchmark Scenarios"
Cohesion: 0.50
Nodes (3): Scenario2, Resolution, SrcResolution

### Community 132 - "Typography Spacing Rules"
Cohesion: 0.67
Nodes (3): Font Weight Scale and Pairing, Line Height Guidelines, Paragraph Spacing and Line Length (65-75ch)

### Community 133 - "Dialog Component Tokens"
Cohesion: 0.67
Nodes (3): Dialog Component Spec (sizes/anatomy), Dialog/Modal Component Tokens, Z-Index Scale

### Community 136 - "Adding a New Language"
Cohesion: 0.67
Nodes (3): Adding A New Language, Catalog Structure Culture From File Name, Translation Catalog Conventions

### Community 139 - "Destructive Foreground Token"
Cohesion: 0.67
Nodes (3): destructive-foreground, $type, $value

### Community 140 - "Primary Token"
Cohesion: 0.67
Nodes (3): primary, $type, $value

### Community 141 - "Primary Foreground Token"
Cohesion: 0.67
Nodes (3): primary-foreground, $type, $value

### Community 142 - "Secondary Foreground Token"
Cohesion: 0.67
Nodes (3): secondary-foreground, $type, $value

## Ambiguous Edges - Review These
- `Banner Output Path Convention (assets/banners/{campaign})` → `Naming Convention type_campaign_description_timestamp_variant`  [AMBIGUOUS]
  .opencode/skills/brand/references/asset-organization.md · relation: conceptually_related_to
- `Logo Usage Section (variants, clear space, minimum size)` → `Logo Minimum Size (digital 120px/24px, print 35mm/10mm)`  [AMBIGUOUS]
  .opencode/skills/brand/references/brand-guideline-template.md · relation: shares_data_with
- `MouseLab Project Overview` → `MouseRepo README`  [AMBIGUOUS]
  README.md · relation: references

## Knowledge Gaps
- **412 isolated node(s):** `$schema`, `plugin`, `fs`, `path`, `fs` (+407 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 974 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **64 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **What is the exact relationship between `Banner Output Path Convention (assets/banners/{campaign})` and `Naming Convention type_campaign_description_timestamp_variant`?**
  _Edge tagged AMBIGUOUS (relation: conceptually_related_to) - confidence is low._
- **Why does `TailwindConfigGenerator` connect `Tailwind Config Generator` to `Design Image Generation`, `Tailwind Config Tests`, `Tailwind Palette Test`, `Tailwind Fonts Test`, `Tailwind Spacing Test`, `Tailwind Plugin Recommendation Test`, `Tailwind Next.js Plugin Test`, `Tailwind TypeScript Test`, `Tailwind TypeScript Init Test`, `Tailwind Validation Test`, `Tailwind Empty Config Test`, `Tailwind JavaScript Init Test`, `Tailwind Write Config Test`, `Tailwind Framework Init Test`, `Tailwind Full JS Config Test`, `Tailwind TypeScript Output Test`, `Tailwind Custom Output Test`, `Tailwind Custom Colors Test`, `Tailwind Config Emission`, `Tailwind Config Base`, `Generated Config Tests`?**
  _High betweenness centrality (0.023) - this node is a cross-community bridge._
- **Are the 2 inferred relationships involving `TailwindConfigGenerator` (e.g. with `TestGeneratedConfigIsValidJs` and `TestTailwindConfigGenerator`) actually correct?**
  _`TailwindConfigGenerator` has 2 INFERRED edges - model-reasoned connections that need verification._
- **What connects `$schema`, `plugin`, `fs` to the rest of the system?**
  _412 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `MoveFinder Benchmarks` be split into smaller, more focused modules?**
  _Cohesion score 0.05980861244019139 - nodes in this community are weakly interconnected._
- **What is the exact relationship between `Logo Usage Section (variants, clear space, minimum size)` and `Logo Minimum Size (digital 120px/24px, print 35mm/10mm)`?**
  _Edge tagged AMBIGUOUS (relation: shares_data_with) - confidence is low._
- **Why does `ShadcnInstaller` connect `shadcn Installer Tests` to `Design Image Generation`, `Brand Token Sync Tests`?**
  _High betweenness centrality (0.017) - this node is a cross-community bridge._