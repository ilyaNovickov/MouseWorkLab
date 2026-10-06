# Graph Report - MouseWorkLab  (2026-10-06)

## Corpus Check
- 169 files · ~222,973 words
- Verdict: corpus is large enough that graph structure adds value.
- Unclassified: 62 file(s) not represented in the graph (top: .csv 53, .axaml 5, (none) 2)

## Summary
- 2032 nodes · 3225 edges · 170 communities (109 shown, 61 thin omitted)
- Extraction: 92% EXTRACTED · 8% INFERRED · 0% AMBIGUOUS · INFERRED: 254 edges (avg confidence: 0.86)
- Token cost: 0 input · 0 output

## Community Hubs (Navigation)
- BM25 Search Core (scripts)
- Design Tokens JSON Values
- shadcn Installer Tests
- Asset Validation Scripts
- Slide Search CLI
- Console Formatting Utilities
- Design Tokens JSON Values
- Brand and Banner Composition Rules
- Core Data Quality Contracts
- Design Data Contract Tests
- Avalonia App Lifecycle and DI
- Token Validator Wrappers
- HTML Token Validator CLI
- Tailwind Config Tests
- Slide Copywriting Formulas
- Avalonia Namespaces and Views
- Welcome Window View Model
- Settings Architecture Rules
- Matrix Model Types and Spans
- UX Data Quality Contracts
- BM25 Search Core (cip)
- Design System Generator
- NuGet Package References
- Catalog Refresh and Relevance Tests
- Slide Generation CLI
- Settings View Models
- Tailwind Config Generator API
- Window Manager and Temp Settings
- Dark Mode Resolution
- App Shutdown and Disposal
- Background Image Fetching
- Icon Generation CLI
- Design Tokens Semantic Colors
- Brand Approval Checklists
- Brand Guidelines Templates
- Slide Decision System
- BM25 Core Regression Tests
- Web Stack Freshness Tests
- Settings Models and JSON Serialization
- Design Token Reference Docs
- shadcn and Tailwind Reference Docs
- Brand Voice and Messaging
- CIP Image Generation CLI
- Design Tokens JSON Values
- Disposal and Settings Window Bugs
- Reasoning Rules Application
- Dark Palette and WCAG Tests
- Catalog Refresh Tests
- User Profile and Identity
- Banner Sizes and Typography Rules
- Native Desktop Stack Freshness
- Core Domain Types
- Logo Generation CLI
- Application Settings Persistence
- CIP, Logo and Icon Design
- Button Component Tokens
- Input and Card Component Tokens
- Localization Documentation Concepts
- Logo BM25 Search Core
- Main Window View Model
- Design Tokens Starter Schema
- WCAG Contrast and Palette Selection
- Search Router Tests
- Search Domain Tests
- Brand Package Workflow Scripts
- HTML Rendering CLI
- Text Layout Resilience Tests
- Design Tokens Component Tokens
- Banner Design Workflow
- Typography Rules and Specs
- Color Palette and WCAG Rules
- Brand Context Extraction Script
- Logo and CIP Prompt Engineering
- AOT Analyzer Constraints
- ProTranslate Binding Rules
- Brand Context Injection Script
- Design Tokens Primitive Values
- Design Skill Routing
- Slide Skill References
- Design Tokens Button Tokens
- Style Taxonomy Tests
- Color Psychology and Harmony
- CIP Search CLI
- Catalog Validation
- BM25 Algorithm Implementation
- Generated Config Validity Tests
- Asset Organization and Sync
- CIP Deliverables and Mockups
- Design Tokens Input Tokens
- Design Tokens Radius Tokens
- Art Direction Styles
- Sub-skill Scope Boundaries
- Icon Design Reference
- Slide HTML Layout and Charts
- Relevance Evaluator Threshold Tests
- Startup Wiring Order
- Settings File Storage Format
- Designer Container and Finalizer Rules
- Project Overview and Boundaries
- ProTranslate Per-Project Generation
- Translation Key Authoring Workflow
- Culture and Theme Change Events
- Design Tokens Shadow Values
- CultureOption Model
- XAML Translation Bugs
- Avalonia Program Entry Point
- Tailwind Config CLI Entry
- Design Tokens Border Tokens
- Design Tokens Radius Values
- Design Tokens Size Tokens
- Banner Sizes and Safe Zones
- Motion and Animation Tokens
- Design Tokens Padding Tokens
- Design Tokens xl Values
- Design Tokens md Values
- Design Tokens None Values
- Vector Value Type
- Typography Scale and Spacing
- Dialog Component Tokens
- Compiled Translation Catalogs
- JSON Catalog Diagnostics
- Adding a New Language
- opencode.json Plugin Entry
- opencode.json Plugin Schema
- Design Tokens Destructive Token
- Design Tokens Destructive Foreground
- Design Tokens Muted Token
- Design Tokens Primary Foreground
- Design Tokens Ring Token
- Design Tokens Secondary Foreground
- Print Specifications
- Imagery Style Guidelines
- Alert Component Tokens
- Badge Component Tokens
- Card Component Tokens
- Table Component Tokens
- Spacing Token Semantics
- Typography Token Semantics
- Token File Organization
- DTCG Token Alignment
- Slide Background Treatments
- NativeAOT Publish Requirements
- AOT Image Size
- Tailwind CSS Variables
- UI Styling Test Requirements

## God Nodes (most connected - your core abstractions)
1. `TailwindConfigGenerator` - 58 edges
2. `TestTailwindConfigGenerator` - 35 edges
3. `DesignSystemGenerator` - 35 edges
4. `ShadcnInstaller` - 34 edges
5. `WelcomeWindowViewModel` - 34 edges
6. `TestShadcnInstaller` - 26 edges
7. `AppSettingsViewModel` - 22 edges
8. `Brand Skill` - 17 edges
9. `read_rows()` - 16 edges
10. `IMatrix` - 16 edges

