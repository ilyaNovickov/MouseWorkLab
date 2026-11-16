using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;


namespace MouseLabAvaloniaDesktop.Generators
{
    [Generator]
    public class CopyFromGenerator : IIncrementalGenerator
    {
        public void Initialize(IncrementalGeneratorInitializationContext context)
        {
            // 1. Внедряем атрибут в компиляцию
            context.RegisterPostInitializationOutput(ctx =>
            {
                ctx.AddSource(
                    "GenerateCopyFromAttribute.g.cs",
                    SourceText.From(AttributeSource.Code, Encoding.UTF8));
            });

            // 2. Находим все классы с [GenerateCopyFrom]
            var candidates = context.SyntaxProvider.ForAttributeWithMetadataName(
                fullyQualifiedMetadataName: AttributeSource.CopyFromFullName,
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetClassInfo(ctx))
                .Where(static c => c is not null)
                .Select(static (c, _) => c!.Value);

            // 3. Генерируем код
            context.RegisterSourceOutput(candidates, static (spc, classInfo) =>
            {
                string source = GenerateCopyFromMethod(classInfo, spc);
                spc.AddSource($"{classInfo.ClassName}.CopyFrom.g.cs",
                    SourceText.From(source, Encoding.UTF8));
            });
        }

        // ── Сбор информации о классе ──────────────────────
        private enum CopyStrategy
        {
            ValueOrString,       // this.X = source.X;
            ICopyable,           // this.X.CopyFrom(source.X);
            Reference,           // this.X = source.X;  (с атрибутом)
            Clone,               // this.X = (T)source.X.Clone();
            Ignore,              // пропустить
            UnknownReference     // → Warning MLAB001
        }

        private record struct PropertyInfo(
            string Name,
            string TypeFullName,
            CopyStrategy Strategy,
            LocationInfo? Location);

        private record struct ClassInfo(
            string Namespace,
            string ClassName,
            ImmutableArray<PropertyInfo> Properties);

        //private readonly record struct LocationInfo(string FilePath, TextSpan TextSpan, LinePositionSpan LineSpan)
        private readonly record struct LocationInfo
        {
            public LocationInfo(string filePath, TextSpan textSpan, LinePositionSpan lineSpan)
            {
                FilePath = filePath;
                TextSpan = textSpan;
                LineSpan = lineSpan;
            }

            public readonly string FilePath { get; }

            public readonly TextSpan TextSpan { get; }

            public readonly LinePositionSpan LineSpan { get; }

            public Location ToLocation() => Location.Create(FilePath, TextSpan, LineSpan);

            public static LocationInfo? From(ISymbol symbol)
            {
                var loc = symbol.Locations.FirstOrDefault();
                if (loc is null || loc.SourceTree is null) return null;
                return new LocationInfo(loc.SourceTree.FilePath, loc.SourceSpan, loc.GetLineSpan().Span);
            }
        }

        private static ClassInfo? GetClassInfo(GeneratorAttributeSyntaxContext ctx)
        {
            if (ctx.TargetSymbol is not INamedTypeSymbol classSymbol)
                return null;

            string ns = classSymbol.ContainingNamespace.IsGlobalNamespace
                ? string.Empty
                : classSymbol.ContainingNamespace.ToDisplayString();

            var properties = classSymbol
                .GetMembers()
                .OfType<IPropertySymbol>()
                .Where(p =>
                    !p.IsStatic &&
                    !p.IsReadOnly &&
                    !p.IsWriteOnly &&
                    p.DeclaredAccessibility == Accessibility.Public &&
                    p.GetMethod is not null &&
                    p.SetMethod is not null)
                .Select(p =>
                    {
                        var strategy = DetermineStrategy(p, classSymbol);
                        return new PropertyInfo(
                            Name: p.Name,
                            TypeFullName: p.Type.ToDisplayString(),
                            Strategy: strategy,
                            Location: LocationInfo.From(p));
                    })
                .ToImmutableArray();

            return new ClassInfo(ns, classSymbol.Name, properties);
        }

        private static CopyStrategy DetermineStrategy(
    IPropertySymbol property,
    INamedTypeSymbol classSymbol)
        {
            var type = property.Type;

            // 1. Атрибуты на самом свойстве
            var fromProp = TryReadAttrStrategy(property.GetAttributes());
            if (fromProp is not null) return fromProp.Value;

            // 2. Атрибуты на бэкинг-филде (для [ObservableProperty])
            var backingField = FindObservableBackingField(property, classSymbol);
            if (backingField is not null)
            {
                var fromField = TryReadAttrStrategy(backingField.GetAttributes());
                if (fromField is not null) return fromField.Value;
            }

            // 3. Value type или string
            if (type.IsValueType || type.SpecialType == SpecialType.System_String)
                return CopyStrategy.ValueOrString;

            // 4. ICopyable<T>
            bool implementsICopyable = type.AllInterfaces.Any(i =>
                i.IsGenericType &&
                i.OriginalDefinition.ToDisplayString() ==
                    "MouseLabAvaloniaDesktop.Models.Settings.ICopyable<T>" &&
                SymbolEqualityComparer.Default.Equals(i.TypeArguments[0], type));

            if (implementsICopyable)
                return CopyStrategy.ICopyable;

            // 5. Неизвестный ссылочный тип
            return CopyStrategy.UnknownReference;
        }

