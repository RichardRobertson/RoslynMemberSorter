using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Roslynator.Testing;
using Roslynator.Testing.CSharp;
using Roslynator.Testing.CSharp.Xunit;
using RoslynMemberSorter.Enums;
using Xunit;

namespace RoslynMemberSorter.Tests;

public class UnitTests : XunitDiagnosticVerifier<MemberSorterAnalyzer, FixOneCodeFixProvider>
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
						[MakePropertyKey(dco => dco.IdentifierNames)] = MakeEnumValue(IdentifierOrder.Default),
						[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.Default),
						[MakePropertyKey(dco => dco.FieldOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.KindOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.ArityOrder)] = MakeEnumValue(ArityOrder.Default),
						[MakePropertyKey(dco => dco.OperatorOrder)] = string.Empty,
						[MakePropertyKey(dco => dco.MergeEvents)] = MakeEnumValue(false),
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

	private static string MakePropertyKey<T>(System.Linq.Expressions.Expression<Func<DeclarationComparerOptions, T>> expression)
	{
		var prefixLookup = new Dictionary<string, string>();
		foreach (var property in typeof(DeclarationComparerOptions).GetProperties())
		{
			prefixLookup[property.Name] = property.Name switch
			{
				nameof(DeclarationComparerOptions.AccessibilityOrder) => "dotnet_diagnostic.rms0005.",
				nameof(DeclarationComparerOptions.IdentifierNames) => "dotnet_diagnostic.rms0007.",
				nameof(DeclarationComparerOptions.ArityOrder) => "dotnet_diagnostic.rms0008.",
				nameof(DeclarationComparerOptions.ExplicitInterfaceSpecifiers) => "dotnet_diagnostic.rms0006.",
				nameof(DeclarationComparerOptions.FieldOrder) => "dotnet_diagnostic.rms0004.",
				nameof(DeclarationComparerOptions.KindOrder) => "dotnet_diagnostic.rms0002.",
				nameof(DeclarationComparerOptions.MergeEvents) => "dotnet_diagnostic.rms0002.",
				nameof(DeclarationComparerOptions.OperatorOrder) => "dotnet_diagnostic.rms0007.",
				nameof(DeclarationComparerOptions.ParameterNames) => "dotnet_diagnostic.rms0010.",
				nameof(DeclarationComparerOptions.ParameterTypeNames) => "dotnet_diagnostic.rms0009.",
				nameof(DeclarationComparerOptions.ReferenceParameterOrder) => "dotnet_diagnostic.rms0009.",
				nameof(DeclarationComparerOptions.SortOrders) => "dotnet_diagnostic.rms_shared.",
				nameof(DeclarationComparerOptions.Static) => "dotnet_diagnostic.rms0003.",
				nameof(DeclarationComparerOptions.UnknownAccessibilityOrder) => "dotnet_diagnostic.rms0005.",
				nameof(DeclarationComparerOptions.UnknownFieldMutabilityOrder) => "dotnet_diagnostic.rms0004.",
				nameof(DeclarationComparerOptions.UnknownKindOrder) => "dotnet_diagnostic.rms0001.",
				nameof(DeclarationComparerOptions.UnknownOperatorTokenOrder) => "dotnet_diagnostic.rms0007.",
#pragma warning disable RCS1079, RCS1140
				_ => throw new NotImplementedException($"Property {property.Name} prefix not assigned.")
#pragma warning restore RCS1079, RCS1140
			};
		}
		if (expression.Body is System.Linq.Expressions.MemberExpression memberExpression)
		{
			return prefixLookup[memberExpression.Member.Name] + memberExpression.Member.Name.ToSnakeCase();
		}
		else
		{
			throw new ArgumentException("Expression is not a member access", nameof(expression));
		}
	}

	private async Task VerifyDiagnosticAndFixAsync(TestCode source, DiagnosticDescriptor diagnostic, string expectedFix, Dictionary<string, string> config)
	{
		await VerifyDiagnosticAndFixAsync(new DiagnosticTestData(diagnostic, source.Value, source.Spans, source.AdditionalSpans), new ExpectedTestState(expectedFix, "Fix this member order"), CommonOptions.WithConfigOptions(CommonOptions.ConfigOptions.SetItems(config))).ConfigureAwait(false);
	}

	private async Task VerifyNoDiagnosticAsync(TestCode source, DiagnosticDescriptor diagnostic, Dictionary<string, string> config)
	{
		await VerifyNoDiagnosticAsync(new DiagnosticTestData(diagnostic, source.Value, source.Spans, source.AdditionalSpans), CommonOptions.WithConfigOptions(CommonOptions.ConfigOptions.SetItems(config))).ConfigureAwait(false);
	}

	[Fact]
	public async Task Accessibility_EmptyOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    private int B;\r\n    public int A;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Accessibility)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.AccessibilityOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Accessibility_InOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    private int B;\r\n    public int A;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Private, Accessibility.Public),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Accessibility)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.AccessibilityOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Accessibility_Order_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    private int B;\r\n    [|public int A;|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    public int A;\r\n    private int B;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Public, Accessibility.Private),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Accessibility)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.AccessibilityOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Arity_Default_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Method(int a, int b) => throw new System.NotImplementedException();\r\n    int Method(int a) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ArityOrder)] = MakeEnumValue(ArityOrder.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterArity)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.ParameterArityOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Arity_HighToLow_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Method(int a, int b) => throw new System.NotImplementedException();\r\n    int Method(int a) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ArityOrder)] = MakeEnumValue(ArityOrder.HighToLow),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterArity)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.ParameterArityOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Arity_LowToHigh_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Method(int a, int b) => throw new System.NotImplementedException();\r\n    [|int Method(int a) => throw new System.NotImplementedException();|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    int Method(int a) => throw new System.NotImplementedException();\r\n    int Method(int a, int b) => throw new System.NotImplementedException();\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ArityOrder)] = MakeEnumValue(ArityOrder.LowToHigh),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterArity)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.ParameterArityOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ExplicitInterfaceSpecifiers_Default_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test : System.Collections.IEnumerable\r\n{\r\n    System.Collections.IEnumerator GetEnumerator() => throw new System.NotImplementedException();\r\n    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ExplicitInterfaceSpecifier)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.ExplicitInterfaceSpecifierOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ExplicitInterfaceSpecifiers_First_Async()
	{
		var source = TestCode.Parse("class test : System.Collections.IEnumerable\r\n{\r\n    System.Collections.IEnumerator GetEnumerator() => throw new System.NotImplementedException();\r\n    [|System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();|]\r\n}\r\n");
		const string expectedFix = "class test : System.Collections.IEnumerable\r\n{\r\n    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();\r\n    System.Collections.IEnumerator GetEnumerator() => throw new System.NotImplementedException();\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.First),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ExplicitInterfaceSpecifier)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.ExplicitInterfaceSpecifierOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ExplicitInterfaceSpecifiers_Last_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test : System.Collections.IEnumerable\r\n{\r\n    System.Collections.IEnumerator GetEnumerator() => throw new System.NotImplementedException();\r\n    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ExplicitInterfaceSpecifiers)] = MakeEnumValue(Order.Last),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ExplicitInterfaceSpecifier)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.ExplicitInterfaceSpecifierOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Field_EmptyOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int MutableField;\r\n    const int ConstField = 0;\r\n    readonly int ReadOnlyField;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.FieldOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.FieldOrder)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.FieldOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Field_InOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int MutableField;\r\n    const int ConstField = 0;\r\n    readonly int ReadOnlyField;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.FieldOrder)] = MakeEnumList(FieldMutability.Mutable, FieldMutability.Const, FieldMutability.ReadOnly),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.FieldOrder)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.FieldOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Field_Order_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int MutableField;\r\n    [|const int ConstField = 0;|]\r\n    readonly int ReadOnlyField;\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    const int ConstField = 0;\r\n    readonly int ReadOnlyField;\r\n    int MutableField;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.FieldOrder)] = MakeEnumList(FieldMutability.Const, FieldMutability.ReadOnly, FieldMutability.Mutable),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.FieldOrder)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.FieldOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Identifier_Alphabetical_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int B;\r\n    [|int A;|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    int A;\r\n    int B;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.IdentifierNames)] = MakeEnumValue(IdentifierOrder.Alphabetical),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.IdentifierOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Identifier_Default_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int B;\r\n    int A;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.IdentifierNames)] = MakeEnumValue(IdentifierOrder.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.IdentifierOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Identifier_ReverseAlphabetical_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int B;\r\n    int A;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.IdentifierNames)] = MakeEnumValue(IdentifierOrder.ReverseAlphabetical),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.IdentifierOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Kind_EmptyOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Field;\r\n    int Property\r\n     {\r\n        get;\r\n    }\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		// default sort order means do not reorder, verify no diagnostic
		await VerifyNoDiagnosticAsync(source, DiagnosticIds.KindOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Kind_InOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Field;\r\n    int Property\r\n     {\r\n        get;\r\n    }\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.FieldDeclaration, SyntaxKind.PropertyDeclaration),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.KindOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Kind_Order_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Field;\r\n    [|int Property\r\n     {\r\n        get;\r\n    }|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    int Property\r\n    {\r\n        get;\r\n    }\r\n    int Field;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.PropertyDeclaration, SyntaxKind.FieldDeclaration),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.KindOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task MergeEvents_False_InOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    event System.EventHandler MultiLine\r\n    {\r\n        add => throw new System.NotImplementedException();\r\n        remove => throw new System.NotImplementedException();\r\n    }\r\n    event System.EventHandler SingleLine;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.EventDeclaration, SyntaxKind.EventFieldDeclaration),
			[MakePropertyKey(dco => dco.MergeEvents)] = MakeEnumValue(false),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.KindOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task MergeEvents_False_Order_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    event System.EventHandler MultiLine\r\n    {\r\n        add => throw new System.NotImplementedException();\r\n        remove => throw new System.NotImplementedException();\r\n    }\r\n    [|event System.EventHandler SingleLine;|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    event System.EventHandler SingleLine;\r\n    event System.EventHandler MultiLine\r\n    {\r\n        add => throw new System.NotImplementedException();\r\n        remove => throw new System.NotImplementedException();\r\n    }\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.EventFieldDeclaration, SyntaxKind.EventDeclaration),
			[MakePropertyKey(dco => dco.MergeEvents)] = MakeEnumValue(false),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.KindOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task MergeEvents_True_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    event System.EventHandler MultiLine\r\n    {\r\n        add => throw new System.NotImplementedException();\r\n        remove => throw new System.NotImplementedException();\r\n    }\r\n    event System.EventHandler SingleLine;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.KindOrder)] = MakeEnumList(SyntaxKind.EventFieldDeclaration, SyntaxKind.EventDeclaration),
			[MakePropertyKey(dco => dco.MergeEvents)] = MakeEnumValue(true),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Kind)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.KindOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Operator_EmptyOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    public static test operator -(test a, test b) => throw new System.NotImplementedException();\r\n    public static test operator +(test a, test b) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.OperatorOrder)] = string.Empty,
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.IdentifierOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Operator_InOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    public static test operator -(test a, test b) => throw new System.NotImplementedException();\r\n    public static test operator +(test a, test b) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.OperatorOrder)] = MakeEnumList(SyntaxKind.MinusToken, SyntaxKind.PlusToken),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.IdentifierOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Operator_Order_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    public static test operator -(test a, test b) => throw new System.NotImplementedException();\r\n    [|public static test operator +(test a, test b) => throw new System.NotImplementedException();|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    public static test operator +(test a, test b) => throw new System.NotImplementedException();\r\n    public static test operator -(test a, test b) => throw new System.NotImplementedException();\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.OperatorOrder)] = MakeEnumList(SyntaxKind.PlusToken, SyntaxKind.MinusToken),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Identifier)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.IdentifierOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ParameterName_Alphabetical_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    [|void Method(string a) => throw new System.NotImplementedException();|]\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ParameterNames)] = MakeEnumValue(IdentifierOrder.Alphabetical),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterNames)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.ParameterNameOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ParameterName_Default_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ParameterNames)] = MakeEnumValue(IdentifierOrder.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterNames)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.ParameterNameOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ParameterType_Alphabetical_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n    [|void Method(double b) => throw new System.NotImplementedException();|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ParameterTypeNames)] = MakeEnumValue(IdentifierOrder.Alphabetical),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterTypes)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.ParameterTypeOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task ParameterType_Default_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    void Method(int c) => throw new System.NotImplementedException();\r\n    void Method(string a) => throw new System.NotImplementedException();\r\n    void Method(double b) => throw new System.NotImplementedException();\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.ParameterTypeNames)] = MakeEnumValue(IdentifierOrder.Default),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.ParameterTypes)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.ParameterTypeOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Sort_EmptyOrder_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    public int PublicInstance;\r\n    private static int PrivateStatic;\r\n    private int PrivateInstance;\r\n    public static int PublicStatic;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Public, Accessibility.Private),
			[MakePropertyKey(dco => dco.SortOrders)] = string.Empty,
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.First)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.AccessibilityOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Sort_Order_AccessibilityStatic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    public int PublicInstance;\r\n    private static int PrivateStatic;\r\n    private int PrivateInstance;\r\n    [|public static int PublicStatic;|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    public static int PublicStatic;\r\n    public int PublicInstance;\r\n    private static int PrivateStatic;\r\n    private int PrivateInstance;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Public, Accessibility.Private),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumList(SortOrder.Accessibility, SortOrder.Static),
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.First)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.AccessibilityOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Sort_Order_StaticAccessibility_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    public int PublicInstance;\r\n    [|private static int PrivateStatic;|]\r\n    private int PrivateInstance;\r\n    [|public static int PublicStatic;|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    public static int PublicStatic;\r\n    private static int PrivateStatic;\r\n    public int PublicInstance;\r\n    private int PrivateInstance;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.AccessibilityOrder)] = MakeEnumList(Accessibility.Public, Accessibility.Private),
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumList(SortOrder.Static, SortOrder.Accessibility),
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.First)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.StaticOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Static_Default_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Instance;\r\n    static int Static;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Static),
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.Default)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.StaticOutOfOrder, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Static_First_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Instance;\r\n    [|static int Static;|]\r\n}\r\n");
		const string expectedFix = "class test\r\n{\r\n    static int Static;\r\n    int Instance;\r\n}\r\n";
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Static),
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.First)
		};

		await VerifyDiagnosticAndFixAsync(source, DiagnosticIds.StaticOutOfOrder, expectedFix, config).ConfigureAwait(false);
	}

	[Fact]
	public async Task Static_Last_NoDiagnostic_Async()
	{
		var source = TestCode.Parse("class test\r\n{\r\n    int Instance;\r\n    static int Static;\r\n}\r\n");
		var config = new Dictionary<string, string>()
		{
			[MakePropertyKey(dco => dco.SortOrders)] = MakeEnumValue(SortOrder.Static),
			[MakePropertyKey(dco => dco.Static)] = MakeEnumValue(Order.Last)
		};

		await VerifyNoDiagnosticAsync(source, DiagnosticIds.StaticOutOfOrder, config).ConfigureAwait(false);
	}
}
