using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynMemberSorter.Enums;

namespace RoslynMemberSorter;

/// <summary>
/// Specifies options for <see cref="DeclarationComparer" />.
/// </summary>
public sealed class DeclarationComparerOptions
{
	/// <summary>
	/// Tries to get a value by key from a collection.
	/// </summary>
	/// <param name="key">The key to try to retrieve.</param>
	/// <param name="value">The value if <paramref name="key" /> is found.</param>
	/// <returns><see langword="true" /> if <paramref name="key" /> is found; otherwise <see langword="false" />.</returns>
	private delegate bool TryGetDelegate(string key, out string? value);

	/// <summary>
	/// Indicates the order in which accessibility (eg: public, private) should be sorted.
	/// </summary>
	public ImmutableArray<Accessibility> AccessibilityOrder
	{
		get;
		set;
	} = ImmutableArray.Create
	(
		Accessibility.Public,
		Accessibility.ProtectedOrInternal,
		Accessibility.Internal,
		Accessibility.Protected,
		Accessibility.ProtectedAndInternal,
		Accessibility.Private
	);

	/// <summary>
	/// Indicates how names should be sorted.
	/// </summary>
	public NameOrder AlphabeticalIdentifiers
	{
		get;
		set;
	} = NameOrder.Alphabetical;

	/// <summary>Indicates where explicit interface implementation members should be ordered.</summary>
	/// <value>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Effect</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="Order.Default" /></term>
	/// 			<description>Explicit interface implementation is ignored for sorting purposes.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.First" /></term>
	/// 			<description>Members that explicitly implement an interface are ordered before members that do not.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.Last" /></term>
	/// 			<description>Members that explicitly implement an interface are ordered after members that do not.</description>
	/// 		</item>
	/// 	</list>
	/// </value>
	public Order ExplicitInterfaceSpecifiers
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Indicates how field mutability should be handled.
	/// </summary>
	public ImmutableArray<FieldMutability> FieldOrder
	{
		get;
		set;
	} = ImmutableArray.Create(FieldMutability.Const, FieldMutability.ReadOnly, FieldMutability.Mutable);

	/// <summary>
	/// Indicates the order in which member kinds (eg: field, constructor) should be sorted.
	/// </summary>
	public ImmutableArray<SyntaxKind> KindOrder
	{
		get;
		set;
	} = ImmutableArray.Create
	(
		SyntaxKind.FieldDeclaration,
		SyntaxKind.ConstructorDeclaration,
		SyntaxKind.DestructorDeclaration,
		SyntaxKind.IndexerDeclaration,
		SyntaxKind.PropertyDeclaration,
		SyntaxKind.EventFieldDeclaration,
		SyntaxKind.EventDeclaration,
		SyntaxKind.MethodDeclaration,
		SyntaxKind.OperatorDeclaration,
		SyntaxKind.ConversionOperatorDeclaration,
		SyntaxKind.EnumDeclaration,
		SyntaxKind.InterfaceDeclaration,
		SyntaxKind.StructDeclaration,
		SyntaxKind.ClassDeclaration,
		SyntaxKind.RecordDeclaration,
		SyntaxKind.RecordStructDeclaration,
		SyntaxKind.DelegateDeclaration
	);

	/// <summary>Indicates how method arity should be handled.</summary>
	/// <value>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Effect</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="Order.Default" /></term>
	/// 			<description>Method arity is ignored for sorting purposes.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.First" /></term>
	/// 			<description>Methods with low arity come before methods with high arity.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.Last" /></term>
	/// 			<description>Methods with low arity come after methods with high arity.</description>
	/// 		</item>
	/// 	</list>
	/// </value>
	/// <remarks>This can apply to constructors, delegates, indexers, and methods.</remarks>
	public Order LowArity
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Indicates the order in which operators should be sorted as they do not have conventional alphabetical names.
	/// </summary>
	public ImmutableArray<SyntaxKind> OperatorOrder
	{
		get;
		set;
	} = ImmutableArray.Create
	(
		SyntaxKind.PlusToken,
		SyntaxKind.MinusToken,
		SyntaxKind.ExclamationToken,
		SyntaxKind.TildeToken,
		SyntaxKind.PlusPlusToken,
		SyntaxKind.MinusMinusToken,
		SyntaxKind.AsteriskToken,
		SyntaxKind.SlashToken,
		SyntaxKind.PercentToken,
		SyntaxKind.AmpersandToken,
		SyntaxKind.BarToken,
		SyntaxKind.CaretToken,
		SyntaxKind.LessThanLessThanToken,
		SyntaxKind.GreaterThanGreaterThanToken,
		SyntaxKind.EqualsEqualsToken,
		SyntaxKind.ExclamationEqualsToken,
		SyntaxKind.LessThanToken,
		SyntaxKind.GreaterThanToken,
		SyntaxKind.LessThanEqualsToken,
		SyntaxKind.GreaterThanEqualsToken,
		SyntaxKind.TrueKeyword,
		SyntaxKind.FalseKeyword
	);

