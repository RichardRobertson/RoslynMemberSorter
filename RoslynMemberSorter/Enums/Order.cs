namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates how to sort a given member detail.
/// </summary>
/// <remarks>See properties on <see cref="DeclarationComparerOptions" /> for more details.</remarks>
public enum Order
{
	/// <summary>
	/// This detail is not taken into account when sorting members.
	/// </summary>
	Default,

	/// <summary>
	/// This member detail should come first.
	/// </summary>
	First,

	/// <summary>
	/// This member detail should come last.
	/// </summary>
	Last
}
