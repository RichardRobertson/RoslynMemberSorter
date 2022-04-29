# RMS0009: Parameter Types

Constructors, delegates, indexers, and methods can be sorted by the types of the parameters they define.

This option is used during the `parameter_types` sorting stage.

```editorconfig
dotnet_diagnostic.rms0009.parameter_type_names = alphabetical
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Parameter type names are not sorted alphabetically. |
| `alphabetical` | Parameter type names are sorted in alphabetical order. |
| `reverse_alphabetical` | Parameter type names are sorted in reverse alphabetical order. |

___

```editorconfig
dotnet_diagnostic.rms0009.reference_parameter_order = first
```

| Value | Definition |
| - | - |
| `default` | Parameters are not sorted by whether they are passed by reference. |
| `first` | Parameters passed by reference are sorted before those that are not. |
| `last` | Parameters sorted by reference are sorted after those that are not. |