## Surprising Connections (you probably didn't know these)
- `Theme Picker Missing From UI` --semantically_similar_to--> `Theme Setting Persisted But No UI Picker Yet`  [INFERRED] [semantically similar]
  docs/README.md → AGENTS.md
- `Type Scale (Major Third 1.25, 16px base)` --semantically_similar_to--> `Typography Section (font stack, type scale)`  [INFERRED] [semantically similar]
  .opencode/skills/brand/references/typography-specifications.md → .opencode/skills/brand/references/brand-guideline-template.md
- `Color Hierarchy (primary/secondary/neutral/semantic)` --shares_data_with--> `Starter Semantic Colors (success/warning/error/info)`  [INFERRED]
  .opencode/skills/brand/references/color-palette-management.md → .opencode/skills/brand/templates/brand-guidelines-starter.md
- `Type Scale (Major Third 1.25, 16px base)` --semantically_similar_to--> `Core Visual Elements (logo/color/type)`  [INFERRED] [semantically similar]
  .opencode/skills/brand/references/typography-specifications.md → .opencode/skills/brand/references/visual-identity.md
- `Logo Variation Set (horizontal, vertical, stacked, monochrome)` --conceptually_related_to--> `Scalability Checklist (16x16 favicon, single color, black/white)`  [INFERRED]
  .opencode/skills/design/references/cip-deliverable-guide.md → .opencode/skills/design/references/logo-style-guide.md

## Import Cycles
- None detected.

## Hyperedges (group relationships)
- **Brand Token Sync Chain (guidelines to tokens)** — _opencode_skills_brand_skill_brand_guidelines_source_of_truth, opencode_skills_brand_scripts_sync_brand_to_tokens, _opencode_skills_brand_skill_design_tokens, _opencode_skills_brand_references_update_sync_to_design_tokens, _opencode_skills_brand_references_update_verify_sync, _opencode_skills_brand_references_update_always_sync_all_three_files, _opencode_skills_brand_references_brand_guideline_template_extractable_fields [EXTRACTED 1.00]
- **Brand Compliance Review Family (visual, voice, technical, legal)** — _opencode_skills_brand_references_approval_checklist_assetapprovalchecklist, _opencode_skills_brand_references_approval_checklist_logo_usage_review, _opencode_skills_brand_references_approval_checklist_color_compliance_review, _opencode_skills_brand_references_approval_checklist_accessibility_review, _opencode_skills_brand_references_approval_checklist_technical_requirements_review, _opencode_skills_brand_references_approval_checklist_legal_compliance_review, _opencode_skills_brand_references_consistency_checklist_brandconsistencychecklist, _opencode_skills_brand_references_logo_usage_rules_logousagerules, _opencode_skills_brand_references_color_palette_management_wcag_contrast_ratios [INFERRED 0.85]
- **Banner Design Pipeline (research to exported variants)** — _opencode_skills_banner_design_skill_banner_workflow, _opencode_skills_banner_design_references_banner_sizes_and_styles_pinterest_research_queries, _opencode_skills_banner_design_references_banner_sizes_and_styles_art_direction_styles, _opencode_skills_banner_design_skill_model_selection, _opencode_skills_banner_design_skill_html_css_composition, _opencode_skills_banner_design_skill_png_export, _opencode_skills_banner_design_skill_output_path_convention [EXTRACTED 1.00]
- **HTML/CSS to PNG Render Pipeline (banner, social photos, CIP presentation)** — _opencode_skills_design_skill_html_screenshot_export_pattern, _opencode_skills_design_references_social_photos_design_html_screenshot_pipeline, _opencode_skills_design_references_social_photos_design_screenshot_export_options, _opencode_skills_design_references_social_photos_design_visual_qa_loop, _opencode_skills_design_references_cip_design_cip_html_presentation [INFERRED 0.85]
- **AI Image Prompt Construction (structure + style keywords + negative prompts)** — _opencode_skills_design_references_logo_prompt_engineering_core_prompt_structure, _opencode_skills_design_references_logo_prompt_engineering_style_keyword_library, _opencode_skills_design_references_logo_prompt_engineering_negative_prompts, _opencode_skills_design_references_cip_prompt_engineering_base_prompt_structure, _opencode_skills_design_references_cip_prompt_engineering_deliverable_modifiers, _opencode_skills_design_references_cip_prompt_engineering_quality_modifiers [INFERRED 0.85]
- **Industry Color Semantics Shared Across Logo and CIP** — _opencode_skills_design_references_logo_color_psychology_primary_color_meanings, _opencode_skills_design_references_logo_color_psychology_industry_color_combinations, _opencode_skills_design_references_cip_style_guide_cip_color_psychology, _opencode_skills_design_references_logo_design_industry_defaults, _opencode_skills_design_references_cip_style_guide_cip_design_styles [INFERRED 0.85]
- **Three-layer design token hierarchy (primitive → semantic → component)** — _opencode_skills_design_system_references_primitive_tokens, _opencode_skills_design_system_references_semantic_tokens, _opencode_skills_design_system_references_component_tokens, _opencode_skills_design_system_skill_three_layer_token_structure, _opencode_skills_design_system_references_token_architecture_why_three_layers [EXTRACTED 1.00]
- **Interactive component state contract (spec pattern + states + priority + accessibility)** — _opencode_skills_design_system_skill_component_spec_pattern, _opencode_skills_design_system_references_component_specs_button, _opencode_skills_design_system_references_states_and_variants_interactive_states, _opencode_skills_design_system_references_states_and_variants_state_priority, _opencode_skills_design_system_references_states_and_variants_accessibility_requirements, _opencode_skills_design_system_references_component_tokens_button_tokens [EXTRACTED 1.00]
- **Slide generation pipeline (BM25 search → decision CSVs → strategy/layout/copy → tokenized HTML + validation)** — _opencode_skills_design_system_skill_slide_search_bm25, _opencode_skills_design_system_skill_contextual_decision_flow, _opencode_skills_slides_references_slide_strategies_deck_structure_catalog, _opencode_skills_slides_references_layout_patterns_layout_selection_by_use_case, _opencode_skills_slides_references_copywriting_formulas_formula_to_slide_mapping, _opencode_skills_slides_references_html_template, _opencode_skills_design_system_skill_slide_token_validator_py [INFERRED 0.85]
- **AOT Safe Avoidance Of Reflection** — agents_appsettingsjsoncontext, agents_generic_enum_json_converter, agents_static_view_locator, agents_no_invariant_globalization, agents_handwritten_form_validation, docs_architecture_aot_requirements, docs_architecture_no_computed_type_lookup [EXTRACTED 1.00]
- **Per User Settings Flow And Handoff** — agents_loadfrom_creates_file, agents_snapshot_carries_user, agents_temporary_settings_empty_snapshot, agents_keyed_singleton_temporary_settings, agents_settings_handoff_once, docs_architecture_two_settings_instances, docs_architecture_settings_handoff, docs_architecture_user_directory_name [EXTRACTED 1.00]
- **Dispose Ownership Chain Across ViewModels** — docs_dispose_call_chain, docs_dispose_case_a_b, docs_dispose_when_required_table, docs_dispose_observablestring_case, docs_dispose_release_chain_settings, docs_dispose_review_checklist, agents_windows_manager_dispose, agents_observablestring_ownership [EXTRACTED 1.00]

