using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace RoslynMemberSorter;

/// <summary>
/// Provides code fixes for <see cref="MemberSorterAnalyzer" />.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class FixAllCodeFixProvider : MemberSorterCodeFixProviderBase
{
	/// <inheritdoc />
	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		context.RegisterCodeFix(CodeAction.Create("Fix all member order", cancellationToken => OrderMembersAsync(context, true, cancellationToken), context.Diagnostics[0].Id + ".all"), context.Diagnostics[0]);
		return Task.CompletedTask;
	}
}
