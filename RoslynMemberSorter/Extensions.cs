using System;
using System.Collections.Generic;
using System.Text;

namespace RoslynMemberSorter;

/// <summary>
/// Contains extension methods.
/// </summary>
public static class Extensions
{
	/// <summary>
	/// Checks to see if a collection is ordered.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements in <paramref name="source" />.</typeparam>
	/// <param name="source">An <see cref="IEnumerable{T}" /> to check the order of.</param>
	/// <param name="comparer">An <see cref="IComparer{T}" /> to compare elements.</param>
	/// <returns><see langword="true" /> if the collection is already ordered; otherwise <see langword="false" />.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="comparer" /> is <see langword="null" />.</exception>
	public static bool IsOrdered<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
	{
		if (source is null)
		{
			throw new ArgumentNullException(nameof(source));
		}
		if (comparer is null)
		{
			throw new ArgumentNullException(nameof(comparer));
		}
		var enumerator = source.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			return true;
		}
		var lastElement = enumerator.Current;
		while (enumerator.MoveNext())
		{
			var currentElement = enumerator.Current;
			if (comparer.Compare(lastElement, currentElement) > 0)
			{
				return false;
			}
			lastElement = currentElement;
		}
		return true;
	}

	/// <summary>
	/// Converts a Pascal case identifier to a lower snake case identifer.
	/// </summary>
	/// <param name="value">The identifier to convert</param>
	/// <returns>A <see cref="string" /> that converts capital letters to lower case and separates words with underscores <c>'_'</c>.</returns>
	/// <remarks><see cref="SortOrders" /> would be keyed as <c>"sort_orders"</c>.</remarks>
	public static string ToSnakeCase(this string value)
	{
		var builder = new StringBuilder(value.Length);
		for (int i = 0; i < value.Length; i++)
		{
			if (char.IsUpper(value[i]))
			{
				if (i != 0)
				{
					builder.Append('_');
				}
				builder.Append(char.ToLower(value[i]));
			}
			else
			{
				builder.Append(value[i]);
			}
		}
		return builder.ToString();
	}
}