## Communities (170 total, 61 thin omitted)

### Community 0 - "BM25 Search Core (scripts)"
Cohesion: 0.06
Nodes (31): BM25, _contains_phrase(), detect_domain(), _domain_keywords(), _exact_match_diagnostic(), _exact_row_identity(), _exact_stack_identifier(), _file_signature() (+23 more)

### Community 1 - "Design Tokens JSON Values"
Cohesion: 0.05
Nodes (53): $type, $value, $type, $value, $type, $value, $type, $value (+45 more)

### Community 3 - "Asset Validation Scripts"
Cohesion: 0.06
Nodes (41): checkManifest(), formatBytes(), formatOutput(), fs, main(), parseFilename(), path, RULES (+33 more)

### Community 4 - "Slide Search CLI"
Cohesion: 0.08
Nodes (17): format_context(), format_result(), main(), BM25, calculate_pattern_break(), detect_domain(), get_background_config(), get_color_for_emotion() (+9 more)

### Community 5 - "Console Formatting Utilities"
Cohesion: 0.07
Nodes (14): ansi_ljust(), _detect_page_type(), format_ascii_box(), format_markdown(), format_master_md(), format_page_override_md(), generate_design_system(), _generate_intelligent_overrides() (+6 more)

### Community 6 - "Design Tokens JSON Values"
Cohesion: 0.06
Nodes (34): $type, $value, $type, $value, $type, $value, $type, $value (+26 more)

### Community 7 - "Brand and Banner Composition Rules"
Cohesion: 0.08
Nodes (25): Text-to-Image Ratio by Medium, Banner Output Path Convention (assets/banners/{campaign}), PNG Export via Screenshot at Exact Dimensions, Post-Approval Archival (manifest.json update), Color Compliance Review (60/30/10), Technical and Platform Requirements Review, Asset Cleanup Workflow, Asset Directory Structure (.assets/ vs assets/) (+17 more)

### Community 8 - "Core Data Quality Contracts"
Cohesion: 0.13
Nodes (24): _check_app_interface_contract(), _check_chart_contract(), _check_color_contract(), _check_core_data_contract(), _check_file(), _check_icon_contract(), _check_landing_claims(), _check_motion_contract() (+16 more)

### Community 9 - "Design Data Contract Tests"
Cohesion: 0.10
Nodes (7): read_rows(), split_values(), style_identities(), TestGeneratedCatalogContract, TestLandingAndStackContract, TestReasoningContract, TestStyleIdentityContract

### Community 10 - "Avalonia App Lifecycle and DI"
Cohesion: 0.11
Nodes (14): App, Services, Themes, Dark, Default, Light, AppSettingsServiceBase, CurrentAppTheme (+6 more)

### Community 11 - "Token Validator Wrappers"
Cohesion: 0.13
Nodes (6): test_sync_parses_bundled_starter_template(), main(), _run(), test_flags_hardcoded_hex_sharing_line_with_token(), test_token_only_line_reports_no_violation(), main()

### Community 12 - "HTML Token Validator CLI"
Cohesion: 0.12
Nodes (12): get_context(), is_allowed_exception(), is_allowed_rgba(), is_inside_block(), load_css_variables(), main(), print_result(), print_summary() (+4 more)

### Community 14 - "Slide Copywriting Formulas"
Cohesion: 0.14
Nodes (26): AIDA (Attention-Interest-Desire-Action), Cost of Inaction Formula, FAB (Features-Advantages-Benefits), Formula-to-Slide Mapping with Emotion, Headline Patterns (power words, contrast, social proof), PAS (Problem-Agitate-Solution), Copywriting Formulas for Slides, Slides Creation Guide (+18 more)

### Community 15 - "Avalonia Namespaces and Views"
Cohesion: 0.17
Nodes (8): MouseLabAvaloniaApp.ViewModels.Welcome, MouseLabAvaloniaApp.Views, MouseLabAvaloniaApp.ViewModels, MouseLabAvaloniaApp.ViewModels.Settings, MouseLabAvaloniaApp.Services.WindowsManager, MouseLabAvaloniaApp, ViewLocator, AppSettingsView

