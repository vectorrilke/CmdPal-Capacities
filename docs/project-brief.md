# Capacities Command Palette Extension - Project Brief

## Goal
Build a PowerToys Command Palette extension that lets users search Capacities objects, open them, append content, and create new objects directly from Windows Command Palette.

## Confirmed behavior (current)

1. Input format
   - Support both text-only and text+URL (Option C).
   - Example:
     - `cap Web about css, css-tricks.com`
     - `cap Remember to review CSS layouts`
   - Support escaped newlines with `\n`.

2. Insert position
   - Extension writes with append semantics.
   - Prepend option is intentionally removed from settings UX.

3. Search-first object workflow
   - User enters API auth in settings.
   - User invokes the extension alias and types search text.
   - Extension calls `POST https://api.capacities.io/objects/search` with a query and limit, for example:

       ```json
       {
          "query": "CmdPal Test",
          "limit": 20
       }
       ```

   - User selects the matching object from search results.
   - Extension offers open or append-then-open actions for that object.

4. Content output mode
   - Markdown only.

5. Create object flow
   - User chooses/searches structure.
   - User enters object name.
   - User enters object content.
   - Extension creates object.
   - Extension can then open web, open app, or do nothing based on setting.

## Current settings

1. Capacities API auth token
2. After Create Object: open web, open app, or do nothing

Note:
1. Alias is configured in Command Palette's extension-level alias setting.
2. This extension no longer duplicates alias as its own custom setting.
3. The visible token value is masked while the full token is stored internally.

## Current UX shape

When user invokes the extension alias (for example `cap`):
1. Typing 3 or more characters starts object search immediately.
2. Without a search query, the idle screen exposes:
   - Create new object
   - Set API token or Token set

Current behavior for create new object:
1. User chooses structure.
2. User enters name.
3. User enters content.
4. Object is created and can then open in app, web, or do nothing.

## Command handling

1. User types object title text via Command Palette alias.
2. Extension searches Capacities objects.
3. User chooses an object action.
4. If appending, extension parses the entered text and builds markdown content.
5. Extension sends request to Capacities API.
6. Extension opens the object or shows success/failure feedback.

## Decision: Version 0.9 interaction model

- Version 0.9 is search-first, not target-first.
- The extension no longer relies on a persistent selected target object.
- Object actions are chosen per search result.

## Future enhancement

1. Browse Capacities structure from API via `GET /space/structures` and display top object categories (`structures[].title`) in settings.
2. Add richer structure-driven icon support in CmdPal lists.
3. Implement create flows for Tasks, Weblinks, Daily Notes, media, and PDFs.
4. Set tags for new objects.
5. Improve structure-specific creation behavior as the API evolves.
