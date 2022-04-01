namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates a member detail that can be sorted. These values are used for <see cref="DeclarationComparerOptions.SortOrders" /> to indicate the order in which sorts should take place.
/// </summary>
public enum SortOrder
{
	/// <summary>
	/// Sort members based on their accessibility. This uses the order defined by <see cref="DeclarationComparerOptions.AccessibilityOrder" />.
	/// </summary>
	Accessibility,

	/// <summary>
	/// Sort members based on whether they explicitly implement interfaces. This uses the order defined by <see cref="DeclarationComparerOptions.ExplicitInterfaceSpecifiers" />.
	/// </summary>
	ExplicitInterfaceSpecifier,

	/// <summary>
	/// Sort fields based on their mutability. This applies to constant and readonly fields and uses the order defined by <see cref="DeclarationComparerOptions.FieldOrder" />.
	/// </summary>
	FieldOrder,

	/// <summary>
	/// Sort members based on their name or operator token. This uses the order defined by <see cref="DeclarationComparerOptions.AlphabeticalIdentifiers" /> for named members or <see cref="DeclarationComparerOptions.OperatorOrder" /> for operators.
	/// </summary>
	Identifier,

	/// <summary>
	/// Sort members based on their kind. This uses the order defined by <see cref="DeclarationComparerOptions.KindOrder" />.
	/// </summary>
	Kind,

	/// <summary>
	/// Sort method-like members based on their parameter arity. This applies to constructors, delegates, indexers, and methods an uses the orders defined by <see cref="DeclarationComparerOptions.ArityOrder" />.
	/// </summary>
	ParameterArity,

	/// <summary>
	/// Sort method-like members based on their parameter types. This applies to constructors, delegates, indexers, and methods an uses the orders defined by <see cref="DeclarationComparerOptions.AlphabeticalIdentifiers" /> and <see cref="DeclarationComparerOptions.ReferenceParameterOrder" />.
	/// </summary>
	ParameterTypes,

	/// <summary>
	/// Sort method-like members based on their parameter names. This applies to constructors, delegates, indexers, and methods an uses the orders defined by <see cref="DeclarationComparerOptions.AlphabeticalIdentifiers" />.
	/// </summary>
	ParameterNames,

	/// <summary>
	/// Sort members based on their static or instance status. This uses the order defined by <see cref="DeclarationComparerOptions.Static" />.
	/// </summary>
	Static
}
