using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Comparers.CSharp;

namespace RoslynMemberSorter;

/// <summary>
/// Stores <see cref="DiagnosticDescriptor" /> values used by <see cref="MemberSorterAnalyzer" />.
/// </summary>
public static class DiagnosticIds
{
	/// <summary>
	/// String prefix for configuration options related to accessibility sorting.
	/// </summary>
	public const string AccessibilityPrefix = "dotnet_diagnostic.rms0005.";

	/// <summary>
	/// String prefix for configuration options related to parameter arity sorting.
	/// </summary>
	public const string ArityPrefix = "dotnet_diagnostic.rms0008.";

	/// <summary>
	/// String prefix for configuration options related to explicit interface specifier sorting.
	/// </summary>
	public const string ExplicitInterfacePrefix = "dotnet_diagnostic.rms0006.";

	/// <summary>
	/// String prefix for configuration options related to field sorting.
	/// </summary>
	public const string FieldPrefix = "dotnet_diagnostic.rms0004.";

	/// <summary>
	/// String prefix for configuration options related to identifier sorting.
	/// </summary>
	public const string IdentifierPrefix = "dotnet_diagnostic.rms0007.";

	/// <summary>
	/// String prefix for configuration options related to kind sorting.
	/// </summary>
	public const string KindPrefix = "dotnet_diagnostic.rms0002.";

	/// <summary>
	/// String prefix for configuration options related to parameter name sorting.
	/// </summary>
	public const string ParameterNamePrefix = "dotnet_diagnostic.rms0010.";

	/// <summary>
	/// String prefix for configuration options related to parameter type name sorting.
	/// </summary>
	public const string ParameterTypePrefix = "dotnet_diagnostic.rms0009.";

	/// <summary>
	/// String prefix for shared configuration options related to sorting.
	/// </summary>
	public const string SharedPrefix = "dotnet_diagnostic.rms_shared.";

	/// <summary>
	/// String prefix for configuration options related to static sorting.
	/// </summary>
	public const string StaticPrefix = "dotnet_diagnostic.rms0003.";

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
	/// The diagnostic RMS0008 indicating a particular member is out of order when sorted by parameter arity.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:arity, 2:name, 3:arity
	public static readonly DiagnosticDescriptor ParameterArityOutOfOrder = new("RMS0008", "Member arity out of order", "Member {0} with arity {1} should come before {2} with arity {3}.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0008 indicating a particular member is out of order when sorted by parameter arity.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:name
	public static readonly DiagnosticDescriptor ParameterTypeOutOfOrder = new("RMS0009", "Member parameter type out of order", "Member {0} should come before {1} based on parameter types.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// The diagnostic RMS0008 indicating a particular member is out of order when sorted by parameter arity.
	/// </summary>
	/// <remarks>Message format parameters are 0:name, 1:name
	public static readonly DiagnosticDescriptor ParameterNameOutOfOrder = new("RMS0010", "Member parameter name out of order", "Member {0} should come before {1} based on parameter names.", "Style", DiagnosticSeverity.Info, true);

	/// <summary>
	/// Provides message format parameters for a diagnostic based on the two members being compared.
	/// </summary>
	/// <param name="diagnostic">The diagnostic to provide parameters for.</param>
	/// <param name="current">The current member.</param>
	/// <param name="previous">The previous member.</param>
	/// <returns>An array of <see cref="string" /> to be passed as message format parameters.</returns>
	public static string[] ProvideMessageParameters(DiagnosticDescriptor diagnostic, MemberDeclarationSyntax current, MemberDeclarationSyntax previous)
	{
		var currentName = IdentifierComparer.GetName(current) ?? "[name not found]";
		var previousName = IdentifierComparer.GetName(previous) ?? "[name not found]";
		if (ReferenceEquals(diagnostic, KindOutOfOrder))
		{
			return new string[] { currentName, KindStatus(current), previousName, KindStatus(previous) };

			static string KindStatus(MemberDeclarationSyntax member)
			{
				return member.Kind().ToString();
			}
		}
		else if (ReferenceEquals(diagnostic, StaticOutOfOrder))
		{
			return new string[] { currentName, StaticStatus(current), previousName, StaticStatus(previous) };

			static string StaticStatus(MemberDeclarationSyntax member)
			{
				return member.Modifiers.Any(SyntaxKind.StaticKeyword) || member.Modifiers.Any(SyntaxKind.ConstKeyword) ? "static" : "instance";
			}
		}
		else if (ReferenceEquals(diagnostic, FieldOutOfOrder))
		{
			return new string[] { currentName, FieldStatus(current), previousName, FieldStatus(previous) };

			static string FieldStatus(MemberDeclarationSyntax member)
			{
				if (member is FieldDeclarationSyntax mField)
				{
					return FieldDeclarationMutabilityComparer.GetFieldMutability(mField).ToString();
				}
				else
				{
					return "[not a field]";
				}
			}
		}
		else if (ReferenceEquals(diagnostic, AccessibilityOutOfOrder))
		{
			return new string[] { currentName, AccessibilityStatus(current), previousName, AccessibilityStatus(previous) };

			static string AccessibilityStatus(MemberDeclarationSyntax member)
			{
				return AccessibilityComparer.GetAccessibility(member).ToString();
			}
		}
		else if (ReferenceEquals(diagnostic, ExplicitInterfaceSpecifierOutOfOrder))
		{
			return new string[] { currentName, ExplicitInterfaceSpecifierStatus(current, true), previousName, ExplicitInterfaceSpecifierStatus(previous, false) };

			static string ExplicitInterfaceSpecifierStatus(MemberDeclarationSyntax member, bool isFirst)
			{
				if (HasExplicitInterfaceSpecifierComparer.HasExplicitInterfaceSpecifier(member))
				{
					return isFirst ? "has" : " not";
				}
				else
				{
					return isFirst ? "does not have" : string.Empty;
				}
			}
		}
		else if (ReferenceEquals(diagnostic, IdentifierOutOfOrder) || ReferenceEquals(diagnostic, ParameterTypeOutOfOrder) || ReferenceEquals(diagnostic, ParameterNameOutOfOrder))
		{
			return new string[] { currentName, previousName };
		}
		else if (ReferenceEquals(diagnostic, ParameterArityOutOfOrder))
		{
			return new string[] { currentName, ParameterArity(current), previousName, ParameterArity(previous) };

			static string ParameterArity(MemberDeclarationSyntax member)
			{
				if (member is MethodDeclarationSyntax mMethod)
				{
					return mMethod.Arity.ToString();
				}
				else if (member is IndexerDeclarationSyntax mIndexer)
				{
					return mIndexer.ParameterList.Parameters.Count.ToString();
				}
				else if (member is ConstructorDeclarationSyntax mConstructor)
				{
					return mConstructor.ParameterList.Parameters.Count.ToString();
				}
				else if (member is DelegateDeclarationSyntax mDelegate)
				{
					return mDelegate.Arity.ToString();
				}
				else
				{
					return "[unknown]";
				}
			}
		}
		else
		{
			return Array.Empty<string>();
		}
	}
}
