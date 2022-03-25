using System;
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
	[Obsolete]
	public static readonly DiagnosticDescriptor SortMembers = new("RMS0001", "Sort members", "Sort members", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0002 indicating a particular member is out of order when sorted by kind.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:kind, 2:name, 3:kind</remarks>
	public static readonly DiagnosticDescriptor KindOutOfOrder = new("RMS0002", "Member kind out of order", "Member {0} is kind {1} and should come before {2} of kind {3}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0003 indicating a particular member is out of order when sorted by static or instance.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:"static"/"instance", 2:name, 3:"static"/"instance"</remarks>
	public static readonly DiagnosticDescriptor StaticOutOfOrder = new("RMS0003", "Member static out of order", "Member {0} is {1} and should come before {2} which is {3}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0004 indicating a field is out of order when sorted by mutability.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:mutability, 2:name, 3:mutability</remarks>
	public static readonly DiagnosticDescriptor FieldOutOfOrder = new("RMS0004", "Field mutability out of order", "Field {0} is {1} and should come before {2} which is {3}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0005 indicating a particular member is out of order when sorted by static or instance.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:accessibility, 2:name, 3:accessiblity</remarks>
	public static readonly DiagnosticDescriptor AccessibilityOutOfOrder = new("RMS0005", "Member accessibility out of order", "Member {0} is {1} and should come before {2} which is {3}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0006 indicating a particular member is out of order when sorted by static or instance.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:"has"/"does not have", 2:name, 3:" not"/""</remarks>
	public static readonly DiagnosticDescriptor ExplicitInterfaceSpecifierOutOfOrder = new("RMS0006", "Member explicit interface specifier out of order", "Member {0} {1} an explicit interface specifier and should come before {2} which does{3}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0007 indicating a particular member is out of order when sorted by identifier.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:name</remarks>
	public static readonly DiagnosticDescriptor IdentifierOutOfOrder = new("RMS0007", "Member name out of order", "Member {0} should come before {1}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0008 indicating a particular member is out of order when sorted by identifier.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:name, 2:"arity"/"types"/"names"</remarks>
	public static readonly DiagnosticDescriptor ParametersOutOfOrder = new("RMS0008", "Member parameter list out of order", "Member {0} should come before {1} based on parameter {2}.", "Style", DiagnosticSeverity.Info, true);
}
