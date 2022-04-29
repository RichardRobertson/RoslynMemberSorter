# RMS0002: Kind

Members can be sorted based on what kind of member they are. Examples are properties, nested classes, and constructors. This results in `RMS0002` being reported on the member that is out of order.

This option is used during the `kind` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0002.kind_order = field, constructor, destructor, indexer, property, event_field, event, method, operator, conversion_operator, enum, interface, struct, class, record, record_struct, delegate
```

The default value is listed above and contains all possible values. The names come from the Roslyn enum [SyntaxKind](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.csharp.syntaxkind).

___

```editorconfig
dotnet_diagnostic.rms0002.unknown_kind_order = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Kinds not listed in `kind_order` are ignored for sorting purposes. |
| `first` | Kinds not listed in `kind_order` are listed before those that are. |
| `last` | Kinds not listed in `kind_order` are listed after those that are. |

___

```editorconfig
dotnet_diagnostic.rms0002.merge_events = true
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `true` | Field and property style event declarations are treated the same for sorting. |
| `false` | Field and property style event declarations are separated into `event` and `event_field` kinds. |


> Event field declarations look like fields.
> ```C#
> public event System.EventHandler MyEvent;
> ```
> Event declarations look like properties with accessors.
> ```C#
> public event System.EventHandler MyEvent
> {
>     add => throw new System.NotImplementedException();
>     remove => throw new System.NotImplementedException();
> }
> ```
