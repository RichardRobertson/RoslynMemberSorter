using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on their declared parameter types.
/// </summary>
public sealed class ParameterTypeComparer : ParametersComparerBase
{
	/// <summary>
	/// Initializes a new <see cref="ParameterTypeComparer" /> with the specified order.
	/// </summary>
	/// <param name="typeOrder">How to sort parameter type names.</param>
	/// <param name="referenceOrder">How to sort parameters with the <see langword="ref" /> modifier keyword.</param>
	public ParameterTypeComparer(IdentifierOrder typeOrder, Order referenceOrder)
	{
		typeOrder.AssertValid();
		TypeOrder = typeOrder;
		referenceOrder.AssertValid();
		ReferenceOrder = referenceOrder;
	}

	/// <summary>
	/// Gets a value indicating how to sort parameters with the <see langword="ref" /> modifier keyword.
	/// </summary>
	public Order ReferenceOrder
	{
		get;
	}

	/// <summary>
	/// Gets a value indicating how to sort parameter names.
	/// </summary>
	/// <value>One of the <see cref="IdentifierOrder" /> enum values indicating how to sort parameter type names.</value>
	public IdentifierOrder TypeOrder
	{
		get;
	}

	/// <inheritdoc />
	protected override int CompareParameterSyntaxes(SeparatedSyntaxList<ParameterSyntax> xParameters, SeparatedSyntaxList<ParameterSyntax> yParameters)
	{
		for (var i = 0; i < xParameters.Count && i < yParameters.Count; i++)
		{
			if (TypeOrder != IdentifierOrder.Default)
			{
				var comparison = StringComparer.Ordinal.Compare(xParameters[i].Type?.ToString(), yParameters[i].Type?.ToString());
				if (comparison != 0)
				{
					return TypeOrder == IdentifierOrder.Alphabetical ? comparison : -comparison;
				}
			}
			var xRef = xParameters[i].Modifiers.Any(SyntaxKind.RefKeyword);
			var yRef = yParameters[i].Modifiers.Any(SyntaxKind.RefKeyword);
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
		return 0;
	}
}
