using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CodeFixes;

namespace RoslynMemberSorter;

/// <summary>
/// Provides code fixes for <see cref="MemberSorterAnalyzer" />.
/// </summary>
[ExportCodeFixProvider(LanguageNames.CSharp)]
public sealed class FixOneCodeFixProvider : MemberSorterCodeFixProviderBase
{
	/// <inheritdoc />
	public override Task RegisterCodeFixesAsync(CodeFixContext context)
	{
		context.RegisterCodeFix(CodeAction.Create("Fix this member order", cancellationToken => OrderMembersAsync(context, false, cancellationToken), context.Diagnostics[0].Id), context.Diagnostics[0]);
		return Task.CompletedTask;
	}
}
