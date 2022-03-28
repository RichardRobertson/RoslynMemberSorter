namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates how to sort names.
/// </summary>
public enum NameOrder
{
	/// <summary>
	/// The name of a member is not taken into account when sorting members.
	/// </summary>
	Default,

	/// <summary>
	/// Names are sorted in alphabetical order (eg: A comes before B).
	/// </summary>
	Alphabetical,

	/// <summary>
	/// Names are sorted in reverse alphabetical order (eg: B comes before A).
	/// </summary>
	ReverseAlphabetical
}
