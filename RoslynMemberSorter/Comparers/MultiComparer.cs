using System.Collections.Generic;
using System.Collections.Immutable;

namespace RoslynMemberSorter.Comparers;

/// <summary>
/// A comparer that orders items based on multiple criteria. Each element of <see cref="Comparers" /> is called in sequence if values are considered equivalent. This is similar to calling <see cref="System.Linq.Enumerable.OrderBy{TSource, TKey}(IEnumerable{TSource}, System.Func{TSource, TKey})" /> followed by <see cref="System.Linq.Enumerable.ThenBy{TSource, TKey}(System.Linq.IOrderedEnumerable{TSource}, System.Func{TSource, TKey})" />
/// </summary>
/// <typeparam name="T">The type of objects to compare.</typeparam>
public sealed class MultiComparer<T> : IComparer<T>
{
	/// <inheritdoc cref="MultiComparer{T}.MultiComparer(IEnumerable{IComparer{T}})" />
	public MultiComparer(params IComparer<T>[] comparers)
		: this((IEnumerable<IComparer<T>>)comparers)
	{
	}

	/// <summary>
	/// Initializes a new <see cref="MultiComparer{T}" /> with the given comparers.
	/// </summary>
	/// <param name="comparers">An ordered set of comparers used to order values.</param>
	public MultiComparer(IEnumerable<IComparer<T>> comparers)
	{
		Comparers = comparers.ToImmutableArray();
	}

	/// <summary>
	/// Gets or sets an ordered set of comparers used to order values.
	/// </summary>
	/// <value>An <see cref="ImmutableArray{T}" /> of <see cref="IComparer{T}" /> used in <see cref="Compare" />.</value>
	public ImmutableArray<IComparer<T>> Comparers
	{
		get;
		set;
	}

	/// <inheritdoc cref="Compare(T?, T?, out IComparer{T}?)" />
	public int Compare(T? x, T? y)
	{
		return Compare(x, y, out var _);
	}

	/// <summary>
	/// Compares two objects and returns a value indicating whether one is less than, equal to, or greater than the other.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <param name="comparerMatched">Returns the <see cref="IComparer{T}" /> that produced the result or <see langword="null" /> if <paramref name="x" /> and <paramref name="y" /> are considered equal.</param>
	/// <returns>
	/// 	<para>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Meaning</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>Less than zero</term>
	/// 			<description><paramref name="x" /> is less than <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Zero</term>
	/// 			<description><paramref name="x" /> equals <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Greater than zero</term>
	/// 			<description><paramref name="x" /> is greater than <paramref name="y" />.</description>
	/// 		</item>
	/// 	</list>
	/// </returns>
	public int Compare(T? x, T? y, out IComparer<T>? comparerMatched)
	{
		comparerMatched = null;
		if (ReferenceEquals(x, y))
		{
			return 0;
		}
		else if (x is null)
		{
			return -1;
		}
		else if (y is null)
		{
			return 1;
		}
		foreach (var comparer in Comparers)
		{
			var comparisonValue = comparer.Compare(x, y);
			if (comparisonValue != 0)
			{
				comparerMatched = comparer;
				return comparisonValue;
			}
		}
		return 0;
	}
}
