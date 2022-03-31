using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers;

/// <summary>
/// Provides a base class for comparers that compare items based on a quality found in a list.
/// </summary>
/// <typeparam name="TCompared">The type of the objects being compared.</typeparam>
/// <typeparam name="TQuality">The type of the list of qualities to take an index from.</typeparam>
public abstract class IndexedComparer<TCompared, TQuality> : IComparer<TCompared>
{
	/// <summary>
	/// Initializes a new <see cref="IndexedComparer{TCompared, TList}" /> with the given unknown order value.
	/// </summary>
	/// <param name="qualityList">The order that object qualities should be sorted.</param>
	/// <param name="unknownOrder">Where to sort qualities that are not found.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="unknownOrder" /> is not a defined enum name on <see cref="Order" />.</exception>
	protected IndexedComparer(ImmutableArray<TQuality> qualityList, Order unknownOrder)
	{
		unknownOrder.AssertValid();
		QualityList = qualityList;
		UnknownOrder = unknownOrder;
	}

	/// <summary>
	/// Gets the order that object qualities should be sorted.
	/// </summary>
	/// <value>An <see cref="ImmutableArray{T}" /> of <typeparamref name="TQuality" /> that indicates the order a given quality should sort to.</value>
	public ImmutableArray<TQuality> QualityList
	{
		get;
	}

	/// <summary>Gets a value indicating where unknown qualities should be sorted.</summary>
	/// <value>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Effect</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="Order.Default" /></term>
	/// 			<description>Unknown qualities are ignored for sorting purposes.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.First" /></term>
	/// 			<description>Unknown qualities come before those that are known.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.Last" /></term>
	/// 			<description>Unknown qualities come after those are that known.</description>
	/// 		</item>
	/// 	</list>
	/// </value>
	public Order UnknownOrder
	{
		get;
	}

	/// <summary>
	/// Gets an equality comparer for the object qualities to find in <see cref="QualityList" />.
	/// </summary>
	/// <value>An instance of an <see cref="IEqualityComparer{T}" /> of <typeparamref name="TQuality" /> that can be used to find an index in <see cref="QualityList" />.</value>
	protected abstract IEqualityComparer<TQuality> QualityComparer
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
	public int Compare(TCompared? x, TCompared? y)
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
		var xIndex = QualityList.IndexOf(ProvideQuality(x), QualityComparer);
		var xIsKnown = xIndex != -1;
		var yIndex = QualityList.IndexOf(ProvideQuality(y), QualityComparer);
		var yIsKnown = yIndex != -1;
		if (xIsKnown == yIsKnown)
		{
			return xIndex - yIndex;
		}
		else if (UnknownOrder == Order.Default)
		{
			return 0;
		}
		else if (xIsKnown)
		{
			if (UnknownOrder == Order.First)
			{
				return 1;
			}
			else
			{
				return -1;
			}
		}
		else if (UnknownOrder == Order.First)
		{
			return -1;
		}
		else
		{
			return 1;
		}
	}

	/// <summary>
	/// When overridden in a derived class, provdes a value that can be found in <see cref="QualityList" />.
	/// </summary>
	/// <param name="value">The object to get a quality for.</param>
	/// <returns>A <typeparamref name="TQuality" /> to search for within <see cref="QualityList" />.</returns>
	protected abstract TQuality ProvideQuality(TCompared value);
}
