# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Individual diagnostics have been created for each sort order issue.

### Changed

- Moved enums into `RoslynMemberSorter.Enums` namespace and matching folder

### Deprecated

- Diagnostic `RMS0001` which was reported on the namespace or type that declared the member out of order.

### Fixed

- Changelog date from "month-name day, year" to "year-month-day".

### Removed

- Tossed `RoslynMemberSorter.Accessibility` in favor of using `Microsoft.CodeAnalysis.Accessibility`.

## [0.1.0-alpha] 2022-03-25

Initial release
