namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates how to sort a given member quality.
/// </summary>
public enum Order
{
	/// <summary>
	/// This quality is not taken into account when sorting members.
	/// </summary>
	Default,

	/// <summary>
	/// Members with this quality should come first.
	/// </summary>
	First,

	/// <summary>
	/// Members with this quality should come last.
	/// </summary>
	Last
}