### Community 16 - "Welcome Window View Model"
Cohesion: 0.08
Nodes (16): WelcomeWindowViewModel, AvailableCultures, AvailableThemes, FirstName, FirstNameError, Group, GroupError, HasFirstNameError (+8 more)

### Community 17 - "Settings Architecture Rules"
Cohesion: 0.08
Nodes (13): Settings Handoff Happens Once In AdvanceToMainWindow, Per User settings.json Path slug hash16, Theme Flow ThemeChanged App ApplyTheme RequestedThemeVariant, Theme Setting Persisted But No UI Picker Yet, AppSettingsStore CleanupOlderThan 365 Days, Language And Theme Change Flow Diagram, Profile To Settings Path Handoff In AdvanceToMainWindow, Two Settings Instances Persistent And Temporary (+5 more)

### Community 18 - "Matrix Model Types and Spans"
Cohesion: 0.09
Nodes (18): IMatrix, Bytes, Height, ImmutableBytes, ReadOnlySpanBytes, Size, SpanBytes, Width (+10 more)

### Community 19 - "UX Data Quality Contracts"
Cohesion: 0.13
Nodes (11): read_rows(), TestAccessibilityGuidance, TestChartsTypographyAndIcons, TestCurrentReactGuidance, TestSemanticColors, _check_typography_contract(), _configured_font_names(), contrast_ratio() (+3 more)

### Community 20 - "BM25 Search Core (cip)"
Cohesion: 0.11
Nodes (7): BM25, detect_domain(), get_cip_brief(), _load_csv(), search(), search_all(), _search_csv()

### Community 21 - "Design System Generator"
Cohesion: 0.12
Nodes (3): DesignSystemGenerator, _resolve_dial(), TestReasoningMatch

### Community 22 - "NuGet Package References"
Cohesion: 0.09
Nodes (22): Avalonia (12.1.3), Avalonia.Desktop (12.1.3), Avalonia.Fonts.Inter (12.1.3), Avalonia.Themes.Fluent (12.1.3), Avalonia.Wayland (12.1.3), AvaloniaUI.DiagnosticsSupport (2.2.3), CommunityToolkit.Mvvm (8.4.2), Microsoft.Extensions.DependencyInjection (10.0.12) (+14 more)

### Community 23 - "Catalog Refresh and Relevance Tests"
Cohesion: 0.10
Nodes (3): _write_persisted_file(), TestFixtureValidation, TestMetricMath

### Community 24 - "Slide Generation CLI"
Cohesion: 0.13
Nodes (11): _e(), generate_chart_slide(), generate_cta_slide(), generate_deck(), generate_metrics_slide(), generate_problem_slide(), generate_solution_slide(), generate_testimonial_slide() (+3 more)

### Community 25 - "Settings View Models"
Cohesion: 0.11
Nodes (7): AppSettingsViewModel, AvailableCultures, AvailableThemes, SelectedCulture, SelectedTheme, SettingsWindowViewModel, AppSettings

### Community 27 - "Window Manager and Temp Settings"
Cohesion: 0.12
Nodes (6): TemporaryAppSettingsService, IWindowsManagerService, WindowsManagerService, MainWindow, SettingsWindow, WelcomeWindow

### Community 28 - "Dark Mode Resolution"
Cohesion: 0.14
Nodes (6): _filter_anti_patterns_for_mode(), _query_wants_dark(), _resolve_color_mode(), _style_is_dark_primary(), TestAntiPatternGating, TestModeResolution

### Community 29 - "App Shutdown and Disposal"
Cohesion: 0.11
Nodes (6): AppServices, Instance, Provider, ThemeOption, DisplayName, Theme

### Community 30 - "Background Image Fetching"
Cohesion: 0.16
Nodes (9): generate_css_for_background(), get_background_image(), get_curated_images(), get_overlay_css(), get_pexels_search_url(), load_backgrounds_config(), load_brand_colors(), main() (+1 more)

### Community 31 - "Icon Generation CLI"
Cohesion: 0.16
Nodes (9): Icon Design Built-in Sub-skill, apply_color(), apply_viewbox_size(), extract_svgs(), generate_batch(), generate_icon(), generate_sizes(), load_env() (+1 more)

### Community 32 - "Design Tokens Semantic Colors"
Cohesion: 0.11
Nodes (19): $type, $value, background, foreground, muted-foreground, primary, primary-hover, secondary (+11 more)

### Community 33 - "Brand Approval Checklists"
Cohesion: 0.14
Nodes (18): Asset Approval Checklist, Legal and Regulatory Compliance Review, Logo Usage Review Section, Pre-Approval Quick Review, Reviewer Sign-off and Final Approval, Logo Usage Section (variants, clear space, minimum size), Audit Frequency Cadence, Brand Consistency Checklist (+10 more)

### Community 34 - "Brand Guidelines Templates"
Cohesion: 0.16
Nodes (18): Brand Guidelines Template (placeholder skeleton), Template Usage (save as docs/brand-guidelines.md), Absolute Logo Donts, Mission/Vision/Value Prop/Positioning Templates, Messaging Framework Cascade (Mission to Proof Points), Messaging Framework, Brand Update Command, Skills Used (brand, design-system) (+10 more)

### Community 35 - "Slide Decision System"
Cohesion: 0.16
Nodes (16): Contextual Decision Flow (CSV-driven slide generation), Slide Decision System CSVs (strategy/layout/type/color/background/copy/chart), slide-token-validator.py (slide HTML token compliance check), AIDA Formula (Attention-Interest-Desire-Action), Before-After-Bridge Formula, Cost of Inaction Formula, FAB Formula (Features-Advantages-Benefits), Formula-to-Slide Mapping with Emotion (+8 more)

