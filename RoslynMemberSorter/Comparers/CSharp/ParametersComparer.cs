using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on their declared parameters.
/// </summary>
public sealed class ParametersComparer : IComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Initializes a new <see cref="ParametersComparer" /> with the specified orders.
	/// </summary>
	/// <param name="arityOrder">How to sort arity.</param>
	/// <param name="parameterSortStyle">How to sort individual parameters.</param>
	/// <param name="referenceOrder">Where to sort members that are declared with the <see langword="in" />, <see langword="out" />, or <see langword="ref" /> modifier keywords.</param>
	public ParametersComparer(ArityOrder arityOrder, ParameterSortStyle parameterSortStyle, Order referenceOrder)
	{
		arityOrder.AssertValid();
		parameterSortStyle.AssertValid();
		referenceOrder.AssertValid();
		ArityOrder = arityOrder;
		ParameterSortStyle = parameterSortStyle;
		ReferenceOrder = referenceOrder;
	}

	/// <summary>
	/// Gets a value indicating how to sort member arity.
	/// </summary>
	/// <value>One of the <see cref="Enums.ArityOrder" /> enum values indicating how to sort member arity.</value>
	public ArityOrder ArityOrder
	{
		get;
	}

	/// <summary>
	/// Gets a value indicating how to sort individual parameters.
	/// </summary>
	/// <value>One of the <see cref="Enums.ParameterSortStyle" /> enum values indicating how to sort individual parameters.</value>
	public ParameterSortStyle ParameterSortStyle
	{
		get;
	}

	/// <summary>
	/// Gets a value indicating how to sort reference parameters.
	/// </summary>
	/// <value>One of the <see cref="Order" /> enum values indicating how to sort parameters that are declared with the <see langword="in" />, <see langword="out" />, or <see langword="ref" /> modifier keywords..</value>
	public Order ReferenceOrder
	{
		get;
	}

	/// <summary>
	/// Compares two objects and returns a value indicating whether one should come before the other.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <returns>
	/// 	<para>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Meaning</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>Less than zero</term>
	/// 			<description><paramref name="x" /> should come before <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Zero</term>
	/// 			<description><paramref name="x" /> is sort equivalent to <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Greater than zero</term>
	/// 			<description><paramref name="x" /> should come after <paramref name="y" />.</description>
	/// 		</item>
	/// 	</list>
	/// </returns>
	public int Compare(MemberDeclarationSyntax x, MemberDeclarationSyntax y)
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
		var xParameters = GetParameterSyntaxes(x);
		var yParameters = GetParameterSyntaxes(y);
		var arityComparison = xParameters.Count - yParameters.Count;
		if (ArityOrder != ArityOrder.Default && arityComparison != 0)
		{
			return ArityOrder == ArityOrder.LowToHigh ? arityComparison : -arityComparison;
		}
		if (ParameterSortStyle == ParameterSortStyle.SortTypes)
		{
			for (var i = 0; i < xParameters.Count && i < yParameters.Count; i++)
			{
				var comparison = StringComparer.Ordinal.Compare(xParameters[i].Type?.ToString(), yParameters[i].Type?.ToString());
				if (comparison != 0)
				{
					return comparison;
				}
				var xRef = xParameters[i].Modifiers.Any(token => token.IsKind(SyntaxKind.RefKeyword));
				var yRef = yParameters[i].Modifiers.Any(token => token.IsKind(SyntaxKind.RefKeyword));
				if (ReferenceOrder == Order.First)
				{
					if (xRef && !yRef)
					{
						return -1;
					}
					else if (!xRef && yRef)
					{
						return 1;
					}
				}
				else if (ReferenceOrder == Order.Last)
				{
					if (xRef && !yRef)
					{
						return 1;
					}
					else if (!xRef && yRef)
					{
						return -1;
					}
				}
			}
		}
		else if (ParameterSortStyle == ParameterSortStyle.SortNames)
		{
			for (var i = 0; i < xParameters.Count && i < yParameters.Count; i++)
			{
				var comparison = StringComparer.Ordinal.Compare(xParameters[i].Identifier.ToString(), yParameters[i].Identifier.ToString());
				if (comparison != 0)
				{
					return comparison;
				}
			}
		}
		return 0;
	}

	/// <summary>
	/// Gets the <see cref="SeparatedSyntaxList{TNode}" /> of <see cref="ParameterSyntax" /> from a given member.
	/// </summary>
	/// <param name="member">The member to get a parameter list from.</param>
	/// <returns>A <see cref="SeparatedSyntaxList{TNode}" /> of <see cref="ParameterSyntax" /> from <paramref name="member" />'s ParameterList property if it is a <see cref="SyntaxKind.MethodDeclaration" />, <see cref="SyntaxKind.IndexerDeclaration" />, <see cref="SyntaxKind.ConstructorDeclaration" />, or <see cref="SyntaxKind.DelegateDeclaration" />; otherwise returns <see langword="default" />.</returns>
	private static SeparatedSyntaxList<ParameterSyntax> GetParameterSyntaxes(MemberDeclarationSyntax member)
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
