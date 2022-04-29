# RMS0007: Identifier

Members can be sorted alphabetically by name. This results in `RMS0007` being reported on the member that is out of order.

This option is used during the `identifier` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0007.identifier_names = alphabetical
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Names are not sorted alphabetically. |
| `alphabetical` | Names are sorted in alphabetical order. |
| `reverse_alphabetical` | Names are sorted in reverse alphabetical order. |

___

Overloaded operators do not have names but instead use tokens and keywords to identify them.

```editorconfig
dotnet_diagnostic.rms0007.operator_order = plus, minus, exclamation, tilde, plus_plus, minus_minus, asterisk, slash, percent, ampersand, bar, caret, less_than_less_than, greater_than_greater_than, equals_equals, exclamation_equals, less_than, greater_than, less_than_equals, greater_than_equals, true, false
```

The default value is listed above and contains all possible values. The names come from the Roslyn enum [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind).

___

```editorconfig
dotnet_diagnostic.rms0007.unknown_operator_order = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Operators not listed in `operator_order` are ignored for sorting purposes. |
| `first` | Operators not listed in `operator_order` are listed before those that are. |
| `last` | Operators not listed in `operator_order` are listed after those that are. |
