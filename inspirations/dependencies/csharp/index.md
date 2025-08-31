We can use a prepack script:

```xml
<Target Name="PrePackScript" BeforeTargets="Pack">
  <Exec Command="bash ../scripts/prepublish.sh" />
</Target>
```

For example by replacing all the project references with the latest versions of the related package:

```sh
#!/bin/bash
# filepath: scripts/prepublish.sh

csproj="$1"

if [ -z "$csproj" ]; then
  echo "Usage: $0 path/to/your.csproj"
  exit 1
fi

# Extract all ProjectReference includes
grep '<ProjectReference' "$csproj" | sed -E 's/.*Include="([^"]+)".*/\1/' | while read -r ref; do
  # Get the project name from the path (assumes project file name matches package name)
  proj_name=$(basename "$ref" .csproj)

  # Query nuget.org for the latest version
  version=$(curl -s "https://api.nuget.org/v3-flatcontainer/$proj_name/index.json" | jq -r '.versions[-1]')

  if [ "$version" = "null" ] || [ -z "$version" ]; then
    echo "Could not find version for $proj_name, skipping."
    continue
  fi

  # Escape slashes for sed
  ref_escaped=$(echo "$ref" | sed 's/\//\\\//g')

  # Replace ProjectReference with PackageReference
  sed -i.bak "/<ProjectReference Include=\"$ref_escaped\" \/>/c\\
    <PackageReference Include=\"$proj_name\" Version=\"$version\" />
  " "$csproj"

  echo "Replaced $ref with $proj_name $version"
done
```

Although, this is important