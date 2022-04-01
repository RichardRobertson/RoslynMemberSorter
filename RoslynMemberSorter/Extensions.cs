using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
	/// <typeparam name="TEnum">The type of the enumeration to call <see cref="Enum.IsDefined(Type, object)" /> with.</typeparam>
	/// <param name="value">The enum value to check.</param>
	/// <param name="argumentName">The name of the enum variable to report in <see cref="ArgumentException" /> if it is not a defined value.</param>
	/// <exception cref="InvalidEnumArgumentException"><paramref name="value" /> is not a defined enum name on <typeparamref name="TEnum" />.</exception>
	public static void AssertValid<TEnum>(this TEnum value, [CallerArgumentExpression("value")] string argumentName = "") where TEnum : struct, Enum
	{
		if (!Enum.IsDefined(typeof(TEnum), value))
		{
			throw new InvalidEnumArgumentException(argumentName, Unsafe.As<TEnum, int>(ref value), typeof(TEnum));
		}
	}

	/// <summary>
	/// Gets the element immediately before <paramref name="element" />.
	/// </summary>
	/// <typeparam name="T">The type of the collection.</typeparam>
	/// <param name="collection">The collection to search.</param>
	/// <param name="element">The element to match.</param>
	/// <returns>The element immediately before <paramref name="element" />.</returns>
	/// <exception cref="InvalidOperationException"><paramref name="collection" /> is empty or does not contain <paramref name="element" />.</exception>
	/// <exception cref="ArgumentNullException"><paramref name="collection" /> is <see langword="null" />.</exception>
	public static T Before<T>(this IEnumerable<T> collection, T element)
	{
		if (collection is null)
		{
			throw new ArgumentNullException(nameof(collection));
		}
		var enumerator = collection.GetEnumerator();
		if (!enumerator.MoveNext())
		{
			throw new InvalidOperationException("Collection is empty.");
		}
		var last = enumerator.Current;
		while (enumerator.MoveNext())
		{
			if (ReferenceEquals(element, enumerator.Current))
			{
				return last;
			}
		}
		throw new InvalidOperationException("Collection does not contain element.");
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
