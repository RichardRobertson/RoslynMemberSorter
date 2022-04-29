# RMS0004: Field Order

Fields can be sorted based on their mutability. This results in `RMS0004` being reported on the member that is out of order.

This option is used during the `field_order` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0004.field_order = const, read_only, mutable
```

The default value is listed above and contains all possible values. Each mutability level is defined as:
| Value | Definition |
| - | - |
| `const` | Fields declared with the `const` modifier. |
| `mutable` | Fields declared with neither the `const` nor the `readonly` modifier. |
| `read_only` | Fields declared with the `readonly` modifier. |

___

```editorconfig
dotnet_diagnostic.rms0004.unknown_field_mutability_order = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Field mutability values not listed in `field_order` are ignored for sorting purposes. |
| `first` | Field mutability values not listed in `field_order` are listed before those that are. |
| `last` | Field mutability values not listed in `field_order` are listed after those that are. |
