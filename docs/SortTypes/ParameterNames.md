# RMS0010: Parameter Names

Constructors, delegates, indexers, and methods can be sorted by the names of the parameters they define.

This option is used during the `parameter_names` sorting stage.

```editorconfig
dotnet_diagnostic.rms0010.parameter_names = alphabetical
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Parameter names are not sorted alphabetically. |
| `alphabetical` | Parameter names are sorted in alphabetical order. |
| `reverse_alphabetical` | Parameter names are sorted in reverse alphabetical order. |
