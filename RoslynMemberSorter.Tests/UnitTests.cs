using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;
using Xunit;

namespace RoslynMemberSorter.Tests;

public class UnitTests : XunitDiagnosticVerifier<MemberSorterAnalyzer, MemberSorterCodeFixProvider>
{
	private static CSharpTestOptions DefaultCSharpTestOptions
	{
		get
		{
			var allowedCompilerDiagnosticIds = ImmutableArray.Create(
				"CS0067", // Event is never used
				"CS0168", // Variable is declared but never used
				"CS0169", // Field is never used
				"CS0219", // Variable is assigned but its value is never used
				"CS0414", // Field is assigned but its value is never used
				"CS0649", // Field is never assigned to, and will always have its default value null
				"CS0660", // Type defines operator == or operator != but does not override Object.Equals(object o)
				"CS0661", // Type defines operator == or operator != but does not override Object.GetHashCode()
				"CS8019", // Unnecessary using directive
				"CS8321" // The local function is declared but never used
			);
			return CSharpTestOptions.Default
				.WithParseOptions(CSharpTestOptions.Default.ParseOptions.WithLanguageVersion(LanguageVersion.CSharp10))
				.WithAllowedCompilerDiagnosticIds(allowedCompilerDiagnosticIds)
				.WithConfigOptions(new Dictionary<string, string>()
					{
						[MakePropertyKey(dco => dco.AccessibilityOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.AlphabeticalIdentifiers)] = MakeEnumValue(NameOrder.Default),
						[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.Default),
						[MakePropertyKey(dco => dco.FieldOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.KindOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.LowArity)] = MakeEnumValue(Order.Default),
						[MakePropertyKey(dco => dco.OperatorOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.ParameterSortStyle)] = MakeEnumValue(ParameterSortStyle.Default),
						[MakePropertyKey(dco => dco.SingleLineEvents)] = MakeEnumValue(Order.Default),
						[MakePropertyKey(dco => dco.SortOrders)] = string.Empty,
						[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.Default)
					});
		}
	}

	protected override TestOptions CommonOptions => DefaultCSharpTestOptions;

	private static string MakeEnumList<TEnum>(params TEnum[] values) where TEnum : struct
	{
		return string.Join(',', values.Select(v => v.ToString()!.ToSnakeCase()));
	}

	private static string MakeEnumValue<TEnum>(TEnum value) where TEnum : struct
	{
		return value.ToString()!.ToSnakeCase();
	}

	private static string MakePropertyKey<T>(System.Linq.Expressions.Expression<System.Func<DeclarationComparerOptions, T>> expression)
	{
		if (expression.Body is System.Linq.Expressions.MemberExpression me)
		{
			return "dotnet_diagnostic.rms0001." + me.Member.Name.ToSnakeCase();
		}
		else
		{
			throw new System.ArgumentException("Expression is not a member access", nameof(expression));
		}
	}

	private async Task VerifyDiagnosticAndFixAsync(TestCode source, string expectedFix, Dictionary<string, string> config)
	{
		await VerifyDiagnosticAndFixAsync(new DiagnosticTestData(DiagnosticIds.SortMembers, source.Value, source.Spans), new ExpectedTestState(expectedFix), CommonOptions.WithConfigOptions(CommonOptions.ConfigOptions.SetItems(config))).ConfigureAwait(false);
	}

