using System;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

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
	} = ImmutableArray.Create(DiagnosticIds.SortMembers);

	/// <inheritdoc />
	public override void Initialize(AnalysisContext context)
	{
		context.EnableConcurrentExecution();
		context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
		context.RegisterSyntaxNodeAction(AnalyzeFileScopedNamespace, SyntaxKind.FileScopedNamespaceDeclaration);
		context.RegisterSyntaxNodeAction(AnalyzeNamespace, SyntaxKind.NamespaceDeclaration);
		context.RegisterSyntaxNodeAction(AnalyzeType, SyntaxKind.ClassDeclaration, SyntaxKind.InterfaceDeclaration, SyntaxKind.RecordDeclaration, SyntaxKind.RecordStructDeclaration, SyntaxKind.StructDeclaration);
	}

	/// <summary>
	/// Analyzes a file scoped namespace to see if its members are in order.
	/// </summary>
	/// <param name="context">The context of the analysis.</param>
	private void AnalyzeFileScopedNamespace(SyntaxNodeAnalysisContext context)
	{
		var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
		var comparer = new DeclarationComparer(DeclarationComparerOptions.FromAnalyzerConfigOptions(options));
		var fileScopedNamespaceDeclaration = (FileScopedNamespaceDeclarationSyntax)context.Node;
		if (!fileScopedNamespaceDeclaration.Members.IsOrdered(comparer))
		{
			context.ReportDiagnostic(Diagnostic.Create(DiagnosticIds.SortMembers, fileScopedNamespaceDeclaration.Name.GetLocation(), comparer.Options.ToImmutableDictionary()));
		}
	}

	/// <summary>
	/// Analyzes a namespace to see if its members are in order.
	/// </summary>
	/// <param name="context">The context of the analysis.</param>
	private void AnalyzeNamespace(SyntaxNodeAnalysisContext context)
	{
		var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
		var comparer = new DeclarationComparer(DeclarationComparerOptions.FromAnalyzerConfigOptions(options));
		var namespaceDeclaration = (NamespaceDeclarationSyntax)context.Node;
		if (!namespaceDeclaration.Members.IsOrdered(comparer))
		{
			context.ReportDiagnostic(Diagnostic.Create(DiagnosticIds.SortMembers, namespaceDeclaration.Name.GetLocation(), comparer.Options.ToImmutableDictionary()));
		}
	}

	/// <summary>
	/// Analyzes a type to see if its members are in order.
	/// </summary>
	/// <param name="context">The context of the analysis.</param>
	private void AnalyzeType(SyntaxNodeAnalysisContext context)
	{
		var options = context.Options.AnalyzerConfigOptionsProvider.GetOptions(context.Node.SyntaxTree);
		var comparer = new DeclarationComparer(DeclarationComparerOptions.FromAnalyzerConfigOptions(options));
		var typeDeclaration = (TypeDeclarationSyntax)context.Node;
		if (!typeDeclaration.Members.IsOrdered(comparer))
		{
			context.ReportDiagnostic(Diagnostic.Create(DiagnosticIds.SortMembers, typeDeclaration.Identifier.GetLocation(), comparer.Options.ToImmutableDictionary()));
		}
	}
}