	/// <summary>
	/// Indicates how parameter lists should be compared.
	/// </summary>
	public ParameterSortStyle ParameterSortStyle
	{
		get;
		set;
	} = ParameterSortStyle.SortTypes;

	/// <summary>Indicates whether single line events should be separated from events with accessors.</summary>
	/// <value>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Effect</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="Order.Default" /></term>
	/// 			<description>Single line or accessor status is ignored for sorting purposes.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.First" /></term>
	/// 			<description>Single line events come before events with accessors.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.Last" /></term>
	/// 			<description>Single line events come after events with accessors.</description>
	/// 		</item>
	/// 	</list>
	/// </value>
	public Order SingleLineEvents
	{
		get;
		set;
	} = Order.Default;

	/// <summary>
	/// Indicates the order in which sorts (eg: by kind, by accessibility) should take place. <see cref="SortOrder" /> values not specified here will not be sorted at all.
	/// </summary>
	public ImmutableArray<SortOrder> SortOrders
	{
		get;
		set;
	} = ImmutableArray.Create
	(
		SortOrder.Kind,
		SortOrder.Static,
		SortOrder.FieldOrder,
		SortOrder.Accessibility,
		SortOrder.ExplicitInterfaceSpecifier,
		SortOrder.Identifier,
		SortOrder.Parameters
	);

	/// <summary>Indicates how static and instance members should be handled.</summary>
	/// <value>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Effect</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term><see cref="Order.Default" /></term>
	/// 			<description>Static or instance status is ignored for sorting purposes.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.First" /></term>
	/// 			<description>Static members come before instance members.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term><see cref="Order.Last" /></term>
	/// 			<description>Static members come after instance members.</description>
	/// 		</item>
	/// 	</list>
	/// </value>
	public Order Static
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Creates a new <see cref="DeclarationComparerOptions" /> instance from the given property name and value pairs.
	/// </summary>
	/// <param name="options">The config to read property names and values from.</param>
	/// <returns>A new <see cref="DeclarationComparerOptions" /> using the given property values.</returns>
	/// <remarks>Config options are prefixed with <c>"dotnet_diagnostic.rms0001."</c>. Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"dotnet_diagnostic.rms0001.sort_orders"</c>.</remarks>
	public static DeclarationComparerOptions FromAnalyzerConfigOptions(AnalyzerConfigOptions options)
	{
		const string prefix = "dotnet_diagnostic.rms0001.";
		return FromFunc(options.TryGetValue, prefix);
	}

	/// <summary>
	/// Creates a new <see cref="DeclarationComparerOptions" /> instance from the given property name and value pairs.
	/// </summary>
	/// <param name="dictionary">The dictionary to read property names and values from.</param>
	/// <returns>A new <see cref="DeclarationComparerOptions" /> using the given property values.</returns>
	/// <remarks>Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"sort_orders"</c>.</remarks>
	public static DeclarationComparerOptions FromDictionary(IReadOnlyDictionary<string, string?> dictionary)
	{
		return FromFunc(dictionary.TryGetValue, string.Empty);
	}

	/// <summary>
	/// Creates a dictionary that can be passed to <see cref="FromDictionary" /> to make a copy of these options.
	/// </summary>
	/// <returns>An <see cref="ImmutableDictionary{TKey,TValue}" /> that contains stringified property values from this instance.</returns>
	/// <remarks>Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"sort_orders"</c>,</remarks>
	public ImmutableDictionary<string, string?> ToImmutableDictionary()
	{
		var dictionary = new Dictionary<string, string?>();
		foreach (var property in typeof(DeclarationComparerOptions).GetProperties())
		{
			var propertyValue = property.GetValue(this);
			if (propertyValue is IEnumerable<Accessibility> enumAccessibility)
			{
				dictionary.Add(property.Name.ToSnakeCase(), string.Join(",", enumAccessibility.Select(e => e.ToString().ToSnakeCase())));
			}
			else if (propertyValue is IEnumerable<SyntaxKind> enumSyntaxKind)
			{
				dictionary.Add(property.Name.ToSnakeCase(), string.Join(",", enumSyntaxKind.Select(e => e.ToString().ToSnakeCase())));
			}
			else if (propertyValue is IEnumerable<SortOrder> enumSortOrders)
			{
				dictionary.Add(property.Name.ToSnakeCase(), string.Join(",", enumSortOrders.Select(e => e.ToString().ToSnakeCase())));
			}
			else if (propertyValue is IEnumerable<FieldMutability> enumFieldMutability)
			{
				dictionary.Add(property.Name.ToSnakeCase(), string.Join(",", enumFieldMutability.Select(e => e.ToString().ToSnakeCase())));
			}
			else if (propertyValue is Enum)
			{
				dictionary.Add(property.Name.ToSnakeCase(), propertyValue.ToString().ToSnakeCase());
			}
			else
			{
#pragma warning disable RCS1079, RCS1140
				throw new NotImplementedException($"A property of type {property.PropertyType} has been detected and stringifying is not yet implemented.");
#pragma warning restore RCS1079, RCS1140
			}
		}
		return dictionary.ToImmutableDictionary();
	}