### Community 36 - "BM25 Core Regression Tests"
Cohesion: 0.11
Nodes (3): TestBm25CoreBehavior, TestDiagnosticsContracts, TestTokenizer

### Community 38 - "Settings Models and JSON Serialization"
Cohesion: 0.21
Nodes (3): MouseLabAvaloniaApp.Models, MouseLabAvaloniaApp.Services.AppSettings, AppSettingsJsonContext

### Community 39 - "Design Token Reference Docs"
Cohesion: 0.15
Nodes (9): Component Specifications Reference, Component Tokens Reference, Primitive Tokens Reference, Semantic Tokens Reference, States and Variants Reference, Tailwind Integration Reference, Token Architecture Reference, Design System Skill (+1 more)

### Community 40 - "shadcn and Tailwind Reference Docs"
Cohesion: 0.15
Nodes (16): ui-styling MIT License, Canvas Design System, shadcn ui Accessibility Patterns, shadcn ui Component Reference, shadcn ui Theming And Customization, Tailwind CSS Customization, Tailwind CSS Responsive Design, Tailwind CSS Utility Reference (+8 more)

### Community 41 - "Brand Voice and Messaging"
Cohesion: 0.16
Nodes (15): Content Quality and Messaging Review, Voice and Tone Section (personality, voice chart, prohibited terms), Voice Consistency Audit (tone/language/messaging), Elevator Pitches (10/30/60 second), Message Architecture (primary plus 3-5 supporting messages), Message by Audience Matrix, Message Testing Questions, Brand Voice Framework (+7 more)

### Community 42 - "CIP Image Generation CLI"
Cohesion: 0.19
Nodes (7): build_cip_prompt(), check_logo_required(), generate_cip_set(), generate_with_nano_banana(), load_env(), load_logo_image(), main()

### Community 44 - "Design Tokens JSON Values"
Cohesion: 0.12
Nodes (16): $type, $value, $type, $value, $type, $value, $type, $value (+8 more)

### Community 45 - "Disposal and Settings Window Bugs"
Cohesion: 0.13
Nodes (11): WindowsManagerService Checks Open Windows Then Disposes VM On Closed, Who Releases What And When Three Steps, Bug Two SettingsWindowViewModel Did Not Dispose AppSettings, Bug Three Theme Strings Were Never Released, Bug One ShowSettingsAsync Checked Windows Too Late, Dispose Call Chain ViewModelBase Strings ProTranslateStrings, Release Chain For Settings Window ViewModels, SourceGenerator And Analyzers As Analyzer References (+3 more)

### Community 46 - "Reasoning Rules Application"
Cohesion: 0.17
Nodes (4): apply_decision_rules(), _object_without_duplicates(), parse_decision_rules(), _validate_action()

### Community 47 - "Dark Palette and WCAG Tests"
Cohesion: 0.18
Nodes (4): _palette_is_dark(), _relative_luminance(), TestEndToEndCoherence, TestLuminance

### Community 49 - "User Profile and Identity"
Cohesion: 0.23
Nodes (6): UserProfile, FirstName, Group, LastName, MiddleName, UserIdentity

### Community 50 - "Banner Sizes and Typography Rules"
Cohesion: 0.21
Nodes (7): Banner Size Matrix (social, display ads, web, print), Banner Sizes and Art Direction Styles Reference, Parallel Subagent Orchestration for Per-Size HTML, Screenshot Export Options (Chrome headless, chrome-devtools, Playwright, Puppeteer), Social Photo Platform Sizes, Social Photos Design Guide, Social Photo Typography Hierarchy

### Community 52 - "Core Domain Types"
Cohesion: 0.29
Nodes (4): MouseLab.Core, MouseLab.Core.Models, MouseLab.Core.Providers, MouseLab.Services

### Community 53 - "Logo Generation CLI"
Cohesion: 0.20
Nodes (5): enhance_prompt(), generate_batch(), generate_logo(), load_env(), main()

### Community 54 - "Application Settings Persistence"
Cohesion: 0.19
Nodes (8): ApplicationSettingsService, AppSettingsSnapshot, Culture, Theme, User, AppSettingsStore, LegacySettingsFilePath, RootDirectory

### Community 55 - "CIP, Logo and Icon Design"
Cohesion: 0.21
Nodes (9): CIP Workflow (brief, mockups, render), 15 Icon Styles (outlined, filled, duotone ... animated-ready), Industry Defaults (style, colors, typography), Logo Design Reference, Logo Style Catalog (55+ styles in 4 categories), Logo Workflow (brief, generate, ask, preview), Core Logo Types (wordmark, lettermark, pictorial, abstract, mascot, emblem, combination), Logo Style Guide (+1 more)

### Community 56 - "Button Component Tokens"
Cohesion: 0.17
Nodes (11): Button Component Spec (variants/sizes/states/anatomy), Button Component Tokens, Border Radius Scale, Interactive State Tokens (ring, opacity-disabled, transitions), Disabled State Treatment + ARIA, Focus Ring Spec (ring offset + ring color), Interactive State Definitions (default/hover/focus/active/disabled/loading), Loading States and Spinner Placement (+3 more)

### Community 57 - "Input and Card Component Tokens"
Cohesion: 0.15
Nodes (12): Input Component Spec (variants/sizes/states/anatomy), Card Component Tokens, Input Component Tokens, Primitive Color Scales (gray/blue/status), Shadow Scale, Color Semantics (primary/secondary/muted/accent/destructive/status), Dark Mode Overrides (.dark class), Error States and Messages (+4 more)

