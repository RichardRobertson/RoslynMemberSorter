## RMS0008: Parameter Arity

Constructors, delegates, indexers, and methods can be sorted by the number of parameters they define. Arity of a method is the number of parameters it has; therefore, `void none()` has arity of zero and `void three(int a, int b, int c)` has an arity of three. This results in `RMS0008` being reported on the member that is out of order.

This option is used during the `parameter_arity` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0008.arity_order = low_to_high
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Method arity is ignored for sorting purposes. |
| `low_to_high` | Methods with low arity come before methods with high arity. |
| `high_to_low` | Methods with low arity come after methods with high arity. |
