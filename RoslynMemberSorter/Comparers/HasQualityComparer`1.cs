using System.Collections.Generic;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter.Comparers;

/// <summary>
/// Provides a base class for comparers that compare items based on whether they possess a specific quality.
/// </summary>
/// <typeparam name="TCompared">The type of the objects being compared.</typeparam>
public abstract class HasQualityComparer<TCompared> : IComparer<TCompared>
{
	/// <summary>
	/// Initializes a new <see cref="HasQualityComparer{TCompared}" /> with the given quality order.
	/// </summary>
	/// <param name="qualityOrder">Where items possessing this quality should be sorted.</param>
	protected HasQualityComparer(Order qualityOrder)
	{
		qualityOrder.AssertValid();
		QualityOrder = qualityOrder;
	}

	/// <summary>Indicates where items that possess this quality should be ordered.</summary>
	/// <value>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Effect</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="Order.Default" /></term>
	/// 			<description>This quality is ignored for sorting purposes.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.First" /></term>
	/// 			<description>Items that possess this quality are ordered before items that do not.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.Last" /></term>
	/// 			<description>Items that possess this quality are ordered after items that do not.</description>
	/// 		</item>
	/// 	</list>
	/// </value>
	public Order QualityOrder
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
		if (QualityOrder == Order.Default || ReferenceEquals(x, y))
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
		var xHasQuality = HasQuality(x);
		var yHasQuality = HasQuality(y);
		if (xHasQuality && !yHasQuality)
		{
			return QualityOrder == Order.First ? -1 : 1;
		}
		else if (!xHasQuality && yHasQuality)
		{
			return QualityOrder == Order.First ? 1 : -1;
		}
		else
		{
			return 0;
		}
	}

	/// <summary>
	/// When overridden in a derived class, provides a value that indicates whether <paramref name="value" /> possesses a specific quality.
	/// </summary>
	/// <param name="value">The value to check.</param>
	/// <returns><see langword="true" /> if <paramref name="value" /> possesses a specific quality; otherwise <see langword="false" />.</returns>
	protected abstract bool HasQuality(TCompared value);
}
