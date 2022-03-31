using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on whether they explicitly implement an interface.
/// </summary>
public sealed class HasExplicitInterfaceSpecifierComparer : HasQualityComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Initializes a new <see cref="IsStaticComparer" /> with the given explicit interface specifier order.
	/// </summary>
	/// <param name="explicitInterfaceSpecifierOrder">How members that explicitly implement an interface should be sorted compared to those that do not.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="explicitInterfaceSpecifierOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	public HasExplicitInterfaceSpecifierComparer(Order explicitInterfaceSpecifierOrder)
		: base(explicitInterfaceSpecifierOrder)
	{
	}

	/// <inheritdoc />
	protected override bool HasQuality(MemberDeclarationSyntax value)
	{
		if (value is MethodDeclarationSyntax mMethod)
		{
			return mMethod.ExplicitInterfaceSpecifier is not null;
		}
		else if (value is IndexerDeclarationSyntax mIndexer)
		{
			return mIndexer.ExplicitInterfaceSpecifier is not null;
		}
		else if (value is PropertyDeclarationSyntax mProperty)
		{
			return mProperty.ExplicitInterfaceSpecifier is not null;
		}
		else if (value is EventDeclarationSyntax mEvent)
		{
			return mEvent.ExplicitInterfaceSpecifier is not null;
		}
		else
		{
			return false;
		}
	}
}
