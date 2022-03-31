using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on their accessibility.
/// </summary>
public sealed class AccessibilityComparer : IndexedComparer<MemberDeclarationSyntax, Accessibility>
{
	/// <summary>
	/// Initializes a new <see cref="AccessibilityComparer" /> with the given accessibility order.
	/// </summary>
	/// <param name="accessibilityOrder">The order in which member accessibility should be sorted.</param>
	/// <param name="unknownOrder">Where member accessibilities not found in <paramref name="accessibilityOrder" /> should be sorted.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="unknownOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	public AccessibilityComparer(ImmutableArray<Accessibility> accessibilityOrder, Order unknownOrder)
		: base(accessibilityOrder, unknownOrder)
	{
	}

	/// <inheritdoc />
	protected override IEqualityComparer<Accessibility> QualityComparer => EqualityComparer<Accessibility>.Default;

	/// <inheritdoc />
	protected override Accessibility ProvideQuality(MemberDeclarationSyntax value)
	{
		if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.PublicKeyword)) || value.IsKind(SyntaxKind.NamespaceDeclaration))
		{
			return Accessibility.Public;
		}
		else if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.InternalKeyword)))
		{
			if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.ProtectedKeyword)))
			{
				return Accessibility.ProtectedOrInternal;
			}
			else
			{
				return Accessibility.Internal;
			}
		}
		else if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.ProtectedKeyword)))
		{
			if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.PrivateKeyword)))
			{
				return Accessibility.ProtectedAndInternal;
			}
			else
			{
				return Accessibility.Protected;
			}
		}
		else if (value.Modifiers.Any(token => token.IsKind(SyntaxKind.PrivateKeyword)))
		{
			return Accessibility.Private;
		}
		else
		{
			var xMajorParent = value.Ancestors().FirstOrDefault(node => node.IsKind(SyntaxKind.InterfaceDeclaration)
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