### Community 58 - "Localization Documentation Concepts"
Cohesion: 0.21
Nodes (9): Localization Architecture File Role Tables, Leak Cases A Strings And B Whole ViewModel, Dispose Cost Analysis Memory And CPU, MainWindowViewModel Commented Out Unsubscribe Trap, Dispose Related Documents, Which ViewModels Need Dispose And Why, Method Four Translated ComboBox Items Need ItemTemplate, MouseLab Documentation Index (+1 more)

### Community 59 - "Logo BM25 Search Core"
Cohesion: 0.22
Nodes (5): detect_domain(), _load_csv(), search(), search_all(), _search_csv()

### Community 60 - "Main Window View Model"
Cohesion: 0.15
Nodes (5): MainWindowViewModel, WindowsManager, ViewModelBase, Strings, Translations

### Community 61 - "Design Tokens Starter Schema"
Cohesion: 0.15
Nodes (12): component, $type, $value, dark, semantic, $schema, $type, $value (+4 more)

### Community 62 - "WCAG Contrast and Palette Selection"
Cohesion: 0.22
Nodes (4): _contrast_ratio(), _derive_dark_palette(), _select_palette_for_mode(), TestPaletteSelection

### Community 65 - "Brand Package Workflow Scripts"
Cohesion: 0.18
Nodes (7): Complete Brand Package Workflow, CIP Design Built-in Sub-skill, scripts/logo/core.py (BM25 search engine), Logo Design Built-in Sub-skill, Slides Built-in Sub-skill, format_output(), generate_design_brief()

### Community 66 - "HTML Rendering CLI"
Cohesion: 0.23
Nodes (4): generate_html(), get_deliverable_info(), get_image_base64(), main()

### Community 67 - "Text Layout Resilience Tests"
Cohesion: 0.18
Nodes (3): read_rows(), TestTextLayoutDataContracts, TestTextLayoutRetrieval

### Community 68 - "Design Tokens Component Tokens"
Cohesion: 0.20
Nodes (12): $type, $value, bg, bg, padding, shadow, card, bg (+4 more)

### Community 71 - "Banner Design Workflow"
Cohesion: 0.18
Nodes (11): 22 Art Direction Styles, Banner Sizes & Styles Reference, CTA Rules Reference, Pinterest Research Queries for Art Direction, Visual Hierarchy 3-Zone Rule, Banner Design Workflow (5 steps), Banner Design Skill, CTA Rules (one per banner, bottom-right, min 44px) (+3 more)

### Community 72 - "Typography Rules and Specs"
Cohesion: 0.22
Nodes (11): Banner Typography Reference (2 typefaces, 4.5:1, 7 words/line), Banner Typography Rules (max 2 fonts, 16px body, 32px headline), Typography Review Section, Typography Section (font stack, type scale), Common Font Pairings, Font Stack Structure (Inter plus JetBrains Mono), Letter Spacing and Tracking Rules, Type Scale (Major Third 1.25, 16px base) (+3 more)

### Community 73 - "Color Palette and WCAG Rules"
Cohesion: 0.18
Nodes (11): Accessibility Review (WCAG AA 4.5:1), Color Palette Section (primary/secondary/neutral + WCAG), Color Hierarchy (primary/secondary/neutral/semantic), Color Usage Dos and Donts, Color Palette Management, Relative Luminance and Contrast Ratio Functions, External Color Tools (Coolors, WebAIM, Tailwind, Color Hunt), WCAG 2.1 Contrast Ratio Requirements (+3 more)

### Community 74 - "Brand Context Extraction Script"
Cohesion: 0.25
Nodes (8): adjustBrightness(), { execFileSync }, extractColorsFromMarkdown(), fs, generateColorScale(), main(), path, updateDesignTokens()

### Community 75 - "Logo and CIP Prompt Engineering"
Cohesion: 0.22
Nodes (9): CIP Base Prompt Structure, CIP Mockup Prompt Engineering, Lighting and Context Modifiers, CIP Quality Modifiers and Negative Prompts, Core Prompt Structure, Logo AI Prompt Engineering, Logo Negative Prompts (NOT photorealistic, no text, no busy background), Logo Prompt Templates (quick, detailed brief, variation) (+1 more)

### Community 76 - "AOT Analyzer Constraints"
Cohesion: 0.18
Nodes (7): Analyzers Cannot See Runtime AOT Failures, App.axaml.cs Does Not Call AvaloniaXamlLoader For Views, ViewLocator Generated By StaticViewLocator 0.7.0, ViewLocator Is Forward Looking Not Load Bearing, Trim And AOT Analyzers Enabled On Plain Build, Views Resolved Without Reflection Generated Table, Solution Layout Tree

### Community 77 - "ProTranslate Binding Rules"
Cohesion: 0.24
Nodes (6): A Missing Key Renders The Key Text, ProTranslateStrings Raises PropertyChanged Per Key, Method One Binding Strings Key, Runtime Symptom To Cause Table, Translation Does Not Work Checklist, Empty String Is A Valid Translation

### Community 78 - "Brand Context Injection Script"
Cohesion: 0.31
Nodes (10): extractColorsFromTable(), extractCoreAttributes(), extractHexColors(), extractImageStyle(), extractTypography(), extractVoice(), fs, generatePromptAddition() (+2 more)

### Community 79 - "Design Tokens Primitive Values"
Cohesion: 0.18
Nodes (11): fast, normal, slow, $type, $value, $type, $value, primitive (+3 more)

### Community 80 - "Design Skill Routing"
Cohesion: 0.29
Nodes (9): Design Routing Guide, Multi-Skill Workflows (new project, migration, component creation), Routing by Question Type, Routing by Task Type, When to Use Multiple Skills, Design Skill (Unified), New Design System Workflow (brand to tokens to implementation), External Sub-skills (brand, design-system, ui-styling) (+1 more)