	/// <summary>
	/// Creates a new <see cref="DeclarationComparerOptions" /> instance from the given property name and value pairs.
	/// </summary>
	/// <param name="tryGetFunc">A delegate pointing to a keyed collection to read properties from.</param>
	/// <param name="prefix">A <see cref="string" /> to prepend to all key names passed to <paramref name="tryGetFunc" />.</param>
	/// <returns>A new <see cref="DeclarationComparerOptions" /> using the given property values.</returns>
	/// <remarks>Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"sort_orders"</c>.</remarks>
	private static DeclarationComparerOptions FromFunc(TryGetDelegate tryGetFunc, string prefix)
	{
		var options = new DeclarationComparerOptions();
		foreach (var property in typeof(DeclarationComparerOptions).GetProperties())
		{
			if (tryGetFunc(prefix + property.Name.ToSnakeCase(), out var value) && value is not null)
			{
				if (property.PropertyType == typeof(ImmutableArray<Accessibility>))
				{
					property.SetValue(options, TryParseAll<Accessibility>(CleanAndSplitString(value)));
				}
				else if (property.PropertyType == typeof(ImmutableArray<SyntaxKind>))
				{
					property.SetValue(options, TryParseAll<SyntaxKind>(CleanAndSplitString(value)));
				}
				else if (property.PropertyType == typeof(ImmutableArray<SortOrder>))
				{
					property.SetValue(options, TryParseAll<SortOrder>(CleanAndSplitString(value)));
				}
				else if (property.PropertyType == typeof(Order))
				{
					if (Enum.TryParse<Order>(CleanString(value), true, out var sortOrder))
					{
						property.SetValue(options, sortOrder);
					}
				}
				else if (property.PropertyType == typeof(ParameterSortStyle))
				{
					if (Enum.TryParse<ParameterSortStyle>(CleanString(value), true, out var parameterSortStyle))
					{
						property.SetValue(options, parameterSortStyle);
					}
				}
				else if (property.PropertyType == typeof(NameOrder))
				{
					if (Enum.TryParse<NameOrder>(CleanString(value), true, out var nameOrder))
					{
						property.SetValue(options, nameOrder);
					}
				}
				else if (property.PropertyType == typeof(ImmutableArray<FieldMutability>))
				{
					property.SetValue(options, TryParseAll<FieldMutability>(CleanAndSplitString(value)));
				}
				else
				{
#pragma warning disable RCS1079, RCS1140
					throw new NotImplementedException($"A property of type {property.PropertyType} has been detected and parsing is not yet implemented.");
#pragma warning restore RCS1079, RCS1140
				}
			}
		}
		return options;

		static string CleanString(string value)
		{
			return value.Trim().Replace("_", string.Empty);
		}

		static IEnumerable<string> CleanAndSplitString(string value)
		{
			return value.Replace("_", string.Empty).Split(',').Select(v => v.Trim());
		}
	}

	/// <summary>
	/// Attempts to convert an enumerable of string values to an array of <see langword="enum" /> values.
	/// </summary>
	/// <typeparam name="TEnum">The type of the enumeration to call <see cref="Enum.TryParse{TEnum}(string, bool, out TEnum)" /> with.</typeparam>
	/// <param name="values">An <see cref="IEnumerable{T}" /> of <see cref="string" /> to try to convert.</param>
	/// <returns>A <see cref="ImmutableArray{T}" /> of <typeparamref name="TEnum" /> created from parsing <paramref name="values" />. The array will only contain successful values and invalid values will be dropped.</returns>
	/// <remarks>Each value will be run through <see cref="Enum.TryParse{TEnum}(string, bool, out TEnum)" /> ignoring case. If that fails, <c>"Declaration"</c>, <c>"Token"</c>, and <c>"Keyword"</c> will be appended in series to the end of the value to attempt to match a <see cref="SyntaxKind" />.</remarks>
	private static ImmutableArray<TEnum> TryParseAll<TEnum>(IEnumerable<string> values) where TEnum : struct, Enum
	{
		return values.Select(value =>
			(
				Enum.TryParse<TEnum>(value, true, out var parseResult)
					|| Enum.TryParse(value + "Declaration", true, out parseResult)
					|| Enum.TryParse(value + "Token", true, out parseResult)
					|| Enum.TryParse(value + "Keyword", true, out parseResult),
				parseResult
			))
			.Where(item => item.Item1)
			.Select(item => item.parseResult)
			.ToImmutableArray();
	}
}
