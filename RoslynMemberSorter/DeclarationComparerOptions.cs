using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslynMemberSorter.Comparers;
using RoslynMemberSorter.Comparers.CSharp;
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
	public IdentifierOrder IdentifierNames
	{
		get;
		set;
	} = IdentifierOrder.Alphabetical;

	/// <summary>
	/// Indicates how parameter arity should be sorted.
	/// </summary>
	public ArityOrder ArityOrder
	{
		get;
		set;
	} = ArityOrder.LowToHigh;

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

	/// <summary>
	/// Indicates whether <see cref="SyntaxKind.EventDeclaration" /> and <see cref="SyntaxKind.EventFieldDeclaration" /> should be treated as equivalent when sorting by kind.
	/// </summary>
	public bool MergeEvents
	{
		get;
		set;
	} = true;

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
	/// Indicates how parameter type names should be sorted.
	/// </summary>
	public IdentifierOrder ParameterTypeNames
	{
		get;
		set;
	} = IdentifierOrder.Alphabetical;

	/// <summary>
	/// Indicates how parameter names should be sorted.
	/// </summary>
	public IdentifierOrder ParameterNames
	{
		get;
		set;
	} = IdentifierOrder.Alphabetical;

	/// <summary>
	/// Indicates the order in which parameters that have a <see langword="ref" /> modifier keyword should be sorted compared to those that do not.
	/// </summary>
	public Order ReferenceParameterOrder
	{
		get;
		set;
	} = Order.First;

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
		SortOrder.ParameterArity,
		SortOrder.ParameterTypes
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
	/// Indicates where to sort accessibilities that are not found in <see cref="AccessibilityOrder" />.
	/// </summary>
	public Order UnknownAccessibilityOrder
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Indicates where to sort field mutabilities that are not found in <see cref="FieldOrder" />.
	/// </summary>
	public Order UnknownFieldMutabilityOrder
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Indicates where to sort kinds that are not found in <see cref="KindOrder" />.
	/// </summary>
	public Order UnknownKindOrder
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Indicates where to sort operator tokens that are not found in <see cref="OperatorOrder" />.
	/// </summary>
	public Order UnknownOperatorTokenOrder
	{
		get;
		set;
	} = Order.First;

	/// <summary>
	/// Creates a new <see cref="DeclarationComparerOptions" /> instance from the given property name and value pairs.
	/// </summary>
	/// <param name="options">The config to read property names and values from.</param>
	/// <returns>A new <see cref="DeclarationComparerOptions" /> using the given property values.</returns>
	/// <remarks>Config options are prefixed with <c>"dotnet_diagnostic.rms####."</c>. Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"dotnet_diagnostic.rms0001.sort_orders"</c>.</remarks>
	public static DeclarationComparerOptions FromAnalyzerConfigOptions(AnalyzerConfigOptions options)
	{
		var prefixLookup = new Dictionary<string, string>();
		foreach (var property in typeof(DeclarationComparerOptions).GetProperties())
		{
			prefixLookup[property.Name] = property.Name switch
			{
				nameof(AccessibilityOrder) => "dotnet_diagnostic.rms0005.",
				nameof(IdentifierNames) => "dotnet_diagnostic.rms0007.",
				nameof(ArityOrder) => "dotnet_diagnostic.rms0008.",
				nameof(ExplicitInterfaceSpecifiers) => "dotnet_diagnostic.rms0006.",
				nameof(FieldOrder) => "dotnet_diagnostic.rms0004.",
				nameof(KindOrder) => "dotnet_diagnostic.rms0002.",
				nameof(MergeEvents) => "dotnet_diagnostic.rms0002.",
				nameof(OperatorOrder) => "dotnet_diagnostic.rms0007.",
				nameof(ParameterNames) => "dotnet_diagnostic.rms0010.",
				nameof(ParameterTypeNames) => "dotnet_diagnostic.rms0009.",
				nameof(ReferenceParameterOrder) => "dotnet_diagnostic.rms0009.",
				nameof(SortOrders) => "dotnet_diagnostic.rms_shared.",
				nameof(Static) => "dotnet_diagnostic.rms0003.",
				nameof(UnknownAccessibilityOrder) => "dotnet_diagnostic.rms0005.",
				nameof(UnknownFieldMutabilityOrder) => "dotnet_diagnostic.rms0004.",
				nameof(UnknownKindOrder) => "dotnet_diagnostic.rms0002.",
				nameof(UnknownOperatorTokenOrder) => "dotnet_diagnostic.rms0007.",
#pragma warning disable RCS1079, RCS1140
				_ => throw new NotImplementedException($"Property {property.Name} prefix not assigned.")
#pragma warning restore RCS1079, RCS1140
			};
		}
		return FromFunc(options.TryGetValue, prefixLookup);
	}

	/// <summary>
	/// Creates a new <see cref="DeclarationComparerOptions" /> instance from the given property name and value pairs.
	/// </summary>
	/// <param name="dictionary">The dictionary to read property names and values from.</param>
	/// <returns>A new <see cref="DeclarationComparerOptions" /> using the given property values.</returns>
	/// <remarks>Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"sort_orders"</c>.</remarks>
	public static DeclarationComparerOptions FromDictionary(IReadOnlyDictionary<string, string?> dictionary)
	{
		return FromFunc(dictionary.TryGetValue);
	}

	/// <summary>
	/// Creates a comparer that matches the current settings.
	/// </summary>
	/// <returns>A <see cref="MultiComparer{T}" /> of <see cref="MemberDeclarationSyntax" /></returns>
	public MultiComparer<MemberDeclarationSyntax> ToCSharpComparer()
	{
		var comparers = new List<IComparer<MemberDeclarationSyntax>>();
		foreach (var sort in SortOrders)
		{
			switch (sort)
			{
				case SortOrder.Accessibility:
					comparers.Add(new AccessibilityComparer(AccessibilityOrder, UnknownAccessibilityOrder));
					break;
				case SortOrder.ExplicitInterfaceSpecifier:
					comparers.Add(new HasExplicitInterfaceSpecifierComparer(ExplicitInterfaceSpecifiers));
					break;
				case SortOrder.FieldOrder:
					comparers.Add(new FieldDeclarationMutabilityComparer(FieldOrder, UnknownFieldMutabilityOrder));
					break;
				case SortOrder.Identifier:
					comparers.Add(new IdentifierComparer(IdentifierNames, OperatorOrder, UnknownOperatorTokenOrder));
					break;
				case SortOrder.Kind:
					comparers.Add(new KindComparer(KindOrder, UnknownKindOrder, MergeEvents));
					break;
				case SortOrder.ParameterArity:
					comparers.Add(new ParameterArityComparer(ArityOrder));
					break;
				case SortOrder.ParameterTypes:
					comparers.Add(new ParameterTypeComparer(ParameterTypeNames, ReferenceParameterOrder));
					break;
				case SortOrder.ParameterNames:
					comparers.Add(new ParameterNameComparer(ParameterNames));
					break;
				case SortOrder.Static:
					comparers.Add(new IsStaticComparer(Static));
					break;
			}
		}
		return new MultiComparer<MemberDeclarationSyntax>(comparers);
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
			else if (propertyValue is bool)
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
	/// <param name="prefixLookup">An optional dictionary of property name keys to prefix strings to pass to <paramref name="tryGetFunc" />.</param>
	/// <returns>A new <see cref="DeclarationComparerOptions" /> using the given property values.</returns>
	/// <remarks>Property names are stored in lower snake case. So <see cref="SortOrders" /> would be keyed as <c>"sort_orders"</c>.</remarks>
	private static DeclarationComparerOptions FromFunc(TryGetDelegate tryGetFunc, IDictionary<string, string>? prefixLookup = null)
	{
		var options = new DeclarationComparerOptions();
		foreach (var property in typeof(DeclarationComparerOptions).GetProperties())
		{
			if (prefixLookup?.TryGetValue(property.Name, out var prefixString) != true)
			{
				prefixString = null;
			}
			if (tryGetFunc(prefixString + property.Name.ToSnakeCase(), out var value) && value is not null)
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
				else if (property.PropertyType == typeof(IdentifierOrder))
				{
					if (Enum.TryParse<IdentifierOrder>(CleanString(value), true, out var nameOrder))
					{
						property.SetValue(options, nameOrder);
					}
				}
				else if (property.PropertyType == typeof(ImmutableArray<FieldMutability>))
				{
					property.SetValue(options, TryParseAll<FieldMutability>(CleanAndSplitString(value)));
				}
				else if (property.PropertyType == typeof(bool))
				{
					if (bool.TryParse(CleanString(value), out var boolValue))
					{
						property.SetValue(options, boolValue);
					}
				}
				else if (property.PropertyType == typeof(ArityOrder))
				{
					if (Enum.TryParse<ArityOrder>(CleanString(value), true, out var arityOrder))
					{
						property.SetValue(options, arityOrder);
					}
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
