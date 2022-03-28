namespace RoslynMemberSorter.Enums;

/// <summary>
/// Indicates how parameter lists should be compared.
/// </summary>
public enum ParameterSortStyle
{
	/// <summary>
	/// Indicates parameter types and names are ignored for sorting purposes.
	/// </summary>
	Default,

	/// <summary>
	/// Sort methods based on parameter types.
	/// </summary>
	/// <remarks><c>SomeMethod(int number)</c> comes before <c>SomeMethod(string address)</c>.</remarks>
	SortTypes,

	/// <summary>
	/// Sort methods based on parameter names.
	/// </summary>
	/// <remarks><c>SomeMethod(string address)</c> comes before <c>SomeMethod(int number)</c>.</remarks>
	SortNames
}
