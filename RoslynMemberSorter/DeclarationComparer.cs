using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMemberSorter;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> values based on a number of details indicated by <see cref="Options" />.
/// </summary>
public sealed class DeclarationComparer : IComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Initializes a new instance of <see cref="DeclarationComparer" /> with the given options.
	/// </summary>
	/// <param name="options">A <see cref="DeclarationComparerOptions" /> object specifying what details to compare.</param>
	public DeclarationComparer(DeclarationComparerOptions options)
	{
		Options = options;
	}

	/// <summary>
	/// The options used by this comparer.
	/// </summary>
	/// <value>A <see cref="DeclarationComparerOptions" /> object indicating how to compare <see cref="MemberDeclarationSyntax" /> objects.</value>
	public DeclarationComparerOptions Options
	{
		get;
	}

	/// <summary>
	/// Compares two <see cref="MemberDeclarationSyntax" /> objects and returns a value indicating whether one is less than, equal to, or greater than the other.
	/// </summary>
	/// <param name="x">The first <see cref="MemberDeclarationSyntax" /> to compare.</param>
	/// <param name="y">The second <see cref="MemberDeclarationSyntax" /> to compare.</param>
	/// <returns>
	/// 	<para>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Meaning</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>Less than zero</term>
	/// 			<description><paramref name="x" /> is less than <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Zero</term>
	/// 			<description><paramref name="x" /> equals <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Greater than zero</term>
	/// 			<description><paramref name="x" /> is greater than <paramref name="y" />.</description>
	/// 		</item>
	/// 	</list>
	/// </returns>
	public int Compare(MemberDeclarationSyntax? x, MemberDeclarationSyntax? y)
	{
		if (ReferenceEquals(x, y))
		{
			return 0;
		}
		else if (x is null)
		{
			return 1;
		}
		else if (y is null)
		{
			return -1;
		}
		foreach (var sort in Options.SortOrders)
		{
			switch (sort)
			{
				case SortOrder.Kind:
					int kindComparison = CompareKind(x, y);
					if (kindComparison != 0)
					{
						return kindComparison;
					}
					break;
				case SortOrder.Static:
					int staticComparison = CompareStatic(x, y);
					if (staticComparison != 0)
					{
						return staticComparison;
					}
					break;
				case SortOrder.FieldOrder:
					int readOnlyComparison = CompareMutability(x, y);
					if (readOnlyComparison != 0)
					{
						return readOnlyComparison;
					}
					break;
				case SortOrder.Accessibility:
					int accessiblityComparison = CompareAccessibility(x, y);
					if (accessiblityComparison != 0)
					{
						return accessiblityComparison;
					}
					break;
				case SortOrder.ExplicitInterfaceSpecifier:
					int explicitInterfaceSpecifierComparison = CompareExplicitInterfaceSpecifier(x, y);
					if (explicitInterfaceSpecifierComparison != 0)
					{
						return explicitInterfaceSpecifierComparison;
					}
					break;
				case SortOrder.Identifier:
					int identifierComparison = CompareIdentifier(x, y);
					if (identifierComparison != 0)
					{
						return identifierComparison;
					}
					break;
				case SortOrder.Parameters:
					int parameterTypesComparison = CompareParameters(x, y);
					if (parameterTypesComparison != 0)
					{
						return parameterTypesComparison;
					}
					break;
			}
		}
		return 0;
	}

	/// <inheritdoc cref="Compare" />
	private int CompareAccessibility(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		return Options.AccessibilityOrder.IndexOf(GetAccessibility(x)) - Options.AccessibilityOrder.IndexOf(GetAccessibility(y));

		static Accessibility GetAccessibility(MemberDeclarationSyntax member)
		{
			if (member.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword)))
			{
				return Accessibility.Public;
			}
			else if (member.Modifiers.Any(token => token.IsKind(SyntaxKind.InternalKeyword)))
			{
				if (member.Modifiers.Any(token => token.IsKind(SyntaxKind.ProtectedKeyword)))
				{
					return Accessibility.ProtectedInternal;
				}
				else
				{
					return Accessibility.Internal;
				}
			}
			else if (member.Modifiers.Any(token => token.IsKind(SyntaxKind.ProtectedKeyword)))
			{
				if (member.Modifiers.Any(token => token.IsKind(SyntaxKind.PrivateKeyword)))
				{
					return Accessibility.PrivateProtected;
				}
				else
				{
					return Accessibility.Protected;
				}
			}
			else if (member.Modifiers.Any(token => token.IsKind(SyntaxKind.PrivateKeyword)))
			{
				return Accessibility.Private;
			}
			else
			{
				var xMajorParent = member.Ancestors().FirstOrDefault(node => node.IsKind(SyntaxKind.InterfaceDeclaration)
					|| node.IsKind(SyntaxKind.ClassDeclaration)
					|| node.IsKind(SyntaxKind.RecordDeclaration)
					|| node.IsKind(SyntaxKind.RecordStructDeclaration)
					|| node.IsKind(SyntaxKind.StructDeclaration));
				if (xMajorParent.IsKind(SyntaxKind.InterfaceDeclaration))
				{
					return Accessibility.Public;
				}
				else if (xMajorParent is null)
				{
					return Accessibility.Internal;
				}
				else
				{
					return Accessibility.Private;
				}
			}
		}
	}

	/// <inheritdoc cref="Compare" />
	private int CompareExplicitInterfaceSpecifier(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		if (Options.ExplicitInterfaceSpecifiers == Order.Default)
		{
			return 0;
		}
		var xExplicit = HasExplicitInterfaceSpecifier(x);
		var yExplicit = HasExplicitInterfaceSpecifier(y);
		if (xExplicit && !yExplicit)
		{
			return Options.ExplicitInterfaceSpecifiers == Order.First ? -1 : 1;
		}
		else if (!xExplicit && yExplicit)
		{
			return Options.ExplicitInterfaceSpecifiers == Order.First ? 1 : -1;
		}
		else
		{
			return 0;
		}

		static bool HasExplicitInterfaceSpecifier(MemberDeclarationSyntax member)
		{
			if (member is MethodDeclarationSyntax mMethod)
			{
				return mMethod.ExplicitInterfaceSpecifier is not null;
			}
			else if (member is IndexerDeclarationSyntax mIndexer)
			{
				return mIndexer.ExplicitInterfaceSpecifier is not null;
			}
			else if (member is PropertyDeclarationSyntax mProperty)
			{
				return mProperty.ExplicitInterfaceSpecifier is not null;
			}
			else if (member is EventDeclarationSyntax mEvent)
			{
				return mEvent.ExplicitInterfaceSpecifier is not null;
			}
			else
			{
				return false;
			}
		}
	}

	/// <inheritdoc cref="Compare" />
	/// <remarks>
	/// 	<para>Identifiers vary based on their types as seen in this table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Type</term>
	/// 			<description>Identifer</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="OperatorDeclarationSyntax" /></term>
	/// 			<description><see cref="OperatorDeclarationSyntax.OperatorToken" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="FieldDeclarationSyntax" /></term>
	/// 			<description><see cref="FieldDeclarationSyntax.Declaration" />.<see cref="VariableDeclarationSyntax.Variables" />.<see cref="Enumerable.First{TSource}(IEnumerable{TSource})" />.<see cref="VariableDeclaratorSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="DelegateDeclarationSyntax" /></term>
	/// 			<description><see cref="DelegateDeclarationSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="EventDeclarationSyntax" /></term>
	/// 			<description><see cref="EventDeclarationSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="EventFieldDeclarationSyntax" /></term>
	/// 			<description><see cref="EventFieldDeclarationSyntax.Declaration" />.<see cref="VariableDeclarationSyntax.Variables" />.<see cref="Enumerable.First{TSource}(IEnumerable{TSource})" />.<see cref="VariableDeclaratorSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="PropertyDeclarationSyntax" /></term>
	/// 			<description><see cref="PropertyDeclarationSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="MethodDeclarationSyntax" /></term>
	/// 			<description><see cref="MethodDeclarationSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="ConversionOperatorDeclarationSyntax" /></term>
	/// 			<description><see cref="ConversionOperatorDeclarationSyntax.Type" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="BaseTypeDeclarationSyntax" /></term>
	/// 			<description><see cref="BaseTypeDeclarationSyntax.Identifier" /></description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Anything else</term>
	/// 			<description><see langword="null" /></description>
	/// 		</item>
	/// 	</list>
	/// </remarks>
	private int CompareIdentifier(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		if (x is OperatorDeclarationSyntax xOperator && y is OperatorDeclarationSyntax yOperator)
		{
			return Options.OperatorOrder.IndexOf(xOperator.OperatorToken.Kind(), SyntaxFacts.EqualityComparer) - Options.OperatorOrder.IndexOf(yOperator.OperatorToken.Kind(), SyntaxFacts.EqualityComparer);
		}
		else if (Options.AlphabeticalIdentifiers != NameOrder.Default)
		{
			var xName = GetMemberName(x);
			var yName = GetMemberName(y);
			return StringComparer.Ordinal.Compare(xName, yName) * (Options.AlphabeticalIdentifiers == NameOrder.Alphabetical ? 1 : -1);

			static string? GetMemberName(MemberDeclarationSyntax member)
			{
				if (member is FieldDeclarationSyntax mField)
				{
					return mField.Declaration.Variables.First().Identifier.ToString();
				}
				else if (member is DelegateDeclarationSyntax mDelegate)
				{
					return mDelegate.Identifier.ToString();
				}
				else if (member is EventDeclarationSyntax mEvent)
				{
					return mEvent.Identifier.ToString();
				}
				else if (member is EventFieldDeclarationSyntax mEventField)
				{
					return mEventField.Declaration.Variables.First().Identifier.ToString();
				}
				else if (member is PropertyDeclarationSyntax mProperty)
				{
					return mProperty.Identifier.ToString();
				}
				else if (member is MethodDeclarationSyntax mMethod)
				{
					return mMethod.Identifier.ToString();
				}
				else if (member is ConversionOperatorDeclarationSyntax mConversionOperator)
				{
					return mConversionOperator.Type.ToString();
				}
				else if (member is BaseTypeDeclarationSyntax mBaseType)
				{
					return mBaseType.Identifier.ToString();
				}
				else
				{
					return null;
				}
			}
		}
		else
		{
			return 0;
		}
	}

	/// <inheritdoc cref="Compare" />
	private int CompareKind(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		if ((x.IsKind(SyntaxKind.EventDeclaration) || x.IsKind(SyntaxKind.EventFieldDeclaration)) && (y.IsKind(SyntaxKind.EventDeclaration) || y.IsKind(SyntaxKind.EventFieldDeclaration)))
		{
			if (Options.SingleLineEvents == Order.Default)
			{
				return 0;
			}
			else
			{
				var xSingleLine = x.IsKind(SyntaxKind.EventFieldDeclaration);
				var ySingleLine = y.IsKind(SyntaxKind.EventFieldDeclaration);
				if (xSingleLine && !ySingleLine)
				{
					return Options.SingleLineEvents == Order.First ? -1 : 1;
				}
				else if (!xSingleLine && ySingleLine)
				{
					return Options.SingleLineEvents == Order.First ? 1 : -1;
				}
				else
				{
					return 0;
				}
			}
		}
		else
		{
			return Options.KindOrder.IndexOf(x.Kind(), SyntaxFacts.EqualityComparer) - Options.KindOrder.IndexOf(y.Kind(), SyntaxFacts.EqualityComparer);
		}
	}

	/// <inheritdoc cref="Compare" />
	private int CompareMutability(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		if (x is not FieldDeclarationSyntax xField || y is not FieldDeclarationSyntax yField)
		{
			return 0;
		}
		return Options.FieldOrder.IndexOf(GetFieldMutability(xField)) - Options.FieldOrder.IndexOf(GetFieldMutability(yField));

		static FieldMutability GetFieldMutability(FieldDeclarationSyntax field)
		{
			if (field.Modifiers.Any(token => token.IsKind(SyntaxKind.ConstKeyword)))
			{
				return FieldMutability.Const;
			}
			else if (field.Modifiers.Any(token => token.IsKind(SyntaxKind.ReadOnlyKeyword)))
			{
				return FieldMutability.ReadOnly;
			}
			else
			{
				return FieldMutability.Mutable;
			}
		}
	}

	/// <inheritdoc cref="Compare" />
	private int CompareParameters(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		var xParameters = GetParameters(x);
		var yParameters = GetParameters(y);
		return CompareSeparatedSyntaxList(xParameters, yParameters);

		static SeparatedSyntaxList<ParameterSyntax> GetParameters(MemberDeclarationSyntax member)
		{
			if (member is MethodDeclarationSyntax mMethod)
			{
				return mMethod.ParameterList.Parameters;
			}
			else if (member is IndexerDeclarationSyntax mIndexer)
			{
				return mIndexer.ParameterList.Parameters;
			}
			else if (member is ConstructorDeclarationSyntax mConstructor)
			{
				return mConstructor.ParameterList.Parameters;
			}
			else if (member is DelegateDeclarationSyntax mDelegate)
			{
				return mDelegate.ParameterList.Parameters;
			}
			else
			{
				return default;
			}
		}
	}

	/// <summary>
	/// Compares parameter lists based on details such as arity, types, and names.
	/// </summary>
	/// <param name="x">The first list to compare.</param>
	/// <param name="y">The second list to compare.</param>
	/// <returns>
	/// 	<para>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Meaning</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>Less than zero</term>
	/// 			<description><paramref name="x" /> is less than <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Zero</term>
	/// 			<description><paramref name="x" /> equals <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Greater than zero</term>
	/// 			<description><paramref name="x" /> is greater than <paramref name="y" />.</description>
	/// 		</item>
	/// 	</list>
	/// </returns>
	private int CompareSeparatedSyntaxList(SeparatedSyntaxList<ParameterSyntax> x, SeparatedSyntaxList<ParameterSyntax> y)
	{
		if (Options.LowArity == Order.First)
		{
			if (x.Count < y.Count)
			{
				return -1;
			}
			else if (x.Count > y.Count)
			{
				return 1;
			}
		}
		else if (Options.LowArity == Order.Last)
		{
			if (x.Count < y.Count)
			{
				return 1;
			}
			else if (x.Count > y.Count)
			{
				return -1;
			}
		}
		if (Options.ParameterSortStyle == ParameterSortStyle.Default)
		{
			return 0;
		}
		else if (Options.ParameterSortStyle == ParameterSortStyle.SortTypes)
		{
			for (var i = 0; i < x.Count && i < y.Count; i++)
			{
				var comparison = StringComparer.Ordinal.Compare(x[i].Type?.ToString(), y[i].Type?.ToString());
				if (comparison != 0)
				{
					return comparison;
				}
			}
		}
		else
		{
			for (var i = 0; i < x.Count && i < y.Count; i++)
			{
				var comparison = StringComparer.Ordinal.Compare(x[i].Identifier.ToString(), y[i].Identifier.ToString());
				if (comparison != 0)
				{
					return comparison;
				}
			}
		}
		return 0;
	}

	/// <inheritdoc cref="Compare" />
	/// <remarks>Constant fields are considered static because they are not attached to an instance.</remarks>
	private int CompareStatic(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
	{
		if (Options.Static == Order.Default)
		{
			return 0;
		}
		var xStatic = x.Modifiers.Any(token => token.IsKind(SyntaxKind.StaticKeyword) || token.IsKind(SyntaxKind.ConstKeyword));
		var yStatic = y.Modifiers.Any(token => token.IsKind(SyntaxKind.StaticKeyword) || token.IsKind(SyntaxKind.ConstKeyword));
		if (xStatic && !yStatic)
		{
			return Options.Static == Order.First ? -1 : 1;
		}
		else if (!xStatic && yStatic)
		{
			return Options.Static == Order.First ? 1 : -1;
		}
		else
		{
			return 0;
		}
	}
}
