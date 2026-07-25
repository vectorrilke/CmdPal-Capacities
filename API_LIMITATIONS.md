# API Limitations & Design Decisions

This document explains the current scope of the Capacities Command Palette extension, why certain features are deferred, and the technical reasoning behind these decisions.

## Overview

**Version 0.1.0** focuses on core workflows:
- ✅ Search existing objects
- ✅ Open objects in App/Web
- ✅ Append markdown to objects
- ✅ Create new objects with markdown content

**Not yet implemented:**
- ❌ Task management workflows
- ❌ Daily Notes integration
- ❌ Media object handling
- ❌ PDF management
- ❌ Tag assignment
- ❌ Weblink creation

## Why Some Features Are Deferred

### 1. API Maturity & Stability

The Capacities API is actively evolving, and not all object types have stable endpoints for creation and manipulation yet.

**Example:** Tasks require specific metadata fields, relationships, and validation that the API doesn't yet expose in a reliable way. Creating a Task through the extension could fail silently or produce unexpected results.

**Solution:** Wait for API enhancements, then add stable Task workflows in Version 1.3+.

### 2. Missing Metadata in Search Responses

Some search responses don't include enough information to reliably identify object properties, which can lead to:
- Incorrect user guidance
- Failed operations
- Confusing UX

**Example:** A search result for a Task might not include:
- Priority level
- Due date
- Assigned project
- Dependency information

**Solution:** The extension calls detail-lookup endpoints (`GET /object?id=<uuid>`) when needed, but this adds latency. Rate-limiting and caching help, but the UX still isn't ideal.

### 3. Rate Limiting & Performance

The Capacities API enforces rate limits (typically 429 responses). Each workflow could require multiple API calls:

```
Search (1 call) → Detail lookup (1+ calls) → Create/Update (1 call) → Verify (1 call)
= 3-5 API calls per workflow
```

For advanced object types with complex validation, this could easily exceed safe thresholds.

**Solution:** Start with simple workflows that need fewer calls, optimize caching, and expand carefully.

### 4. User Safety & Data Integrity

Some object types have strict validation requirements. If the extension fails to meet them, it could:
- Create malformed objects
- Corrupt relationships
- Break user workflows

**Example:** A Daily Note must be linked to the correct date structure. If the extension creates it with the wrong linkage, it becomes inaccessible or creates duplicates.

**Solution:** Only implement workflows where we can guarantee data integrity. Get proper API affordances first.

## Currently Excluded Structures

These structures are explicitly excluded from search and creation:

| Structure | Reason |
|-----------|--------|
| `RootQuery` | Internal system structure |
| `RootDailyNote` | Root entry point (not user-creatable) |
| `RootTask` | Root entry point (not user-creatable) |
| `RootAIChat` | Internal AI feature structure |
| `MediaImage` | Media objects (different creation model) |
| `MediaWebResource` | Weblinks (deferred) |
| `MediaPDF` | PDF management (deferred) |
| `RootSimpleTable` | Tables (deferred - complex structure) |
| `RootTag` | Tags (deferred - relationship model) |

## API Endpoints Used

### Implemented

1. **POST /objects/search**
   - Searches objects by query text
   - Primary flow: object discovery
   - Rate limit: ~100-150 requests per minute

2. **GET /space/structures**
   - Lists all structures in the workspace
   - Used for: filtering, labeling, UI display
   - Cached locally

3. **GET /object?id=<uuid>**
   - Fetches detailed object metadata
   - Used for: enriching search results
   - Called sparingly with cooldown

4. **POST /blocks/append**
   - Appends markdown content to an object
   - Used for: text append workflow
   - Requires valid object ID

5. **POST /object/markdown**
   - Creates new object with markdown content
   - Used for: object creation
   - Validation handled server-side

6. **GET /space**
   - Resolves space ID and metadata
   - Used for: generating app/web URLs

### Not Yet Used (Deferred)

These endpoints exist but aren't used in v0.1.0:

1. **POST /task** - Task creation with specific fields
2. **POST /weblink** - Weblink creation
3. **POST /dailynote** - Daily Note creation
4. **GET /tags** - Tag management
5. **PATCH /object** - Object updates (field-level)
6. **DELETE /object** - Object deletion

## Rate Limiting Strategy

The extension implements multiple layers of protection:

### Layer 1: Minimum Query Length
- Requires 3+ characters before searching
- Reduces accidental high-volume requests

### Layer 2: Request Debouncing
- Waits ~500ms before searching
- Merges rapid successive keypresses
- Reduces duplicate requests from user typing

### Layer 3: Response Caching
- Caches structure data (1 hour)
- Caches object details (10 minutes)
- Reuses results for repeated queries

### Layer 4: Detail-Lookup Limiting
- Only enriches top 3-5 results
- Skips detail lookups if search already provided enough info
- Stops enrichment if approaching rate limits

### Layer 5: Cooldown & Backoff
- Detects 429 responses
- Implements exponential backoff
- Displays user message: "Service temporarily busy, please try again"

## Future Work (Roadmap)

### Version 1.1 (Q3 2026)
- ✅ Stabilization of core workflows
- ✅ Better error messaging
- 🔄 Performance optimization

### Version 1.2 (Q4 2026)
- 📋 Task creation workflow (if API ready)
- 📋 Daily Notes integration (if API ready)
- 📋 Better structure type discovery

### Version 1.3+ (2027)
- 📋 Media object support (Images, PDFs, Weblinks)
- 📋 Tag assignment on creation
- 📋 Advanced filtering and sorting
- 📋 Relationship visualization

## Communicating Limitations to Users

### In-App Messaging
When a user tries to create an unsupported structure type:
```
⚠️ Creating Tasks is coming soon!

For now, create a Task in Capacities, then search and append here.
```

### Documentation
- README explains current scope
- In-app help describes available actions
- GitHub issues document feature requests

### Feedback Channels
- Users can vote on features in GitHub Discussions
- Issues are prioritized by community feedback
- Roadmap updates based on API availability

## Capacities API Contact

For questions about API stability or new endpoints:
- 📧 api@capacities.io
- 🐛 GitHub Issues (Capacities repository)
- 💬 Capacities Community (Slack/Discord)

## Related Resources

- **[Capacities API Docs](https://capacities.io/api)** - Official API reference
- **[PowerToys CmdPal Docs](https://learn.microsoft.com/en-us/windows/powertoys/cmdpal)** - Extension development guide
- **[Extension Development](https://learn.microsoft.com/en-us/windows/powertoys/cmdpal/creating-an-extension)** - Creating CmdPal extensions

---

**Last Updated:** July 25, 2026
**Extension Version:** 0.1.0
**API Version:** v1 (stable)

Have questions about limitations? [Open an issue](https://github.com/vectorrilke/CmdPal-Capacities/issues) or start a [discussion](https://github.com/vectorrilke/CmdPal-Capacities/discussions)!
