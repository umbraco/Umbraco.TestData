## Usage
See `dotnet UmbracoTestData.dll --help` for all mandatory and optional parameters

#### Example
Backoffice url: `http://localhost:8100/umbraco`

Number of items to create: `5000`

Username: `<email>`

Password: `<password>`

`dotnet UmbracoTestData.dll -b http://localhost:8100/umbraco -n 5000 -u <email> -p <password>`

## Publish / Release
### Prerequisite

- [.Net Core 2.2](https://dotnet.microsoft.com/download)


### Make the binary 
When a new version is ready. Tag the version in git and add the binary as part of the release on GitHub.

To create the binary. Run the following command:
```
dotnet publish -c Release -r win10-x64
```