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

RoslynMemberSorter uses configuration lines in `.editorconfig`. Some options use comma delimited lists while others use a single enumeration value. See [docs](docs) for complete details on each sorting method and its options.

When a member is found out of order, a diagnostic such as `RMS0002` will be emitted on the member. The number will vary depending on what quality has been determined to be out of order.

## Contributing

Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License

[MIT](https://choosealicense.com/licenses/mit/)
