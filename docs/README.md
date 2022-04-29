# RoslynMemberSorter Usage

RoslynMemberSorter uses configuration lines in `.editorconfig`. Some options use comma delimited lists while others use a single enumeration value.

Invalid assignments are silently ignored.
- For list options, the invalid list item will be dropped.
    - E.g. for `...accessibility = private, bad_value, public`, the item `bad_value` is dropped and `private, public` will be used.
- For single value options, the assignment will be dropped, and the default will
be used.
    - E.g. for `...static = bad_value`, the default value of `first` will be used.

When a member is found out of order, a diagnostic such as `RMS0002` will be emitted on the member. The number will vary depending on what quality has been determined to be out of order.

# Sort orders

The order that each member detail may be sorted can be configured. This influences which details are sorted as well as the order they are grouped by. Any detail not in the list will not be sorted at all.

> Changing the order sorts are done can drastically change the final order. For example:
> - `sort_orders = static, accessibility` can produce the order
>     - `public static int PublicStatic;`
>     - `private static int PrivateStatic;`
>     - `public int PublicInstance;`
>     - `private int PrivateInstance;`
> - `sort_orders = accessibility, static` can produce the order
>     - `public static int PublicStatic;`
>     - `public int PublicInstance;`
>     - `private static int PrivateStatic;`
>     - `private int PrivateInstance;`

## Options

```editorconfig
dotnet_diagnostic.rms_shared.sort_orders = kind, static, field_order, accessibility, explicit_interface_specifier, identifier, parameter_arity, parameter_types
```

The default value is listed above. Choices for this option are:
| Value | Description | Link |
| - | - | - |
| `accessibility` | Sort members based on their accessibility. | [Accessibility](SortTypes/Accessibility.md) |
| `explicit_interface_specifier` | Sort members based on whether they explicitly implement an interface. | [Explicit Interface Specifier](SortTypes/ExplicitInterfaceSpecifier.md) |
| `field_order` | Sort fields based on their mutability. | [Field Order](SortTypes/FieldOrder.md) |
| `identifier` | Sort members based on their identifier. | [Identifier](SortTypes/Identifier.md) |
| `kind` | Sort members based on their kind. | [Kind](SortTypes/Kind.md) |
| `parameter_arity` | Sort method-like members based on their arity. | [Parameter Arity](SortTypes/ParameterArity.md) |
| `parameter_names` | Sort method-like members based on their parameter names. | [Parameter Names](SortTypes/ParameterNames.md) |
| `parameter_types` | Sort method-like members based on their parameter type names. | [Parameter Types](SortTypes/ParameterTypes.md) |
| `static` | Sort members based on whether they are static or instance. | [Static](SortTypes/Static.md) |
