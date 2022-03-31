using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="FieldDeclarationSyntax" /> objects based on their field mutability.
/// </summary>
public sealed class FieldDeclarationMutabilityComparer : IndexedComparer<FieldDeclarationSyntax, FieldMutability>, IComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Initializes a new <see cref="FieldDeclarationMutabilityComparer" /> with the given mutability order.
	/// </summary>
	/// <param name="fieldMutabilityOrder">The order in which field mutability should be sorted.</param>
	/// <param name="unknownOrder">Where field mutabilities not found in <paramref name="kindOrder" /> should be sorted.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="unknownOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	public FieldDeclarationMutabilityComparer(ImmutableArray<FieldMutability> fieldMutabilityOrder, Order unknownOrder)
		: base(fieldMutabilityOrder, unknownOrder)
	{
	}

	/// <inheritdoc />
	protected override IEqualityComparer<FieldMutability> QualityComparer => EqualityComparer<FieldMutability>.Default;

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
		if (QualityList.IsDefaultOrEmpty || ReferenceEquals(x, y))
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
		else if (x is FieldDeclarationSyntax xField && y is FieldDeclarationSyntax yField)
		{
			return base.Compare(xField, yField);
		}
		else
		{
			return 0;
		}
	}

	/// <inheritdoc />
	protected override FieldMutability ProvideQuality(FieldDeclarationSyntax value)
	{
		if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.ConstKeyword)))
		{
			return FieldMutability.Const;
		}
		else if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.ReadOnlyKeyword)))
		{
			return FieldMutability.ReadOnly;
		}
		else
		{
			return FieldMutability.Mutable;
		}
	}
}