        private static CopyStrategy? TryReadAttrStrategy(ImmutableArray<AttributeData> attributes)
        {
            foreach (var attr in attributes)
            {
                string attrName = attr.AttributeClass?.ToDisplayString() ?? "";

                if (attrName == AttributeSource.IgnoreFullName) return CopyStrategy.Ignore;
                if (attrName == AttributeSource.ReferenceFullName) return CopyStrategy.Reference;
                if (attrName == AttributeSource.CloneFullName) return CopyStrategy.Clone;
            }
            return null;
        }

        /// <summary>
        /// Находит поле, из которого [ObservableProperty] сгенерировал свойство.
        /// Поддерживает соглашения: recentFiles, _recentFiles, m_recentFiles.
        /// </summary>
        private static IFieldSymbol? FindObservableBackingField(
            IPropertySymbol property,
            INamedTypeSymbol classSymbol)
        {
            string prop = property.Name;
            if (string.IsNullOrEmpty(prop)) return null;

            string camel = char.ToLowerInvariant(prop[0]) + prop.Substring(1);

            return classSymbol
                .GetMembers()
                .OfType<IFieldSymbol>()
                .FirstOrDefault(f =>
                    f.Name == camel ||
                    f.Name == "_" + camel ||
                    f.Name == "m_" + camel);
        }

        // ── Генерация кода ────────────────────────────────

        private static string GenerateCopyFromMethod(
    ClassInfo info,
    SourceProductionContext spc)
        {
            var sb = new StringBuilder();

            sb.AppendLine("// <auto-generated/>");
            sb.AppendLine("#nullable enable");
            sb.AppendLine();

            if (!string.IsNullOrEmpty(info.Namespace))
            {
                sb.AppendLine($"namespace {info.Namespace}");
                sb.AppendLine("{");
            }

            sb.AppendLine(
                $"    partial class {info.ClassName} " +
                $": MouseLabAvaloniaDesktop.Models.Settings.ICopyable<{info.ClassName}>");
            sb.AppendLine("    {");
            sb.AppendLine(
                $"        public void CopyFrom({info.ClassName} source)");
            sb.AppendLine("        {");

            foreach (var prop in info.Properties)
            {
                switch (prop.Strategy)
                {
                    case CopyStrategy.ValueOrString:
                    case CopyStrategy.Reference:
                        sb.AppendLine(
                            $"            this.{prop.Name} = source.{prop.Name};");
                        break;

                    case CopyStrategy.ICopyable:
                        sb.AppendLine(
                            $"            this.{prop.Name}.CopyFrom(source.{prop.Name});");
                        break;

                    case CopyStrategy.Clone:
                        sb.AppendLine(
                            $"            this.{prop.Name} = " +
                            $"({prop.TypeFullName})source.{prop.Name}.Clone();");
                        break;

                    case CopyStrategy.Ignore:
                        sb.AppendLine(
                            $"            // [CopyFromIgnore] {prop.Name}");
                        break;

                    case CopyStrategy.UnknownReference:
                        spc.ReportDiagnostic(Diagnostic.Create(
                            new DiagnosticDescriptor(
                                id: "MLAB001",
                                title: "Unsupported property type for CopyFrom",
                                messageFormat:
                                    "Property '{0}' of type '{1}' in class '{2}' " +
                                    "is a reference type without ICopyable<T>. " +
                                    "Use [CopyFromReference], [CopyFromClone], " +
                                    "or [CopyFromIgnore].",
                                category: "MouseLab.Generators",
                                defaultSeverity: DiagnosticSeverity.Warning,
                                isEnabledByDefault: true),
                            prop.Location?.ToLocation() ?? Location.None,
                            prop.Name,
                            prop.TypeFullName,
                            info.ClassName));

                        sb.AppendLine(
                            $"            // WARNING: reference copy for " +
                            $"'{prop.Name}' ({prop.TypeFullName})");
                        sb.AppendLine(
                            $"            this.{prop.Name} = source.{prop.Name};");
                        break;
                }
            }

            // Всегда вызываем хук. Если пользователь не реализует его,
            // partial-метод без тела компилируется в no-op — call также исчезнет.
            sb.AppendLine();
            sb.AppendLine($"            OnCopyFromCompleted(source);");

            sb.AppendLine("        }");
            sb.AppendLine();

            // Всегда объявляем partial-метод (defining declaration).
            // Пользователь может — но не обязан — добавить implementing declaration с телом.
            sb.AppendLine($"        /// <summary>");
            sb.AppendLine($"        /// Hook, вызываемый в конце CopyFrom(). Реализуйте при необходимости.");
            sb.AppendLine($"        /// </summary>");
            sb.AppendLine($"        partial void OnCopyFromCompleted({info.ClassName} source);");

            sb.AppendLine("    }");

            if (!string.IsNullOrEmpty(info.Namespace))
                sb.AppendLine("}");

            return sb.ToString();
        }
    }
}
