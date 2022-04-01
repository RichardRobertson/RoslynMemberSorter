using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

public sealed class IdentifierComparer : IComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Initializes a new <see cref="IdentifierComparer" /> with the given quality order.
	/// </summary>
	/// <param name="nameOrder">How names should be sorted.</param>
	/// <param name="operatorTokenOrder">The order in which operator tokens should be sorted.</param>
	/// <param name="unknownOperatorTokenOrder">Where operator tokens not found in <paramref name="operatorTokenOrder" /> should be sorted.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="nameOrder" /> or <paramref name="unknownOperatorTokenOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	public IdentifierComparer(IdentifierOrder nameOrder, ImmutableArray<SyntaxKind> operatorTokenOrder, Order unknownOperatorTokenOrder)
	{
		nameOrder.AssertValid();
		NameOrder = nameOrder;
		OperatorComparer = new OperatorDeclarationTokenComparer(operatorTokenOrder, unknownOperatorTokenOrder);
	}

	/// <summary>
	/// Gets a value indicating how names should be sorted.
	/// </summary>
	/// <value>One of the <see cref="Enums.IdentifierOrder" /> enum values indicating how to sort names.</value>
	public IdentifierOrder NameOrder
	{
		get;
	}

	/// <summary>
	/// Gets the comparer used for <see cref="OperatorDeclarationSyntax" /> members since they use tokens instead of words for identifiers.
	/// </summary>
	public OperatorDeclarationTokenComparer OperatorComparer
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
		else if (x is OperatorDeclarationSyntax xOperator && y is OperatorDeclarationSyntax yOperator)
		{
			return OperatorComparer.Compare(xOperator, yOperator);
		}
		else if (NameOrder == IdentifierOrder.Default)
		{
			return 0;
		}
		var xName = GetName(x);
		var yName = GetName(y);
		var nameComparison = StringComparer.Ordinal.Compare(xName, yName);
		return NameOrder == IdentifierOrder.Alphabetical ?  nameComparison : -nameComparison;
	}

	/// <summary>
	/// Tries to get an identifier for <paramref name="member" />.
	/// </summary>
	/// <param name="member">The member to check.</param>
	/// <returns>A <see cref="string" /> indicating the name of the member; otherwise <see langword="null" /> if it does not have one.</returns>
	public static string? GetName(MemberDeclarationSyntax member)
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
		else if (member is IndexerDeclarationSyntax)
		{
			return "this[]";
		}
		else if (member is OperatorDeclarationSyntax mOperator)
		{
			return "operator " + mOperator.OperatorToken.ToString();
		}
		else if (member is ConstructorDeclarationSyntax)
		{
			return ".ctor()";
		}
		else if (member is DestructorDeclarationSyntax)
		{
			return "~()";
		}
		else
		{
			return null;
		}
	}

	/// <summary>
	/// Compares <see cref="OperatorDeclarationSyntax" /> objects based on their operator token.
	/// </summary>
	public sealed class OperatorDeclarationTokenComparer : IndexedComparer<OperatorDeclarationSyntax, SyntaxKind>
	{
		/// <summary>
		/// Initializes a new <see cref="OperatorDeclarationTokenComparer" /> with the given operator token order.
		/// </summary>
		/// <param name="operatorTokenOrder">The order in which operator tokens should be sorted.</param>
		/// <param name="unknownOrder">Where operator tokens not found in <paramref name="operatorTokenOrder" /> should be sorted.</param>
		/// <exception cref="InvalidEnumArgumentException"><paramref name="unknownOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
		public OperatorDeclarationTokenComparer(ImmutableArray<SyntaxKind> operatorTokenOrder, Order unknownOrder)
			: base(operatorTokenOrder, unknownOrder)
		{
		}

		/// <inheritdoc />
		protected override IEqualityComparer<SyntaxKind> QualityComparer => SyntaxFacts.EqualityComparer;

		/// <inheritdoc />
		protected override SyntaxKind ProvideQuality(OperatorDeclarationSyntax value)
		{
			return value.OperatorToken.Kind();
		}
	}
}
