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
    - `ParametersComparer`
- `ArityOrder` enum which provides more clarity when sorting arity.

### Changed

- Moved enums into `RoslynMemberSorter.Enums` namespace and matching folder.
- `FieldMutability` comment documentation to use keyword references.
- `Order` comment documentation to remove mention of `DeclarationComparerOptions` and changed 'detail' to 'quality'.

### Deprecated

- Diagnostic `RMS0001` which was reported on the namespace or type that declared the member out of order.
- `DeclarationComparer` class in favor of new comparers.

### Fixed

- Changelog date from "month-name day, year" to "year-month-day".

### Removed

- Tossed `RoslynMemberSorter.Accessibility` in favor of using `Microsoft.CodeAnalysis.Accessibility`.

## [0.1.0-alpha] 2022-03-25

Initial release