### Community 81 - "Slide Skill References"
Cohesion: 0.33
Nodes (10): search-slides.py (BM25 search + contextual recommendations), BM25 Slide Search (search-slides.py), Slide System (token-driven HTML decks), Copywriting Formulas Reference, Slides Create Subcommand, HTML Slide Template Reference, Layout Patterns Reference, Slide Strategies Reference (+2 more)

### Community 83 - "Design Tokens Button Tokens"
Cohesion: 0.20
Nodes (10): fg, font-size, hover-bg, button, $type, $value, $type, $value (+2 more)

### Community 85 - "Color Psychology and Harmony"
Cohesion: 0.31
Nodes (7): CIP Color Psychology Table, CIP Design Style Guide, Color Harmony Types (monochromatic, complementary, analogous, triadic), Color Combinations by Industry, Logo Color Psychology, Primary Color Meanings (blue, red, green, gold, purple, orange, black, white), Quick Reference Palettes

### Community 86 - "CIP Search CLI"
Cohesion: 0.28
Nodes (4): scripts/cip/core.py (BM25 search engine), format_brief(), format_results(), main()

### Community 87 - "Catalog Validation"
Cohesion: 0.28
Nodes (8): _catalog_date(), _check_catalog_contract(), _check_catalog_summary(), _check_font_catalog(), _check_phosphor_catalog(), _imported_weights(), _load_catalog_json(), _valid_google_fonts_exclusion_source()

### Community 90 - "Asset Organization and Sync"
Cohesion: 0.29
Nodes (6): Asset Organization Guide, Color Documentation Formats (markdown, CSS vars, Tailwind), CSS Variable and Tailwind Implementation, Sync to Design Tokens Step, Design Tokens (design-tokens.json / design-tokens.css), Starter Design Components (buttons, spacing, radii)

### Community 91 - "CIP Deliverables and Mockups"
Cohesion: 0.29
Nodes (8): Banner Print Specs (300 DPI, 3-5mm bleed, CMYK), CIP Deliverable Guide, Deliverable Specifications (sizes, materials, formats), Logo Variation Set (horizontal, vertical, stacked, monochrome), CIP Design Reference, CIP Mockup Generation, Deliverable Categories (50+ items, 10 categories), Gemini Nano Banana Image Models (Flash/Pro)

### Community 92 - "Design Tokens Input Tokens"
Cohesion: 0.29
Nodes (8): padding-x, input, $type, $value, focus-ring, padding-x, $type, $value

### Community 93 - "Design Tokens Radius Tokens"
Cohesion: 0.29
Nodes (8): $type, $value, $type, $value, radius, default, full, default

### Community 94 - "Art Direction Styles"
Cohesion: 0.33
Nodes (7): 22 Art Direction Styles, Print Finishes (matte, spot UV, foil, emboss, deboss), Deliverable-Specific Modifiers, CIP Design Styles (8 styles with colors, typography, materials, finishes), Style Keyword Library, Aesthetic Styles (minimalist, vintage, luxury, geometric, organic, gradient), Art Direction Styles Reused from Banner

### Community 95 - "Sub-skill Scope Boundaries"
Cohesion: 0.33
Nodes (5): Pinterest Art Direction Research Queries, CIP HTML Presentation (base64 single-file, dark theme), Banner Design Built-in Sub-skill, Related Skills (frontend-design, ui-ux-pro-max, ai-multimodal, chrome-devtools), Social Photos Built-in Sub-skill

### Community 96 - "Icon Design Reference"
Cohesion: 0.33
Nodes (5): 12 Icon Categories, Icon Generator CLI Options, Icon Design Reference, Icon Multi-size Export (16,24,32,48), gemini-3.1-pro-preview (text-only SVG output)

### Community 97 - "Slide HTML Layout and Charts"
Cohesion: 0.29
Nodes (7): Chart.js Integration for Slides, Slide Requirements (navigation, centered content, Chart.js), HTML Template Chart.js Integration, Slide Navigation Controls (keyboard/click/progress bar), Responsive Breakpoints (tablet 768px / mobile 480px), 16:9 Slide Deck Container (letterboxed), Layout CSS Structures (grid/flex with breakpoints)

### Community 101 - "Designer Container and Finalizer Rules"
Cohesion: 0.40
Nodes (3): Designer Container AppServices Instance, Bug Four Uncached Instance Plus Finalizer Destroyed Singletons, Dispose Review Checklist

### Community 102 - "Project Overview and Boundaries"
Cohesion: 0.33
Nodes (5): IMatrixCutter Default Interface Methods 3 Abstract 6 Default, MouseLab Project Overview, Project Boundaries Core / Services / AvaloniaApp, MouseLabSolution.slnx Lives In src/, MouseRepo README

### Community 103 - "ProTranslate Per-Project Generation"
Cohesion: 0.47
Nodes (5): ProTranslate Package Dependency Graph, Moving ViewModels To Another Project, ProTranslate Adapters For Avalonia, ProTranslate Core UI Free Services, RTL Auto Flow Direction

### Community 104 - "Translation Key Authoring Workflow"
Cohesion: 0.40
Nodes (5): Adding A New Translation String, Four Generated Members Per Key, Method Three Translation Key Attached Property, Method Two Format In The ViewModel, Analyzer Diagnostics PTA001 To PTA005

### Community 105 - "Culture and Theme Change Events"
Cohesion: 0.33
Nodes (4): CultureChangedEventArgs, NewCulture, ThemeChangedEventArgs, NewTheme

### Community 106 - "Design Tokens Shadow Values"
Cohesion: 0.47
Nodes (6): sm, shadow, sm, sm, $type, $value

### Community 107 - "CultureOption Model"
Cohesion: 0.33
Nodes (4): CultureOption, Culture, CultureName, DisplayName

