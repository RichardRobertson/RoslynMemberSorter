using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp;
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
	} = ImmutableArray.Create
	(
		DiagnosticIds.AccessibilityOutOfOrder.Id,
		DiagnosticIds.ExplicitInterfaceSpecifierOutOfOrder.Id,
		DiagnosticIds.FieldOutOfOrder.Id,
		DiagnosticIds.IdentifierOutOfOrder.Id,
		DiagnosticIds.KindOutOfOrder.Id,
		DiagnosticIds.ParameterArityOutOfOrder.Id,
		DiagnosticIds.ParameterNameOutOfOrder.Id,
		DiagnosticIds.ParameterTypeOutOfOrder.Id,
		DiagnosticIds.StaticOutOfOrder.Id
	);

	/// <inheritdoc />
	public override FixAllProvider? GetFixAllProvider()
	{
		return WellKnownFixAllProviders.BatchFixer;
	}

	/// <inheritdoc />
	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		//context.RegisterCodeFix(CodeAction.Create("Fix all member order", cancellationToken => OrderMembersAsync(context, true, cancellationToken), context.Diagnostics[0].Id + ".all"), context.Diagnostics[0]);
		context.RegisterCodeFix(CodeAction.Create("Fix this member order", cancellationToken => OrderMembersAsync(context, false, cancellationToken), context.Diagnostics[0].Id), context.Diagnostics[0]);
		return Task.CompletedTask;
	}

	/// <summary>
	/// Reorders one or all members.
	/// </summary>
	/// <param name="context">A <see cref="CodeFixContext" /> containing context information about the diagnostics to fix. The context must only contain diagnostics with a Id included in the <see cref="FixableDiagnosticIds" /> for the current provider.</param>
	/// <param name="fixAll"><see langword="true" /> to fix all members; otherwise <see langword="false" /> to fix only the member with the given diagnostic.</param>
	/// <param name="cancellationToken">A <see cref="CancellationToken" /> used to cancel an ongoing process.</param>
	/// <returns>The modified <see cref="Document" />.</returns>
	private static async Task<Document> OrderMembersAsync(CodeFixContext context, bool fixAll, CancellationToken cancellationToken)
	{
		var diagnostic = context.Diagnostics[0];
		var root = await context.Document.GetSyntaxRootAsync(cancellationToken).ConfigureAwait(false);
		if (root is null)
		{
			return context.Document;
		}
		var comparer = DeclarationComparerOptions.FromDictionary(diagnostic.Properties).ToCSharpComparer();
		var member = (MemberDeclarationSyntax)root.FindNode(diagnostic.Location.SourceSpan);
		SyntaxNode newNode;
		var majorParent = member.Ancestors().FirstOrDefault(node => node.IsKind(SyntaxKind.InterfaceDeclaration)
			|| node.IsKind(SyntaxKind.ClassDeclaration)
			|| node.IsKind(SyntaxKind.RecordDeclaration)
			|| node.IsKind(SyntaxKind.RecordStructDeclaration)
			|| node.IsKind(SyntaxKind.StructDeclaration)
			|| node.IsKind(SyntaxKind.NamespaceDeclaration)
			|| node.IsKind(SyntaxKind.FileScopedNamespaceDeclaration));
		if (majorParent is null)
		{
			return context.Document;
		}
		if (fixAll)
		{
			if (majorParent is BaseNamespaceDeclarationSyntax baseNamespaceDeclarationSyntax)
			{
				newNode = baseNamespaceDeclarationSyntax.WithMembers(new SyntaxList<MemberDeclarationSyntax>(baseNamespaceDeclarationSyntax.Members.OrderBy(node => node, comparer)));
			}
			else if (majorParent is TypeDeclarationSyntax typeDeclarationSyntax)
			{
				newNode = typeDeclarationSyntax.WithMembers(new SyntaxList<MemberDeclarationSyntax>(typeDeclarationSyntax.Members.OrderBy(node => node, comparer)));
			}
			else
			{
				return context.Document;
			}
		}
		else if (majorParent is BaseNamespaceDeclarationSyntax baseNamespaceDeclarationSyntax)
		{
			newNode = baseNamespaceDeclarationSyntax.WithMembers(new SyntaxList<MemberDeclarationSyntax>(SortItem(baseNamespaceDeclarationSyntax.Members)));
		}
		else if (majorParent is TypeDeclarationSyntax typeDeclarationSyntax)
		{
			newNode = typeDeclarationSyntax.WithMembers(new SyntaxList<MemberDeclarationSyntax>(SortItem(typeDeclarationSyntax.Members)));
		}
		else
		{
			return context.Document;
		}
		root = root.ReplaceNode(majorParent, newNode.WithAdditionalAnnotations(Formatter.Annotation));
		return context.Document.WithSyntaxRoot(root);

		IEnumerable<MemberDeclarationSyntax> SortItem(IEnumerable<MemberDeclarationSyntax> members)
		{
			var newIndex = comparer.FindOrderedIndexForItem(members, member);
			var memberList = members.ToList();
			memberList.Remove(member);
			memberList.Insert(newIndex, member);
			return memberList;
		}
	}
}
