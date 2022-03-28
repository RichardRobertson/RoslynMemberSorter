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
	/// Asserts that a provided enum value is defined on the type.
	/// </summary>
	/// <typeparam name="TEnum"></typeparam>
	/// <param name="value"></param>
	/// <param name="argumentName"></param>
	/// <exception cref="ArgumentException"></exception>
	public static void AssertValid<TEnum>(this TEnum value, string argumentName) where TEnum : struct, Enum
	{
		if (!Enum.IsDefined(typeof(TEnum), value))
		{
			throw new ArgumentException($"{value} is not a defined enum name on {typeof(TEnum).FullName}", argumentName);
		}
	}

	/// <summary>
	/// Returns elements that are out of order when compared to the previous element using <paramref name="comparer" />.
	/// </summary>
	/// <typeparam name="TSource">The type of the elements in <paramref name="source" />.</typeparam>
	/// <param name="source">An <see cref="IEnumerable{T}" /> to check the order of.</param>
	/// <param name="comparer">An <see cref="IComparer{T}" /> to compare elements.</param>
	/// <returns>An <see cref="IEnumerable{T}" /> of <typeparamref name="TSource" /> containing elements that are out of order.</returns>
	/// <exception cref="ArgumentNullException"><paramref name="source" /> or <paramref name="comparer" /> is <see langword="null" />.</exception>
	public static IEnumerable<TSource> FindUnordered<TSource>(this IEnumerable<TSource> source, IComparer<TSource> comparer)
	{
		if (source is null)
		{
			throw new ArgumentNullException(nameof(source));
		}
		if (comparer is null)
		{
			throw new ArgumentNullException(nameof(comparer));
		}
		return FindUnorderedImpl(source, comparer);

		static IEnumerable<TSource> FindUnorderedImpl(IEnumerable<TSource> source, IComparer<TSource> comparer)
		{
			var enumerator = source.GetEnumerator();
			if (!enumerator.MoveNext())
			{
				yield break;
			}
			var lastElement = enumerator.Current;
			while (enumerator.MoveNext())
			{
				var currentElement = enumerator.Current;
				if (comparer.Compare(lastElement, currentElement) > 0)
				{
					yield return currentElement;
				}
				lastElement = currentElement;
			}
		}
	}

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
