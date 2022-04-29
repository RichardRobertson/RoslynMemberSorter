# RMS0006: Explicit Interface Specifier

Types can define members that implement interfaces explicitly. These have no inherent accessibility but can be sorted before or after other members of the same kind. This results in `RMS0006` being reported on the member that is out of order.

This option is used during the `explicit_interface_specifer` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0006.explicit_interface_specifiers = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Explicit interface implementation is ignored for sorting purposes. |
| `first` | Members that explicitly implement an interface are sorted before members that do not. |
| `last` | Members that explicitly implement an interface as sorted after members that do not. |
