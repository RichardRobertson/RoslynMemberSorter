# RMS0005: Accessibility

Members can be sorted based on their accessibility level. This results in `RMS0005` being reported on the member that is out of order.

This option is used during the `accessibility` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0005.accessibility_order = public, protected_or_internal, internal, protected, private_and_internal, private
```

The default value is listed above and contains all possible values. The names come from the Roslyn enum [Accessibility](https://docs.microsoft.com/en-us/dotnet/api/microsoft.codeanalysis.accessibility).

> `protected_or_internal` matches members with both `protected internal` modifiers.
> `protected_and_internal` matches members with both `protected private` modifiers.
>
> `friend` can be used in place of `internal` as both names are equivalent in the `Accessibility` enum.

___

```editorconfig
dotnet_diagnostic.rms0005.unknown_accessibility_order = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Accessibility values not listed in `accessibility_order` are ignored for sorting purposes. |
| `first` | Accessibility values not listed in `accessibility_order` are listed before those that are. |
| `last` | Accessibility values not listed in `accessibility_order` are listed after those that are. |
