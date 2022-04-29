# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Individual diagnostics have been created for each sort order issue.
- Individual comparer classes:
    - `MultiComparer` class which can run comparers in sequence
    - `HasQuality<TCompared>` abstract base class
    - `IndexedComparer<TCompared, TQuality>` abstract base class
    - `AccessibilityComparer`
    - `FieldDeclarationMutabilityComparer`
    - `HasExplicitInterfaceSpecifierComparer`
    - `IdentifierComparer`
        - `IdentifierComparer.OperatorComparer`
    - `IsStaticComparer`
    - `KindComparer`
    - `ParametersComparerBase`
        - `ParameterArityComparer`
        - `ParameterNameComparer`
        - `ParameterTypeComparer`
- `ArityOrder` enum which provides more clarity when sorting arity.
- `DeclarationComparerOptions`
    - `ToCSharpComparer()` to create a new comparer chain from options.
    - Additional properties to match new comparers.
- Second code fix provider to fix all members of a container at once.

### Changed

- Moved enums into `RoslynMemberSorter.Enums` namespace and matching folder.
- `FieldMutability` comment documentation to use keyword references.
- `Order` comment documentation to remove mention of `DeclarationComparerOptions` and changed 'detail' to 'quality'.
- Change the analyzer and code fix provider to use `DeclarationComparerOptions.ToCSharpComparer()`
- Updated unit tests to use moved enums and comparers.
- `DeclarationComparerOptions`
    - Property `Order SingleLineEvents` has changed to `bool MergeEvents`
    - Property `Order LowArity` has changed to `ArityOrder ArityOrder`
- `NameOrder` enum renamed to `IdentifierOrder`
- Moved options to `dotnet_diagnostic.rms####.` where #### is the diagnostic number relevant to the sorting method.
- `SortOrders` is now option `dotnet_diagnostic.rms_shared.sort_orders`.

### Deprecated

- Diagnostic `RMS0001` which was reported on the namespace or type that declared the member out of order.

### Fixed

- Changelog date from "month-name day, year" to "year-month-day".

### Removed

- `RoslynMemberSorter.Accessibility` enum in favor of using `Microsoft.CodeAnalysis.Accessibility`.
- `DeclarationComparer` class in favor of new comparers.
- `ParameterSortStyle` enum in favor of new comparers.

## [0.1.0-alpha] 2022-03-25

Initial release
