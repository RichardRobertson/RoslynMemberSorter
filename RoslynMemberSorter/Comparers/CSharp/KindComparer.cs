using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Compares <see cref="MemberDeclarationSyntax" /> objects based on the <see cref="CSharpSyntaxNode.Kind()" />
/// </summary>
public sealed class KindComparer : IndexedComparer<MemberDeclarationSyntax, SyntaxKind>
{
	/// <summary>
	/// Initializes a new <see cref="KindComparer" /> with the given kind order.
	/// </summary>
	/// <param name="kindOrder">The order in which member kind should be sorted.</param>
	/// <param name="unknownOrder">Where member kinds not found in <paramref name="kindOrder" /> should be sorted.</param>
	/// <param name="mergeEvents"><see langword="true" /> means to treat <see cref="SyntaxKind.EventFieldDeclaration" /> as if they are <see cref="SyntaxKind.EventDeclaration" /> ; <see langword="false" /> to treat them separately in sort order.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="unknownOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	public KindComparer(ImmutableArray<SyntaxKind> kindOrder, Order unknownOrder, bool mergeEvents)
		: base(kindOrder, unknownOrder)
	{
		MergeEvents = mergeEvents;
	}

	/// <summary>
	/// Gets a value indicating whether single line events should be separated from events with accessors.
	/// </summary>
	/// <value><see langword="true" /> means to treat <see cref="SyntaxKind.EventFieldDeclaration" /> as if they are <see cref="SyntaxKind.EventDeclaration" /> ; <see langword="false" /> to treat them separately in sort order.</value>
	public bool MergeEvents
	{
		get;
	}

	/// <inheritdoc />
	protected override IEqualityComparer<SyntaxKind> QualityComparer => SyntaxFacts.EqualityComparer;

	/// <inheritdoc />
	protected override SyntaxKind ProvideQuality(MemberDeclarationSyntax value)
	{
		if (MergeEvents && value.IsKind(SyntaxKind.EventFieldDeclaration))
		{
			return SyntaxKind.EventDeclaration;
		}
		else
		{
			return value.Kind();
		}
	}
}
