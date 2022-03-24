namespace RoslynMemberSorter;

/// <summary>
/// Indicates field mutability.
/// </summary>
public enum FieldMutability
{
	/// <summary>
	/// The field is declared with the const keyword.
	/// </summary>
	Const,

	/// <summary>
	/// The field is declared with neither the const keyword nor the readonly keyword.
	/// </summary>
	Mutable,

	/// <summary>
	/// The field is declared with the readonly keyword.
	/// </summary>
	ReadOnly
}
