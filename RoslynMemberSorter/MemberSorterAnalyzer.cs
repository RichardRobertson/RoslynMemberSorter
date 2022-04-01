using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynMemberSorter.Comparers.CSharp;

namespace RoslynMemberSorter;

/// <summary>
/// Analyzes namespaces and types to check if their declared members are in order.
/// </summary>
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MemberSorterAnalyzer : DiagnosticAnalyzer
{
	/// <inheritdoc />
	public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics
	{
		get;
	} = ImmutableArray.Create
	(
		DiagnosticIds.AccessibilityOutOfOrder,
		DiagnosticIds.ExplicitInterfaceSpecifierOutOfOrder,
		DiagnosticIds.FieldOutOfOrder,
		DiagnosticIds.IdentifierOutOfOrder,
		DiagnosticIds.KindOutOfOrder,
		DiagnosticIds.ParameterArityOutOfOrder,
		DiagnosticIds.ParameterNameOutOfOrder,
		DiagnosticIds.ParameterTypeOutOfOrder,
		DiagnosticIds.StaticOutOfOrder
	);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.FileScopedNamespaceDeclaration, SyntaxKind.NamespaceDeclaration);
		context.RegisterSyntaxNodeAction(AnalyzeType, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration, SyntaxKind.StructDeclaration);
	}

	/// <summary>
	/// Analyzes a member list to see if they are in order.
	/// </summary>
	/// <param name="context">The context of the analysis.</param>
	/// <param name="members">The list of members.</param>
	private void AnalyzeMembers(SyntaxNodeAnalysisContext context, SyntaxList<MemberDeclarationSyntax> members)
	{
		var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
		var declarationComparerOptions = DeclarationComparerOptions.FromAnalyzerConfigOptions(options);
		var comparer = declarationComparerOptions.ToCSharpComparer();
		foreach ((var member, var comparerMatched) in comparer.FindUnordered(members))
		{
			var diagnostic = comparerMatched switch
			{
				AccessibilityComparer => DiagnosticIds.AccessibilityOutOfOrder,
				FieldDeclarationMutabilityComparer => DiagnosticIds.FieldOutOfOrder,
				HasExplicitInterfaceSpecifierComparer => DiagnosticIds.ExplicitInterfaceSpecifierOutOfOrder,
				IdentifierComparer => DiagnosticIds.IdentifierOutOfOrder,
				IsStaticComparer => DiagnosticIds.StaticOutOfOrder,
				KindComparer => DiagnosticIds.KindOutOfOrder,
				ParameterArityComparer => DiagnosticIds.ParameterArityOutOfOrder,
				ParameterTypeComparer => DiagnosticIds.ParameterTypeOutOfOrder,
				ParameterNameComparer => DiagnosticIds.ParameterNameOutOfOrder,
				_ => null
			};
			if (diagnostic is not null)
			{
				context.ReportDiagnostic(Diagnostic.Create(diagnostic, member.GetLocation(), declarationComparerOptions.ToImmutableDictionary(), DiagnosticIds.ProvideMessageParameters(diagnostic, member, members.Before(member))));
			}
		}
	}

	/// <summary>
	/// Analyzes a namespace to see if its members are in order.
	/// </summary>
	/// <param name="context">The context of the analysis.</param>
	private void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
	{
		AnalyzeMembers(context, ((BaseNamespaceDeclarationSyntax)context.Node).Members);
	}

	/// <summary>
	/// Analyzes a type to see if its members are in order.
	/// </summary>
	/// <param name="context">The context of the analysis.</param>
	private void AnalyzeType(SyntaxNodeAnalysisContext context)
	{
		AnalyzeMembers(context, ((TypeDeclarationSyntax)context.Node).Members);
	}
}
