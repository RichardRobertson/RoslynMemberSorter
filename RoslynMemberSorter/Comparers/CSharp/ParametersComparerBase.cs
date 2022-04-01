using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RoslynMemberSorter.Comparers.CSharp;

/// <summary>
/// Base class for a comparer that compares <see cref="MemberDeclarationSyntax" /> objects based on their declared parameters.
/// </summary>
public abstract class ParametersComparerBase : IComparer<MemberDeclarationSyntax>
{
	/// <summary>
	/// Compares two objects and returns a value indicating whether one should come before the other.
	/// </summary>
	/// <param name="x">The first object to compare.</param>
	/// <param name="y">The second object to compare.</param>
	/// <returns>
	/// 	<para>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Meaning</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>Less than zero</term>
	/// 			<description><paramref name="x" /> should come before <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Zero</term>
	/// 			<description><paramref name="x" /> is sort equivalent to <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Greater than zero</term>
	/// 			<description><paramref name="x" /> should come after <paramref name="y" />.</description>
	/// 		</item>
	/// 	</list>
	/// </returns>
	public int Compare(MemberDeclarationSyntax? x, MemberDeclarationSyntax? y)
	{
		if (ReferenceEquals(x, y))
		{
			return 0;
		}
		else if (x is null)
		{
			return 1;
		}
		else if (y is null)
		{
			return -1;
		}
		var xParameters = GetParameterSyntaxes(x);
		var yParameters = GetParameterSyntaxes(y);
		return CompareParameterSyntaxes(xParameters, yParameters);
	}

	/// <summary>
	/// When overriden in a derived class, compares two members parameters.
	/// </summary>
	/// <param name="xParameters">The parameters of the first member.</param>
	/// <param name="yParameters">The parameters of the second member.</param>
	/// <returns>
	/// 	<para>A signed integer that indicates the relative values of <paramref name="x" /> and <paramref name="y" />, as shown in the following table.</para>
	/// 	<list type="table">
	/// 		<listheader>
	/// 			<term>Value</term>
	/// 			<description>Meaning</description>
	/// 		</listheader>
	/// 		<item>
	/// 			<term>Less than zero</term>
	/// 			<description><paramref name="x" /> should come before <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Zero</term>
	/// 			<description><paramref name="x" /> is sort equivalent to <paramref name="y" />.</description>
	/// 		</item>
	/// 		<item>
	/// 			<term>Greater than zero</term>
	/// 			<description><paramref name="x" /> should come after <paramref name="y" />.</description>
	/// 		</item>
	/// 	</list>
	/// </returns>
	protected abstract int CompareParameterSyntaxes(SeparatedSyntaxList<ParameterSyntax> xParameters, SeparatedSyntaxList<ParameterSyntax> yParameters);

	/// <summary>
	/// Gets the <see cref="SeparatedSyntaxList{TNode}" /> of <see cref="ParameterSyntax" /> from a given member.
	/// </summary>
	/// <param name="member">The member to get a parameter list from.</param>
	/// <returns>A <see cref="SeparatedSyntaxList{TNode}" /> of <see cref="ParameterSyntax" /> from <paramref name="member" />'s ParameterList property if it is a <see cref="SyntaxKind.MethodDeclaration" />, <see cref="SyntaxKind.IndexerDeclaration" />, <see cref="SyntaxKind.ConstructorDeclaration" />, or <see cref="SyntaxKind.DelegateDeclaration" />; otherwise returns <see langword="default" />.</returns>
	private static SeparatedSyntaxList<ParameterSyntax> GetParameterSyntaxes(MemberDeclarationSyntax member)
	{
		if (member is MethodDeclarationSyntax mMethod)
		{
			return mMethod.ParameterList.Parameters;
		}
		else if (member is IndexerDeclarationSyntax mIndexer)
		{
			return mIndexer.ParameterList.Parameters;
		}
		else if (member is ConstructorDeclarationSyntax mConstructor)
		{
			return mConstructor.ParameterList.Parameters;
		}
		else if (member is DelegateDeclarationSyntax mDelegate)
		{
			return mDelegate.ParameterList.Parameters;
		}
		else
		{
			return default;
		}
	}
}
