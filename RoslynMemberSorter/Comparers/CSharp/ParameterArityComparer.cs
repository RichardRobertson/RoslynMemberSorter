using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on their parameter arity.
/// </summary>
public sealed class ParameterArityComparer : ParametersComparerBase
{
	/// <summary>
	/// Initializes a new <see cref="ParameterArityComparer" /> with the specified order.
	/// </summary>
	/// <param name="arityOrder">How to sort arity.</param>
	public ParameterArityComparer(ArityOrder arityOrder)
	{
		arityOrder.AssertValid();
		ArityOrder = arityOrder;
	}

	/// <summary>
	/// Gets a value indicating how to sort member arity.
	/// </summary>
	/// <value>One of the <see cref="Enums.ArityOrder" /> enum values indicating how to sort member arity.</value>
	public ArityOrder ArityOrder
	{
		get;
	}

	/// <inheritdoc />
	protected override int CompareParameterSyntaxes(SeparatedSyntaxList<ParameterSyntax> xParameters, SeparatedSyntaxList<ParameterSyntax> yParameters)
	{
		if (ArityOrder == ArityOrder.Default)
		{
			return 0;
		}
		else
		{
			var arityComparison = xParameters.Count - yParameters.Count;
			return ArityOrder == ArityOrder.LowToHigh ? arityComparison : -arityComparison;
		}
	}
}
