using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on whether they are static.
/// </summary>
public sealed class IsStaticComparer : HasQualityComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Initializes a new <see cref="IsStaticComparer" /> with the given static order.
	/// </summary>
	/// <param name="staticOrder">How static members should be ordered compared to instance members.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="staticOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	public IsStaticComparer(Order staticOrder)
		: base(staticOrder)
	{
	}

	/// <inheritdoc />
	protected override bool HasQuality(MemberDeclarationSyntax value)
	{
		return value.Modifiers.Any(SyntaxKind.StaticKeyword) || value.Modifiers.Any(SyntaxKind.ConstKeyword);
	}
}
