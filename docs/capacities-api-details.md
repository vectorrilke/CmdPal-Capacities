# Capacities API Details

This file tracks how the extension currently uses the Capacities API, what is implemented, and what limits still exist.

## Implemented endpoints

1. POST /objects/search
   - searches objects by query text
   - used for the primary object search flow in Command Palette
2. GET /space/structures
   - loads visible structures
   - used to filter supported structure types and label grouped search results
3. GET /object?id=<uuid>
   - used as a follow-up detail lookup when search results do not include enough description or structure information
4. POST /blocks/append
   - appends markdown content to a selected object
5. POST /object/markdown
   - creates a new object with markdown content
6. GET /space
   - resolves space id needed for web URLs and app deep links

## Search behavior

Current search flow:

1. User types 3 or more characters.
2. The extension waits briefly before searching to reduce rate-limit pressure.
3. The extension calls POST /objects/search.
4. Search results are grouped by structure.
5. When search responses are missing useful metadata, the extension may call GET /object?id=<uuid> for selected top results.
6. Structure names and descriptions are resolved from GET /space/structures.
7. The extension explicitly discards these structure IDs from selectable/searchable structure results because they represent internal or unsupported Capacities objects:
   - RootQuery
   - MediaImage
   - RootDailyNote
   - RootAIChat
   - RootTask
   - MediaWebResource
   - RootSimpleTable
   - RootTag
   - MediaPDF

Current protections against 429 responses:

1. Minimum query length
2. Debounced search
3. Structure caching
4. Object-detail caching
5. Per-search limit for detail enrichment
6. Cooldown when the API returns 429

## Supported content model in Version 0.9

Supported now:

1. Search existing objects
2. Open existing objects
3. Append markdown text to existing objects
4. Create generic new objects through markdown content, including blank objects with no body text

Not yet implemented as stable user-facing flows:

1. Tasks
2. Weblinks
3. Daily Notes
4. Media objects
5. PDFs
6. Tags on new objects

## Why some features are deferred

The API is still in development and does not yet provide a stable, high-confidence path for every richer object workflow the extension eventually wants to support.

Examples:

1. Some structures require more metadata than current search responses provide.
2. Some object-specific creation flows need more explicit API affordances.
3. Some user-facing behaviors would be unreliable without additional validation or richer server responses.

## Practical release note for Version 1.0

The first public release should present the extension as:

1. a search-first Capacities companion for Command Palette
2. a quick way to open existing objects
3. a quick way to append text and then open
4. a lightweight create-object workflow

It should not promise richer object-type creation until those paths are tested and stable.