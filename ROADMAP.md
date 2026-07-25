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

## Version 1.0

**Goal:** Stabilize and expand distribution

### Planned Features
- 📋 CmdPal Gallery submission
- 📋 Enhanced error messages and user guidance
- 📋 Performance optimizations
- 📋 Extended documentation and tutorials
- 📋 Community issue triage

---

## Version 1.1

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

## Version 1.2

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

## Version 1.3+

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

## Feedback & Voting

Want to influence the roadmap? Here's how:

- **Vote on Features**: React with 👍 on [GitHub Discussions](https://github.com/vectorrilke/CmdPal-Capacities/discussions)
- **Report Issues**: [GitHub Issues](https://github.com/vectorrilke/CmdPal-Capacities/issues)
- **Start a Discussion**: Share use cases and workflow ideas

---

## Release Status

| Version | Status |
|---------|--------|
| 0.1.0 | 🟢 Released / Release Candidate |
| 1.0 | 🟡 In Progress / Planning |
| 1.1 | 🔵 Planned / Backlog |
| 1.2 | 🔵 Planned / Backlog |
| 1.3+ | 🔵 Future |

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

## Questions?

- 📧 Email: vectorrilke@pm.me
- 🐦 Twitter: [@vectorrilke](https://twitter.com/vectorrilke)
- 💬 Discussions: [GitHub Discussions](https://github.com/vectorrilke/CmdPal-Capacities/discussions)
- 🐛 Issues: [GitHub Issues](https://github.com/vectorrilke/CmdPal-Capacities/issues)

---

**Last Updated:** July 25, 2026
**Maintained By:** Vector Rilke

*This roadmap is subject to change based on user feedback, API availability, and technical constraints.*
