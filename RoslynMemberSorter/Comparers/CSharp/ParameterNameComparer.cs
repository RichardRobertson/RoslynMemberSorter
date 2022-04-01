using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on their declared parameter names.
/// </summary>
public sealed class ParameterNameComparer : ParametersComparerBase
{
	/// <summary>
	/// Initializes a new <see cref="ParameterNameComparer" /> with the specified order.
	/// </summary>
	/// <param name="nameOrder">How to sort parameters names.</param>
	public ParameterNameComparer(IdentifierOrder nameOrder)
	{
		nameOrder.AssertValid();
		NameOrder = nameOrder;
	}

	/// <summary>
	/// Gets a value indicating how to sort parameter names.
	/// </summary>
	/// <value>One of the <see cref="Enums.IdentifierOrder" /> enum values indicating how to sort parameter names.</value>
	public IdentifierOrder NameOrder
	{
		get;
	}

	/// <inheritdoc />
	protected override int CompareParameterSyntaxes(SeparatedSyntaxList<ParameterSyntax> xParameters, SeparatedSyntaxList<ParameterSyntax> yParameters)
	{
		if (NameOrder == IdentifierOrder.Default)
		{
			return 0;
		}
		for (var i = 0; i < xParameters.Count && i < yParameters.Count; i++)
		{
			var comparison = StringComparer.Ordinal.Compare(xParameters[i].Identifier.ToString(), yParameters[i].Identifier.ToString());
			if (comparison != 0)
			{
				return NameOrder == IdentifierOrder.Alphabetical ? comparison : -comparison;
			}
		}
		return 0;
	}
}
