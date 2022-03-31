using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;

namespace RoslynMemberSorter;

/// <summary>
/// Provides code fixes for <see cref="MemberSorterAnalyzer" />.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class MemberSorterCodeFixProvider : CodeFixProvider
{
	/// <inheritdoc />
	public override ImmutableArray<string> FixableDiagnosticIds
	{
		get;
	} = ImmutableArray.Create(DiagnosticIds.SortMembers.Id);

	/// <inheritdoc />
	public override FixAllProvider? GetFixAllProvider()
	{
		return WellKnownFixAllProviders.BatchFixer;
	}

	/// <inheritdoc />
	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		var sortMemberDiagnostic = context.Diagnostics.First(diagnostic => diagnostic.Descriptor == DiagnosticIds.SortMembers);
		context.RegisterCodeFix(
			CodeAction.Create(
				"Fix member order",
				async cancellationToken =>
				{
					var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
					if (root is null)
					{
						return context.Document;
					}
					var comparer = DeclarationComparerOptions.FromDictionary(context.Diagnostics[0].Properties).ToCSharpComparer();
					var nodeAtDiagnostic = root.FindNode(sortMemberDiagnostic.Location.SourceSpan);
					SyntaxNode newNode;
					if (nodeAtDiagnostic is NamespaceDeclarationSyntax namespaceDeclarationSyntax)
					{
						newNode = namespaceDeclarationSyntax.WithMembers(new SyntaxList<MemberDeclarationSyntax>(namespaceDeclarationSyntax.Members.OrderBy(node => node, comparer)));
					}
					else if (nodeAtDiagnostic is TypeDeclarationSyntax typeDeclarationSyntax)
					{
						newNode = typeDeclarationSyntax.WithMembers(new SyntaxList<MemberDeclarationSyntax>(typeDeclarationSyntax.Members.OrderBy(node => node, comparer)));
					}
					else
					{
						return context.Document;
					}
					root = root.ReplaceNode(nodeAtDiagnostic, newNode.WithAdditionalAnnotations(Formatter.Annotation));
					return context.Document.WithSyntaxRoot(root);
				},
				DiagnosticIds.SortMembers.Id),
			sortMemberDiagnostic);
		return Task.CompletedTask;
	}
}
