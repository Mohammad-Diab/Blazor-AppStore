# Blazor-AppStore

A web "app store" that shows the contents of a folder on the server like a file explorer: categories, apps, icons and files, with download links. The front end is a **Blazor WebAssembly** site; the back end is an **ASP.NET Core 3.1** API that reads the folder.

## Features
- Browse folders and apps with icons and breadcrumbs.
- Search across all apps.
- View small text files in the page (up to 256 KB by default).
- Download a file, or a whole folder as a zip.
- A "last accessed" list.

## How it works
- **AppStoreServer** (API): lists the folder set by `AppDirectory` in `config.xml`, serves icons from its `$ICONS` subfolder and files on request. CORS is open, so the site can be hosted on another address.
- **AppStore** (site): static Blazor WebAssembly files. `appsettings.json` holds `ApiUrl`, the address of the API.

## Run it from the release
1. Download both archives from [Releases](https://github.com/Mohammad-Diab/Blazor-AppStore/releases) and extract them with [7-Zip](https://www.7-zip.org).
2. Put the apps you want to show in `C:\AppStore\` (one folder per category), or change `AppDirectory` in `AppStoreServer\config.xml`.
3. Run `AppStoreServer\AppStoreServer.exe`. It listens on `http://localhost:5000`. It is self-contained: no .NET install needed.
4. Serve the `AppStore` folder under the path `/AppStore/`, for example from the folder that contains it:
   ```
   python -m http.server 18080
   ```
   and open `http://localhost:18080/AppStore/`.

In the release, the site expects the API at `http://localhost:5000/Apps/` (the source defaults to the IIS layout, `http://localhost/AppStoreServer/Apps/`). To host it elsewhere (IIS, another port), change `ApiUrl` in `AppStore\appsettings.json` and `AppStoreServer\config.xml`.

## Build
Open `AppStore.sln` in Visual Studio, or:
```
dotnet publish AppStoreServer -c Release -r win-x64 --self-contained true
dotnet publish AppStore -c Release
```
The projects target .NET Core 3.1 and Blazor WebAssembly 3.2. They still build with the current .NET SDK (with an end-of-support warning).

## License
[Apache 2.0](LICENSE). Image credits are in `Images copyright.txt`.
