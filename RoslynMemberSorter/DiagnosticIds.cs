using Microsoft.CodeAnalysis;

namespace RoslynMemberSorter;

/// <summary>
/// Stores <see cref="DiagnosticDescriptor" /> values used by <see cref="MemberSorterAnalyzer" />.
/// </summary>
public static class DiagnosticIds
{
	/// <summary>
	/// The diagnostic RMS0001 to sort members of a type or namespace.
	/// </summary>
	public static readonly DiagnosticDescriptor SortMembers = new("RMS0001", "Sort members", "Sort members", "Style", DiagnosticSeverity.Info, true, null, null);
}
