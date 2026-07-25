# CmdPal Capacities Extension

**Capacities integration for Windows Command Palette (PowerToys CmdPal)**

Search objects, open them, append text, and create new objects without leaving Command Palette.

![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)
![Version: 0.1.0](https://img.shields.io/badge/Version-0.1.0-blue.svg)
![Status: Release Candidate](https://img.shields.io/badge/Status-Release%20Candidate-green.svg)

## Quick Start

### Installation

```powershell
# Via WinGet
winget install VectorRilke.CmdPal-Capacities

# Manual: Download from GitHub Releases
# https://github.com/vectorrilke/CmdPal-Capacities/releases
```

### Usage

1. Open Command Palette (PowerToys CmdPal)
2. Type the extension alias (default: `cap`)
3. Type at least 3 characters to search your Capacities objects
4. Select an object or action

## Screenshots

Search, create, and append objects without leaving Command Palette:

![Search Capacities objects](docs/gallery/screenshots/1-search-objects.png)
![Create new objects](docs/gallery/screenshots/2-create-object.png)
![Enter content](docs/gallery/screenshots/3-create-content.png)

## Features

### Search & Access
- 🔍 Search-first object lookup from Command Palette
- 📊 Results grouped by structure type (List, Page, etc.)
- ⚡ Smart caching and debouncing for performance
- 🔄 Transient error fallback behavior

### Object Actions
- Open in Capacities App
- Open in Capacities Web
- Append markdown text then open App
- Append markdown text then open Web
- Append text only (without opening)

### Object Creation
- Choose structure type
- Enter object name
- Enter markdown content or create blank object
- Post-create behavior: Open App, Open Web, or do nothing

### Authentication & Security
- 🔒 API token stored securely (outside visible settings)
- 🔐 Masked token display in UI
- ✅ Token validation before operations

### Search Resilience
- Minimum query length (3 characters)
- Request debouncing
- Response caching
- Rate-limit protection with automatic cooldown
- Graceful error handling

## Usage

Invoke the extension alias (for example: `cap`) and type at least 3 characters to search.

**Common workflows:**

1. **Search & Open**: Search object → choose object → choose action
2. **Create Object**: No query → Create object → enter details
3. **Set API Token**: No query → Set API token
4. **Create Blank**: Choose structure → enter name → press Enter (empty content)

For text entry, use escaped newlines with `\n` when needed.

## Current Limitations

The extension focuses on **search, open, append, and create** workflows for standard objects. Some advanced features like Tasks, Daily Notes, and media objects are not yet fully supported.

**Why?** The Capacities API is still evolving, and some object types require more stable API affordances. See [**API_LIMITATIONS.md**](API_LIMITATIONS.md) for detailed reasoning and future plans.

## Settings

| Setting | Options | Default |
|---------|---------|---------|
| Capacities API Token | Your API token from Capacities | (required) |
| After Create Object | Open App / Open Web / Do nothing | Open App |

## Roadmap

See the [**ROADMAP.md**](ROADMAP.md) file for a detailed release plan.

**Quick Summary:**

| Version | Status | Timeline |
|---------|--------|----------|
| 0.1.0 | ✅ RC | Available now |
| 1.0 | 📋 Planning | Q3 2026 |
| 1.1 | 📋 Planned | Q4 2026 |
| 1.2 | 📋 Planned | Q1 2027 |
| 1.3+ | 🔮 Future | 2027+ |

**Current Focus (v0.1.0):**
- ✅ Search and open objects
- ✅ Append markdown content
- ✅ Create new objects
- 📋 WinGet submission (PR #407721)

**Next Steps (v1.0+):**
- Stabilization and community feedback
- CmdPal gallery submission
- Microsoft Store (v1.2)
- Task/Daily Notes support (v1.3+)

## Getting Your API Token

1. Go to https://capacities.io/settings/developer
2. Generate an API token
3. Copy the token
4. In Command Palette, use the extension's token setting command to save it

## Security Note

Your API token is stored in the Windows Registry outside of user-visible settings. It is never:
- Displayed in plain text in settings
- Logged to disk
- Transmitted to any server other than Capacities' official API

## Project Structure

```
src/CapacitiesCommandPaletteExtension/
├── CapacitiesCommandPaletteExtension.cs          # Main extension class
├── Capacities/                                   # API client
│   ├── CapacitiesClient.cs
│   ├── CapacitiesContentComposer.cs
├── Commands/                                     # Command implementations
│   ├── OpenCapacitiesObjectCommand.cs
│   ├── CreateObjectCommand.cs
│   ├── AppendToObjectAndOpenCommand.cs
│   └── ...
├── Pages/                                        # UI pages
│   ├── CapacitiesCommandPaletteExtensionPage.cs
│   ├── CreateObjectEnterNamePage.cs
│   └── ...
├── Parsing/                                      # Input parsing
│   └── CapCommandParser.cs
└── Settings/                                     # Configuration
    └── ExtensionSettings.cs
```

## Development

### Requirements
- .NET 10.0 SDK or later
- Windows 10.0.26100.0 or later
- PowerToys Command Palette installed

### Build

```powershell
cd src/CapacitiesCommandPaletteExtension
dotnet build CapacitiesCommandPaletteExtension.csproj
```

### Create Installer

```powershell
# Requires Inno Setup 6
.\build-exe.ps1 -Platforms @("x64", "arm64") -Version "0.1.0.0"
```

## Contributing

Contributions are welcome! Please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

## Support & Feedback

- 📝 [Issues & Bug Reports](https://github.com/vectorrilke/CmdPal-Capacities/issues)
- 💬 [Discussions](https://github.com/vectorrilke/CmdPal-Capacities/discussions)
- 🐦 [@vectorrilke](https://twitter.com/vectorrilke)

## License

MIT License - see [LICENSE](LICENSE) for details

## Credits

- Built for **Windows Command Palette (PowerToys CmdPal)**
- Integrates with **Capacities** workspace platform
- Inspired by PowerToys community extensions

## Author

**Vector Rilke**  
[GitHub](https://github.com/vectorrilke) | [Twitter](https://twitter.com/vectorrilke)

---

**Have feedback?** Open an issue or start a discussion! 🚀