	private async Task VerifyNoDiagnosticAsync(TestCode source, Dictionary<string, string> config)
	{
		await VerifyNoDiagnosticAsync(new DiagnosticTestData(DiagnosticIds.SortMembers, source.Value, source.Spans), CommonOptions.WithConfigOptions(CommonOptions.ConfigOptions.SetItems(config))).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestAccessibilityOrderAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    private int B;\r\n    public int A;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Accessibility)
		};

		// default sort means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Public, Accessibility.Private);
		const string expectedFix = "class test\r\n{\r\n    public int A;\r\n    private int B;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Private, Accessibility.Public);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestAlphabeticalIdentifiersAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    int B;\r\n    int A;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AlphabeticalIdentifiers)] = MakeEnumValue(NameOrder.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		// default sort means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.AlphabeticalIdentifiers)] = MakeEnumValue(NameOrder.Alphabetical);
		const string expectedFix = "class test\r\n{\r\n    int A;\r\n    int B;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.AlphabeticalIdentifiers)] = MakeEnumValue(NameOrder.ReverseAlphabetical);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestExplicitInterfaceSpecifiersAsync()
	{
		var source = TestCode.Parse("class [|test|] : System.Collections.IEnumerable\r\n{\r\n    System.Collections.IEnumerator GetEnumerator() => throw new System.NotImplementedException();\r\n    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ExplicitInterfaceSpecifier)
		};

		// default sort means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.First);
		const string expectedFix = "class test : System.Collections.IEnumerable\r\n{\r\n    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();\r\n    System.Collections.IEnumerator GetEnumerator() => throw new System.NotImplementedException();\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.Last);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestFieldOrderAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    int MutableField;\r\n    const int ConstField = 0;\r\n    readonly int ReadOnlyField;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.FieldOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.FieldOrder)
		};

		// default sort means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.FieldOrder)] = MakeEnumList(FieldMutability.Const, FieldMutability.ReadOnly, FieldMutability.Mutable);
		const string expectedFix = "class test\r\n{\r\n    const int ConstField = 0;\r\n    readonly int ReadOnlyField;\r\n    int MutableField;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.FieldOrder)] = MakeEnumList(FieldMutability.Mutable, FieldMutability.Const, FieldMutability.ReadOnly);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestKindOrderAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    int Field;\r\n    int Property\r\n     {\r\n        get;\r\n    }\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration);
		const string expectedFix = "class test\r\n{\r\n    int Property\r\n    {\r\n        get;\r\n    }\r\n    int Field;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestLowArityAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    int Method(int a, int b) => throw new System.NotImplementedException();\r\n    int Method(int a) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.LowArity)] = MakeEnumValue(Order.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Parameters)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.LowArity)] = MakeEnumValue(Order.First);
		const string expectedFix = "class test\r\n{\r\n    int Method(int a) => throw new System.NotImplementedException();\r\n    int Method(int a, int b) => throw new System.NotImplementedException();\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.LowArity)] = MakeEnumValue(Order.Last);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestOperatorOrderAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    public static test operator -(test a, test b) => throw new System.NotImplementedException();\r\n    public static test operator +(test a, test b) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.OperatorOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.OperatorOrder)] = MakeEnumList(SyntaxKind.PlusToken, SyntaxKind.MinusToken);
		const string expectedFix = "class test\r\n{\r\n    public static test operator +(test a, test b) => throw new System.NotImplementedException();\r\n    public static test operator -(test a, test b) => throw new System.NotImplementedException();\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.OperatorOrder)] = MakeEnumList(SyntaxKind.MinusToken, SyntaxKind.PlusToken);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestParameterSortStyleAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ParameterSortStyle)] = MakeEnumValue(ParameterSortStyle.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Parameters)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.ParameterSortStyle)] = MakeEnumValue(ParameterSortStyle.SortTypes);
		const string expectedTypesFix = "class test\r\n{\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedTypesFix, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.ParameterSortStyle)] = MakeEnumValue(ParameterSortStyle.SortNames);
		const string expectedNamesFix = "class test\r\n{\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedNamesFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestSingleLineEventsAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    event System.EventHandler MultiLine\r\n    {\r\n        add => throw new System.NotImplementedException();\r\n        remove => throw new System.NotImplementedException();\r\n    }\r\n    event System.EventHandler SingleLine;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.SingleLineEvents)] = MakeEnumValue(Order.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.SingleLineEvents)] = MakeEnumValue(Order.First);
		const string expectedFix = "class test\r\n{\r\n    event System.EventHandler SingleLine;\r\n    event System.EventHandler MultiLine\r\n    {\r\n        add => throw new System.NotImplementedException();\r\n        remove => throw new System.NotImplementedException();\r\n    }\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.SingleLineEvents)] = MakeEnumValue(Order.Last);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestSortOrdersAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    public int PublicInstance;\r\n    private static int PrivateStatic;\r\n    private int PrivateInstance;\r\n    public static int PublicStatic;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Public, Accessibility.Private),
			[MakePropertyKey(dco => dco.SortOrders)] = string.Empty,
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.First)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumList(SortOrder.Accessibility, SortOrder.Static);
		const string expectedTypesFix = "class test\r\n{\r\n    public static int PublicStatic;\r\n    public int PublicInstance;\r\n    private static int PrivateStatic;\r\n    private int PrivateInstance;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedTypesFix, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumList(SortOrder.Static, SortOrder.Accessibility);
		const string expectedNamesFix = "class test\r\n{\r\n    public static int PublicStatic;\r\n    private static int PrivateStatic;\r\n    public int PublicInstance;\r\n    private int PrivateInstance;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedNamesFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task TestStaticAsync()
	{
		var source = TestCode.Parse("class [|test|]\r\n{\r\n    int Instance;\r\n    static int Static;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Static),
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.Default)
		};

		// default sort means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);

		// verify order applies
		config[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.First);
		const string expectedFix = "class test\r\n{\r\n    static int Static;\r\n    int Instance;\r\n}\r\n";
		await VerifyDiagnosticAndFixAsync(source, expectedFix, config).ConfigureAwait(false);

		// already in order, verify no diagnostic
		config[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.Last);
		await VerifyNoDiagnosticAsync(source, config).ConfigureAwait(false);
	}
}
