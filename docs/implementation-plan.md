# Implementation Plan and MVP Checklist

This checklist converts the agreed project brief into concrete work items.

## Version roadmap

### Version 0.9 (current)

1. Search-first object flow from the top-level extension page.
2. Open matched object in app or web.
3. Append text then open matched object.
4. Create new object flow:
	- choose/search structure
	- enter name
	- enter content
	- create and optionally open
5. Masked token display with hidden token storage.
6. Debounced search with caching and 429 cooldown behavior.

### Version 1.0 (release)

1. Deploy and validate Version 0.9 behavior end to end.
2. Freeze public release scope around search, open, append-then-open, and create object.
3. Finalize documentation, screenshots, and packaging metadata.
4. Publish to winget.
5. Prepare and submit CmdPal gallery entry using the winget install source.

### Version 1.1 (stabilization)

1. Fix post-release issues from real-world usage.
2. Improve installer, screenshots, and listing polish as needed.
3. Refine icon mapping and visual contrast across structure, object, create, and token rows.
4. Keep the Version 1.0 feature scope stable while hardening the release.

### Version 1.2 (distribution expansion)

1. Publish to Microsoft Store.
2. Complete Store listing, screenshots, and certification work.
3. Decide whether to keep the gallery pointed at winget or later switch preference to Store.

### Version 2.0 (next)

1. Implement create flows for:
	- Tasks
	- Weblinks
	- Daily Notes
	- Media
	- PDFs
2. Set tags for new objects.
3. Advanced icons:
	- read icon info from structure metadata and apply richer icon mapping.

## Current execution order (agreed)

1. Keep current UX polish as-is unless deploy or test uncovers a concrete issue.
2. Deploy + reload + test current 0.9 behavior.
3. Prepare README, screenshots, winget packaging metadata, and gallery submission metadata.
4. Lock Version 1.0 after smoke-test sign-off.
5. Version 1.1 starts after initial public release.
6. Version 1.2 adds Microsoft Store distribution.

## 1) Scaffold and baseline

1. Create extension scaffold from PowerToys Command Palette `Create a new extension`.
2. Place scaffold under `src/<ExtensionName>/` in this repo.
3. Confirm baseline deploy works:
	- Deploy from Visual Studio.
	- Run `Reload Command Palette Extension`.
	- Open extension and verify default TODO item appears.

Done when:
- Project deploys successfully and appears in Command Palette.

## 2) Command and settings model (MVP)

Implement these settings with persistent storage:
1. `ApiToken` (Capacities auth token)
2. `TargetObjectTitle` (display name of selected target object)
3. `TargetObjectId` (selected target UUID, hidden from visible settings)
4. `CreateObjectOpenBehavior` (`web`, `app`, or `none`)
5. `OutputMode` (`markdown` only)

Done when:
- Settings can be changed and survive reload/restart.

## 3) Input parsing (Option C)

Support both:
1. Text only
2. Text + URL (comma-separated, first comma split)

Examples:
1. `cap Remember to review CSS layouts`
2. `cap Web about css, css-tricks.com`

Parsing rules:
1. If comma is present, left side is text and right side is optional URL.
2. Trim whitespace on both parts.
3. Empty text is invalid and should return a user-facing error.

Done when:
- Parser returns structured payload with `Text` and optional `Url`.

## 4) Capacities API integration

1. Add typed HTTP client wrapper for Capacities API.
2. Implement target object search using `GET https://api.capacities.io/objects/search` with request data like:

	```json
	{
		"query": "CmdPal Test",
		"limit": 20
	}
	```

3. Store the selected search result UUID as `TargetObjectId`.
4. Implement append write operation to configured `TargetObjectId`.
5. Add auth handling with `ApiToken`.
6. Add robust error mapping for:
	- Auth failure
	- Target object not found
	- No object search results
	- Validation error
	- Network failure

Done when:
- Settings search can resolve an object UUID and one successful API call adds a block to that configured object.

Additional implemented API operations:
1. Create object with `POST /object/markdown`.
2. Resolve space id with `GET /space` for object web URL open behavior.

## 5) Content rendering strategy

1. `markdown` mode only:
	- Send markdown content to Capacities append endpoint.
	- Plain text input is valid markdown and remains supported through the same path.

Input note:
1. In CmdPal query text, use `\\n` for new line.
2. Practical pattern: `text \\n text`.

Done when:
- Same parsed input is sent through markdown output consistently.

## 6) Command Palette UX flow

1. Top-level extension entry provides multiple actions:
	- Send text (primary)
	- Save API token
	- Choose target
2. Send text action opens the quick input flow for payload text.
3. Execute action writes to Capacities and shows status feedback.

Done when:
- User can complete full flow from Command Palette without leaving it.

## 7) Validation and diagnostics

1. Validate required settings before API call.
2. Validate target object search before saving `TargetObjectId`.
3. Show actionable errors (missing token, missing target object, no matching object, etc.).
4. Add basic debug logging for command invocation and API results.

Done when:
- Common setup mistakes produce clear guidance.

## 8) Post-MVP enhancements

1. Fetch Capacities structure using `GET /space/structures` and show top object categories (`structures[].title`) in settings.
2. Let user choose a target category (for example, Page, Note, Food).
3. Filter `GET /objects/search` by selected structure if the API supports it.
4. Add richer autocomplete for object name lookup results in settings.
5. Optional per-command target override in command text.
6. Show an icon for the selected target object in Command Palette lists (use object type/category icon when available; fallback to a default Capacities target icon).
7. Add a dedicated prepend UX review:
	- Keep append as default.
	- Offer a quick toggle for prepend at send time.
	- Evaluate whether prepend should be sticky (saved setting) or one-shot per command.

## 9) Candidate workflow: create new object from Command Palette

Goal:
1. Allow creating a new object without leaving Command Palette, then immediately writing content to it.

Proposed flow:
1. User selects `Choose target` -> `Create new object`.
2. User selects a `sectionId` category from allowed structures.
3. User enters the new object name/title.
4. User enters object content.
5. Extension calls Capacities create-object endpoint with selected `sectionId`, title, and content.
6. Extension stores created object id/title as current target.
7. Extension applies configured post-create behavior: open web, open app, or do nothing.

Design notes:
1. Validation:
	- Require non-empty title.
	- Handle duplicate-name scenarios with clear user choices (open existing vs create anyway, if API supports).
2. Error handling:
	- Category not available.
	- Create call failed.
	- Create succeeded but follow-up append failed.
3. UX:
	- Display the new object title in the top-level `Choose target` subtitle immediately after create.
4. Investigation needed before implementation:
	- Fine-tune fallback behavior if app-open mode should auto-fallback to web.

## 10) Planned evolution for target object selection

Current behavior:
1. User saves `TargetObjectId` directly.

Planned behavior:
1. User enters text to search object title.
2. Extension calls `GET /objects/search` and returns matching titles.
3. User selects the desired result from the list.
4. Extension stores only the selected UUID internally as `TargetObjectId`.
5. UUID stays hidden from the user-facing picker UI.

## Immediate next coding milestone

After scaffold is added, implement in this order:
1. Settings model and persistence
2. Input parser
3. Capacities HTTP client with object search
4. Settings object selection flow
5. Command invocation wiring
6. Error/status UX
