namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates field mutability.
/// </summary>
public enum FieldMutability
{
	/// <summary>
	/// The field is declared with the <see langword="const" /> keyword.
	/// </summary>
	Const,

	/// <summary>
	/// The field is declared with neither the <see langword="const" /> keyword nor the <see langword="readonly" /> keyword.
	/// </summary>
	Mutable,

	/// <summary>
	/// The field is declared with the <see langword="readonly" /> keyword.
	/// </summary>
	ReadOnly
}