### Community 111 - "Design Tokens Border Tokens"
Cohesion: 0.60
Nodes (5): $type, $value, border, border, border

### Community 112 - "Design Tokens Radius Values"
Cohesion: 0.60
Nodes (5): radius, radius, radius, $type, $value

### Community 113 - "Design Tokens Size Tokens"
Cohesion: 0.60
Nodes (5): lg, $type, $value, lg, lg

### Community 114 - "Banner Sizes and Safe Zones"
Cohesion: 0.50
Nodes (3): Complete Banner Sizes (social, display ads, web, print), Safe Zones (edge margins, YouTube/Meta safe areas), Banner Size Quick Reference (9 platforms)

### Community 115 - "Motion and Animation Tokens"
Cohesion: 0.50
Nodes (4): Motion / Duration Scale, Tailwind Animation / Duration Tokens, Slide Animation Classes (fade-up/count/scale/stagger), Layout Selection by Use Case (25 layouts)

### Community 117 - "Design Tokens Padding Tokens"
Cohesion: 0.67
Nodes (4): padding-y, padding-y, $type, $value

### Community 118 - "Design Tokens xl Values"
Cohesion: 0.67
Nodes (4): xl, xl, $type, $value

### Community 119 - "Design Tokens md Values"
Cohesion: 0.67
Nodes (4): $type, $value, md, md

### Community 120 - "Design Tokens None Values"
Cohesion: 0.67
Nodes (4): $type, $value, none, none

### Community 121 - "Vector Value Type"
Cohesion: 0.50
Nodes (4): Vector, Dx, Dy, Zero

### Community 122 - "Typography Scale and Spacing"
Cohesion: 0.67
Nodes (3): Font Weight Scale and Pairing, Line Height Guidelines, Paragraph Spacing and Line Length (65-75ch)

### Community 123 - "Dialog Component Tokens"
Cohesion: 0.67
Nodes (3): Dialog Component Spec (sizes/anatomy), Dialog/Modal Component Tokens, Z-Index Scale

### Community 126 - "Adding a New Language"
Cohesion: 0.67
Nodes (3): Adding A New Language, Catalog Structure Culture From File Name, Translation Catalog Conventions

### Community 129 - "Design Tokens Destructive Token"
Cohesion: 0.67
Nodes (3): destructive, $type, $value

### Community 130 - "Design Tokens Destructive Foreground"
Cohesion: 0.67
Nodes (3): destructive-foreground, $type, $value

### Community 131 - "Design Tokens Muted Token"
Cohesion: 0.67
Nodes (3): muted, $type, $value

### Community 132 - "Design Tokens Primary Foreground"
Cohesion: 0.67
Nodes (3): primary-foreground, $type, $value

### Community 133 - "Design Tokens Ring Token"
Cohesion: 0.67
Nodes (3): ring, $type, $value

### Community 134 - "Design Tokens Secondary Foreground"
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
- **333 isolated node(s):** `$schema`, `plugin`, `fs`, `path`, `fs` (+328 more)
  These have ≤1 connection - possible missing edges or undocumented components. (Counts symbols only; 822 node(s) total have ≤1 connection when file, concept and rationale nodes are included.)
- **61 thin communities (<3 nodes) omitted from report** — run `graphify query` to explore isolated nodes.

## Suggested Questions
_Questions this graph is uniquely positioned to answer:_

- **What is the exact relationship between `Banner Output Path Convention (assets/banners/{campaign})` and `Naming Convention type_campaign_description_timestamp_variant`?**
  _Edge tagged AMBIGUOUS (relation: conceptually_related_to) - confidence is low._
- **Why does `TailwindConfigGenerator` connect `Tailwind Config Generator API` to `Token Validator Wrappers`, `Tailwind Config Tests`, `Tailwind Config Palette Test`, `Tailwind Config Breakpoints Test`, `Tailwind Config Plugin Recs Test`, `Tailwind Config Colors Test`, `Tailwind Config Plugins Test`, `Tailwind Config Validation Test`, `Tailwind Config Write Test`, `Tailwind Config JS Init Test`, `Tailwind Config Write Content Test`, `Tailwind Config Invalid Path Test`, `Tailwind Config TypeScript Test`, `Tailwind Config Output Path Test`, `Tailwind Config Base Structure Test`, `Tailwind Config React Paths Test`, `Tailwind Config Next.js Paths Test`, `Tailwind Config Custom Colors Test`, `Tailwind Config Emission`, `Tailwind Config Base Setup`, `Generated Config Validity Tests`, `Tailwind Config CLI Entry`?**
  _High betweenness centrality (0.054) - this node is a cross-community bridge._
- **Are the 2 inferred relationships involving `TailwindConfigGenerator` (e.g. with `TestGeneratedConfigIsValidJs` and `TestTailwindConfigGenerator`) actually correct?**
  _`TailwindConfigGenerator` has 2 INFERRED edges - model-reasoned connections that need verification._
- **What connects `$schema`, `plugin`, `fs` to the rest of the system?**
  _333 weakly-connected nodes found - possible documentation gaps or missing edges._
- **Should `BM25 Search Core (scripts)` be split into smaller, more focused modules?**
  _Cohesion score 0.05563093622795115 - nodes in this community are weakly interconnected._
- **What is the exact relationship between `Logo Usage Section (variants, clear space, minimum size)` and `Logo Minimum Size (digital 120px/24px, print 35mm/10mm)`?**
  _Edge tagged AMBIGUOUS (relation: shares_data_with) - confidence is low._
- **Why does `ShadcnInstaller` connect `shadcn Installer Tests` to `Token Validator Wrappers`, `shadcn Installer API`, `shadcn Installer Init`?**
  _High betweenness centrality (0.020) - this node is a cross-community bridge._