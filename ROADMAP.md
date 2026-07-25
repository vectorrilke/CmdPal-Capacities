# Roadmap

Planned features and releases for Capacities Command Palette Extension.

## Version 0.1.0 (Current - Release Candidate)

**Status:** ✅ Available via WinGet (PR #407721 submitted)

### Features Delivered
- ✅ Search existing objects
- ✅ Open objects in Capacities App
- ✅ Open objects in Capacities Web
- ✅ Append markdown content to objects
- ✅ Create new objects with markdown content
- ✅ API token management with secure storage
- ✅ Rate-limit protection with caching
- ✅ Dual-architecture support (x64, ARM64)

### Known Limitations
See [API_LIMITATIONS.md](API_LIMITATIONS.md) for details on:
- Why Tasks and Daily Notes aren't supported yet
- Rate-limiting strategy
- Structure exclusions and reasoning

---

## Version 1.0 (Q3 2026)

**Goal:** Stabilize and expand distribution

### Planned Features
- 📋 CmdPal Gallery submission
- 📋 Enhanced error messages and user guidance
- 📋 Performance optimizations
- 📋 Extended documentation and tutorials
- 📋 Community issue triage

### Expected Release
- Late Q3 2026
- Dependent on Capacities API stabilization

---

## Version 1.1 (Q4 2026)

**Goal:** Improve user experience and visual polish

### Planned Features
- 🎨 Icon mapping for structure types (List, Page, etc.)
- 🎨 Visual contrast and theme refinements
- ⚡ Performance improvements for large workspaces
- 📝 Better search result previews
- 🔧 Settings UI enhancements
- 🐛 Stability and bug fixes from v1.0

### Under Consideration
- Custom hotkeys for common actions
- Search filters by structure type
- Recent objects quick access
- Workspace switching (if multiple API tokens)

---

## Version 1.2 (Q1 2027)

**Goal:** Expand platform availability

### Planned Features
- 🏪 Microsoft Store submission
- 🔄 Auto-update mechanism (via WinGet)
- 📱 Support for future CmdPal mobile/cloud versions
- 🌐 Localization preparation

### Requirements
- Stable marketplace infrastructure
- Auto-update backend (GitHub releases or custom)

---

## Version 1.3+ (2027+)

**Goal:** Advanced workflows and richer integration

### Under Consideration
- **Task Management**
  - Create tasks with due dates and priority
  - Link tasks to projects
  - Task templates for common workflows
  - Dependent on: Capacities Task API enhancements

- **Daily Notes Integration**
  - Quick capture of daily entries
  - Linking to other objects
  - Date navigation
  - Dependent on: Capacities Daily Notes API stabilization

- **Media Object Support**
  - Image linking and previews
  - PDF management
  - Weblink creation
  - File upload capability
  - Dependent on: Media API availability

- **Advanced Features**
  - Tag assignment on creation
  - Relationship visualization
  - Batch operations
  - Custom structure templates
  - Smart suggestions based on usage

- **Developer Experience**
  - API documentation for extensions
  - CmdPal extension SDK improvements
  - Plugin system for Capacities extension

---

## Dependency Tracking

### External Dependencies

**Capacities API**
- Search endpoint: ✅ Stable
- Structure metadata: ✅ Stable
- Object creation: ✅ Mostly stable
- Task API: ⏳ Evolving
- Daily Notes API: ⏳ Evolving
- Media handling: ⏳ Planning phase

**Windows PowerToys CmdPal**
- Extension framework: ✅ Stable
- UI components: ✅ Stable
- Future mobile/cloud: 📋 Planned

**Distribution Platforms**
- WinGet: ✅ Active
- CmdPal Gallery: 📋 Planned
- Microsoft Store: 📋 Planning

---

## Feedback & Voting

### How to Influence the Roadmap

1. **Vote on Features**
   - GitHub Discussions: [Feature Requests](https://github.com/vectorrilke/CmdPal-Capacities/discussions/categories/feature-requests)
   - React with 👍 to show interest

2. **Report Issues**
   - GitHub Issues: [Report a Bug](https://github.com/vectorrilke/CmdPal-Capacities/issues)
   - Include steps to reproduce and expected behavior

3. **Start a Discussion**
   - Ask questions about planned features
   - Suggest use cases
   - Share workflow ideas

4. **Contribute**
   - Pull requests welcome
   - See [Contributing Guidelines](README.md#contributing)

---

## Release Schedule

| Version | Status | Planned Release |
|---------|--------|-----------------|
| 0.1.0 | 🟢 RC | Now (WinGet PR #407721) |
| 1.0 | 🟡 Planning | Q3 2026 |
| 1.1 | 🔵 Backlog | Q4 2026 |
| 1.2 | 🔵 Backlog | Q1 2027 |
| 1.3+ | 🔵 Future | TBD |

**Legend:**
- 🟢 Released / Release Candidate
- 🟡 In Progress / Planning
- 🔵 Planned / Backlog

---

## Breaking Changes Policy

Starting with v1.0, we follow semantic versioning:
- **Major** (1.0 → 2.0): Breaking API/UI changes
- **Minor** (1.0 → 1.1): New features, backward compatible
- **Patch** (1.0 → 1.0.1): Bug fixes, backward compatible

We will provide:
- Migration guides for breaking changes
- Deprecation warnings in minor releases before removal
- At least 2 releases notice before removing features

---

## Architecture & Technical Notes

### Build System
- PowerShell 7+ orchestration
- .NET 10 (net10.0-windows10.0.26100.0)
- Inno Setup 6 for installer generation
- GitHub Actions for CI/CD

### Supported Platforms
- **Windows 10.0.26100.0+** (requires Windows 11)
- **Architectures:** x64, ARM64
- **Runtime:** .NET 10.0

### Performance Targets
- Search response: < 500ms (95th percentile)
- Create object: < 1s (95th percentile)
- Append text: < 500ms (95th percentile)
- Memory footprint: < 50MB resident

---

## Questions?

- 📧 Email: vectorrilke@pm.me
- 🐦 Twitter: [@vectorrilke](https://twitter.com/vectorrilke)
- 💬 Discussions: [GitHub Discussions](https://github.com/vectorrilke/CmdPal-Capacities/discussions)
- 🐛 Issues: [GitHub Issues](https://github.com/vectorrilke/CmdPal-Capacities/issues)

---

**Last Updated:** July 25, 2026
**Maintained By:** Vector Rilke

*This roadmap is subject to change based on user feedback, API availability, and technical constraints.*
