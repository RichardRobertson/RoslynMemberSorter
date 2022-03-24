namespace RoslynMemberSorter;

/// <summary>
/// Indicates the accessibility level of a member.
/// </summary>
public enum Accessibility
{
	/// <summary>
	/// Internal members are accessible to callers in the declaring assembly.
	/// </summary>
	Internal,

	/// <summary>
	/// Private members are accessible to the declaring type.
	/// </summary>
	Private,

	/// <summary>
	/// Private protected members are accessible to types derived from the declaring type but only in the declaring assembly.
	/// </summary>
	PrivateProtected,

	/// <summary>
	/// Protected members are accessible to types derived from the declaring type.
	/// </summary>
	Protected,

	/// <summary>
	/// Protected internal members are accessible to types derived from the declaring type and to the declaring assembly.
	/// </summary>
	ProtectedInternal,

	/// <summary>
	/// Public members are accessible to all callers.
	/// </summary>
	Public
}
