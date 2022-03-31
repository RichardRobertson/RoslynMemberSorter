namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates how to sort arity.
/// </summary>
public enum ArityOrder
{
	/// <summary>
	/// The arity of a member is not taking into account when sorting members.
	/// </summary>
	Default,

	/// <summary>
	/// Low arity members come before those with high arity.
	/// </summary>
	LowToHigh,

	/// <summary>
	/// High arity members come before those with low arity.
	/// </summary>
	HighToLow
}
