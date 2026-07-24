# CmdPal Capacities

Capacities integration for Windows Command Palette (PowerToys CmdPal).

Search objects, open them, append text, and create new objects without leaving Command Palette.

## Current status

1. Current milestone: Version 0.9 release candidate
2. Target first public release: Version 1.0
3. Planned distribution order:
   - winget
   - CmdPal gallery
   - Microsoft Store (Version 1.2)

## Screenshots

Search, create, and append objects without leaving Command Palette:

![Search Capacities objects](docs/gallery/screenshots/1-search-objects.png)
![Create new objects](docs/gallery/screenshots/2-create-object.png)
![Enter content](docs/gallery/screenshots/3-create-content.png)

## Features

1. Search-first object flow from the top-level command
2. Grouped results by structure (for example List, Page)
3. Object actions:
   - Open in App
   - Open in Web
   - Append then open App
   - Append then open Web
   - Append only
4. Create object flow:
   - choose structure
   - enter object name
   - enter content or leave it empty to create a blank object
   - post-create behavior: open App, open Web, or do nothing
5. API token handling:
   - masked token in visible settings
   - full token stored outside visible settings
6. Search resiliency:
   - minimum query length
   - debounce
   - cache usage
   - transient error fallback behavior
7. Structure exclusions:
   - RootQuery
   - MediaImage
   - RootDailyNote
   - RootAIChat
   - RootTask
   - MediaWebResource
   - RootSimpleTable
   - RootTag
   - MediaPDF

## Usage

Invoke the extension alias (for example: cap) and type at least 3 characters to search.

Common paths:

1. Search object -> choose object -> choose action
2. No query yet -> Create object
3. No query yet -> Set API token
4. Choose structure -> enter name -> press Enter on empty content to create a blank object

For text entry, use escaped newlines with \n when needed.

## Settings

1. Capacities API Token
2. After Create Object
   - Open Capacities App
   - Open Capacities Web
   - Do nothing

## Roadmap summary

### Version 1.0

1. Ship current search/open/append/create scope
2. Publish to winget
3. Submit to CmdPal gallery (winget install source)

### Version 1.1

1. Post-release stabilization and issue fixes
2. Icon mapping and visual contrast refinement
3. Listing and install polish

### Version 1.2

1. Publish to Microsoft Store

## Author

**Vector Rilke**

- GitHub: [@vectorrilke](https://github.com/vectorrilke)
- Project: [cp_capabilities_extension](https://github.com/vectorrilke/cp_capabilities_extension)

## License

MIT License - See LICENSE file for details

## Screenshots

For screenshots used in store listings and gallery submissions, see `docs/gallery/SCREENSHOTS.md`.

## Publishing

### WinGet (Primary Distribution)

The extension is published to WinGet with automatic updates via GitHub Actions:

```powershell
winget install VectorRilke.CapacitiesCommandPaletteExtension
winget upgrade CapacitiesCommandPaletteExtension
```

Repository: [microsoft/winget-pkgs](https://github.com/microsoft/winget-pkgs)

### Microsoft Store (Secondary Distribution)

Available in Microsoft Store for broader reach:

- Microsoft Store listing: [Capacities Command Palette Extension](https://www.microsoft.com/store/apps)
- Partner Center: [partner.microsoft.com](https://partner.microsoft.com/dashboard)

### Command Palette Extension Gallery

Listed in the Extension Gallery for discovery within Command Palette:

- Gallery: [microsoft/CmdPal-Extensions](https://github.com/microsoft/CmdPal-Extensions)
- Search "Capacities" in Command Palette settings → Extensions → Gallery

## Development & Contributing

### Build

```powershell
dotnet build .\src\CapacitiesCommandPaletteExtension\CapacitiesCommandPaletteExtension.sln
```

### Deploy & Test

1. Deploy from Visual Studio
2. Run "Reload Command Palette Extension" in Command Palette
3. Test all flows

### Requirements

- PowerToys Command Palette enabled
- .NET 10.0 or later
- Windows 10.0.26100.0 or later

### Version 2.0

1. Expanded create flows (Tasks, Weblinks, Daily Notes, media, PDFs)
2. Tags for new objects
3. Richer metadata-driven icon mapping

## Public docs

1. [docs/capacities-api-details.md](docs/capacities-api-details.md)
2. [docs/publishing-v1-roadmap.md](docs/publishing-v1-roadmap.md)
