# RMS0003: Static

Members can be sorted based on being static or instance. This results in `RMS0003` being reported on the member that is out of order.

This option is used during the `static` sorting stage.

## Options

```editorconfig
dotnet_diagnostic.rms0003.static = first
```

The default value is listed above. Choices for this option are:
| Value | Definition |
| - | - |
| `default` | Static or instance status is ignored for sorting purposes. |
| `first` | Static members come before instance members. |
| `last` | Static members come after instance members. |
