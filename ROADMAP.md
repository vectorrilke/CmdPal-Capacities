# Roadmap

## Where We Are

**Version 0.1.0** — Release Candidate

The core extension is complete and working. WinGet package submission is in review (PR #407721 in microsoft/winget-pkgs). Once merged, users will be able to install via `winget install VectorRilke.CmdPal-Capacities`.

### What works today
- Search existing objects by text
- Open objects in Capacities App or Web
- Append markdown content to objects
- Create new objects with markdown content
- Secure API token storage
- Rate-limit protection with caching and debouncing
- Dual-architecture installers (x64, ARM64)

### What doesn't work yet
Some object types (Tasks, Daily Notes, media) are excluded. This is intentional — the Capacities API for these is still evolving. See [API_LIMITATIONS.md](API_LIMITATIONS.md) for the full reasoning.

---

## What's Next

### Step 1 — WinGet goes live
- Wait for PR #407721 to be reviewed and merged by the WinGet team (typically a few business days)
- Once merged: the package is live and discoverable via `winget install`

### Step 2 — Automate WinGet updates
Right now, every new version requires a manual PR to microsoft/winget-pkgs with updated installer hashes. The next infrastructure task is to automate this: a GitHub Actions workflow that triggers on a new release tag, builds the installers, and submits the WinGet PR automatically.

This means: tag a release → installers built → WinGet PR opened automatically.

### Step 3 — CmdPal Extension Gallery
Once the WinGet package is live, submit to the Command Palette Extension Gallery (microsoft/CmdPal-Extensions). This makes the extension discoverable directly inside Command Palette via the built-in Browse Extensions feature.

### Step 4 — Microsoft Store
A separate submission path for broader reach. Requires a Partner Center account and a packaged MSIX build. Planned after the WinGet and Gallery distribution channels are stable.

---

## Version 1.1 — UX Polish

After initial distribution is stable:

- Icon mapping for structure types (List, Page, etc.)
- Better search result previews
- Settings UI improvements
- Visual and contrast refinements
- Bug fixes from community feedback

---

## Future — API-Dependent Features

The following features are planned but depend on the Capacities API evolving to support them reliably.

**Monitoring the Capacities API is the key input here.** Specifically:

- `structureId` descriptions becoming available in API responses — would allow richer display and filtering without manual mapping
- Stable endpoints for Tasks, Daily Notes, and media types

When the API supports it:

- **Task management** — create tasks with due dates, priority, project linking
- **Daily Notes** — quick capture of daily entries
- **Media objects** — weblinks, images, PDFs
- **Tags** — tag assignment when creating objects
- **Structure descriptions** — display human-readable labels instead of raw type names

---

## Release Status

| Version | Status |
|---------|--------|
| 0.1.0 | RC — WinGet PR in review |
| 1.0 | After WinGet + Gallery distribution confirmed |
| 1.1 | UX polish, post-release fixes |
| 1.2 | Microsoft Store |
| 1.3+ | API-dependent features |

---

## Feedback

- [Issues & Bug Reports](https://github.com/vectorrilke/CmdPal-Capacities/issues)
- [Discussions](https://github.com/vectorrilke/CmdPal-Capacities/discussions)

---

*Last updated: July 25, 2026*
