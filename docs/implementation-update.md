# Implementation Update

## Version status

1. Current baseline is Version 0.9.
2. Version 1.0 scope is stabilization, deploy validation, and publish readiness.
3. Version 2.0 scope is richer creation flows and metadata-driven polish.

## Completed

1. Real Capacities write integration is implemented.
2. Output mode is markdown only.
3. Escaped newline input is supported with \n.
4. API token saving is implemented with masked visible display and hidden storage.
5. Structure loading and filtering are implemented for selectable object types.
6. Search-first object workflow is implemented directly from the top-level extension page.
7. Object results are grouped by structure with descriptions when available.
8. Create-object flow is implemented:
	- choose or search structure
	- enter object name
	- enter content
	- create object
9. Post-create behavior setting is implemented:
	- open Capacities web
	- open Capacities app
	- do nothing
10. Open existing object actions are implemented:
	- open in app
	- open in web
11. Append then open actions are implemented:
	- append then open in app
	- append then open in web
12. Debounce, caching, and 429 cooldown logic are in place for search/detail calls.

## Current behavior summary

1. Main CmdPal behavior is now search-first.
2. Typing 3 or more characters triggers object search.
3. Search uses POST /objects/search with filtered structures from GET /space/structures.
4. Selecting an object opens a second page with open or append-then-open actions.
5. Object creation uses POST /object/markdown.
6. Web open uses URL format with space ID from GET /space and object ID.

## Known constraints

1. Some structure types are intentionally excluded from selection due to API support limits.
2. Some descriptions still require follow-up detail calls because search responses are incomplete.
3. Richer create flows remain constrained by current Capacities API coverage.

## Version 1.0 immediate steps

1. Deploy and reload the current 0.9 build.
2. Execute manual smoke tests for token, search, open, append, and create flows.
3. Finalize screenshots, publish copy, and package metadata.
4. Confirm first public release scope and lock Version 1.0.

## Version 2.0 planned steps

1. Implement create flows for Tasks, Weblinks, and Daily Notes.
2. Add media and PDF-oriented creation paths where the API allows them.
3. Add Set tags for new objects.
4. Implement advanced icon mapping based on structure metadata.

## Next step

1. Deploy from Visual Studio.
2. Run Reload Command Palette Extension.
3. Execute full smoke test of:
	- save and clear API token
	- search results and grouped structures
	- open in app and web
	- append then open in app and web
	- create object flow with each post-create behavior option.
4. Prepare Version 1.0 publish assets and submission checklist after smoke test sign-off.