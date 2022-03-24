# RoslynMemberSorter

RoslynMemberSorter uses Roslyn code analysis to detect out of order namespace and type member declarations in C# projects.

## Installation

Use the dotnet CLI to install RoslynMemberSorter to a project:

```
dotnet add package RichardRobertson.RoslynMemberSorter --version 0.1.0-alpha
```

Or add it to your `.csproj` manually in an `<ItemGroup>`:

```xml
<Project Sdk="Microsoft.NET.Sdk">
    <!-- ... -->
    <ItemGroup>
        <PackageReference Include="RichardRobertson.RoslynMemberSorter" Version="0.1.0-alpha">
            <PrivateAssets>all</PrivateAssets>
            <IncludeAssets>analyzers</IncludeAssets>
        <PackageReference>
    </ItemGroup>
</Project>
```

## Usage

RoslynMemberSorter uses configuration lines in `.editorconfig`. Some options use comma delimited lists while others use a single enumeration value. Each option will be listed below with its default setting and other available values.

When a namespace or type is found with members out of order, diagnostic `RMS0001` is emitted on the identifier of the namespace or type.

Invalid assignments are silently ignored.
- For list options, the invalid list item will be dropped.
    - E.g. for `...accessibility = private, bad_value, public`, the item `bad_value` is dropped and `private, public` will be used.
- For single value options, the assignment will be dropped, and the default will
be used.
    - E.g. for `...static = bad_value`, the default value of `first` will be used.

### Accessibility order

Members can be sorted based on their accessibility level. Any accessibility level not in the list will be placed first due to how the sort works internally.

This option is used during the `accessibility` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.accessibility_order = public, protected_internal, internal, protected, private_protected, private
```

The default value is listed above and contains all possible values.

### Alphabetical identifiers

Members can be sorted alphabetically by name.

This option is used during the `identifier` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.alphabetical_identifiers = alphabetical
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Names are not sorted alphabetically. |
| `alphabetical` | Names are sorted in alphabetical order. |
| `reverse_alphabetical` | Names are sorted in reverse alphabetical order. |

### Explicit interface specifier

Types can define members that implement interfaces explicitly. These have no inherent accessibility but can be sorted before or after other members of the same kind.

This option is used during the `explicit_interface_specifer` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.explicit_interface_specifier = default
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Explicit interface implementation is ignored for sorting purposes. |
| `first` | Members that explicitly implement an interface are sorted before members that do not. |
| `last` | Members that explicitly implement an interface as sorted after members that do not. |

### Field order

Fields can be sorted based on their mutability. Any mutability level not in the list will be placed first due to how the sort works internally.

This option is used during the `field_order` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.field_order = constant, read_only, mutable
```

The default value is listed above and contains all possible values. Each mutability level is defined as:
| Value | Definition |
| - | - |
| `constant` | Fields declared with the `const` modifier. |
| `mutable` | Fields declared with neither the `const` nor the `readonly` modifier. |
| `read_only` | Fields declared with the `readonly` modifier. |

### Kind order

Members can be sorted based on what kind of member they are. Examples are properties, nested classes, and constructors. Any type not in the list will be placed first due to how the sort works internally.

This option is used during the `kind` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.kind_order = field, constructor, destructor, indexer, property, event_field, event, method, operator, conversion_operator, enum, interface, struct, class, record, record_struct, delegate
```

The default value is listed above and contains all possible values. The names come from the Roslyn enum [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind).

### Low arity

Constructors, delegates, indexers, and methods can be sorted by the number of parameters they define. Arity of a method is the number of parameters it has; therefore, `void none()` has arity of zero and `void three(int a, int b, int c)` has an arity of three.

This option is used during the `parameters` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.low_arity = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Method arity is ignored for sorting purposes. |
| `first` | Methods with low arity come before methods with high arity. |
| `last` | Methods with low arity come after methods with high arity. |

### Operator order

Overloaded operators do not have names but instead use tokens and keywords to identify them. Any operator not in the list will be placed first due to how the sort works internally.

This option is used during the `identifier` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.operator_order = plus, minus, exclamation, tilde, plus_plus, minus_minus, asterisk, slash, percent, ampersand, bar, caret, less_than_less_than, greater_than_greater_than, equals_equals, exclamation_equals, less_than, greater_than, less_than_equals, greater_than_equals, true, false
```

The default value is listed above and contains all possible values. The names come from the Roslyn enum [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind).

### Parameter sort style

Constructors, delegates, indexers, and methods can be sorted by the types or names of parameters they define.

This option is used during the `parameters` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.parameter_sort_style = sort_types
```

The default value is listed above. Choices for this option are:
| Value | Definition | Example |
| - | - | - |
| `default` | Parameter types and names are ignored for sorting purposes. | |
| `sort_types` | Sort members using the type name of their parameters. | `void method(int b)` before `void method(string a)` |
| `sort_names` | Sort members using the name of their parameters. | `void method(string a)` before `void method(int b)` |

### Single line events

Events can be sorted based on whether they are defined on a single line or with accessors.

> Single line events look like fields.
> ```C#
> public event System.EventHandler MyEvent;
> ```
> Events with accessors look like properties.
> ```C#
> public event System.EventHandler MyEvent
> {
>     add => throw new System.NotImplementedException();
>     remove => throw new System.NotImplementedException();
> }
> ```

This option is used during the `kind` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.single_line_events = default
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Single line or accessor status is ignored for sorting purposes. |
| `first` | Single line events come before events with accessors. |
| `last` | Single line events come after events with accessors. |

### Sort orders

The order that each member detail may be sorted can be configured. This influences which details are sorted at all as well as the order they are grouped by. Any detail not in the list will not be sorted at all.

> Changing the order sorts are done can drastically change the final order. For example:
>- `sort_orders = static, accessibility` can produce the order
>    - `public static int PublicStatic;`
>    - `private static int PrivateStatic;`
>    - `public int PublicInstance;`
>    - `private int PrivateInstance;`
>- `sort_orders = accessibility, static` can produce the order
>    - `public static int PublicStatic;`
>    - `public int PublicInstance;`
>    - `private static int PrivateStatic;`
>    - `private int PrivateInstance;`

```editorconfig
dotnet_diagnostic.rms0001.sort_orders = kind, static, field_order, accessibility, explicit_interface_specifier, identifier, parameters
```

The default value is listed above and contains all possible values.

### Static order

Members can be sorted based on being static or instance.

This option is used during the `static` sorting stage.

```editorconfig
dotnet_diagnostic.rms0001.static = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Static or instance status is ignored for sorting purposes. |
| `first` | Static members come before instance members. |
| `last` | Static members come after instance members. |

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
