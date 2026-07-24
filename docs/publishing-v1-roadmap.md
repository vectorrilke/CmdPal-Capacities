# Publishing Roadmap for Version 1.0

This checklist covers what still needs to happen before the extension should be published publicly.

## Release target

Target release line:

1. Version 0.9 = current pre-release candidate
2. Version 1.0 = first public release after deploy and validation
3. Version 1.1 = post-release stabilization and polish
4. Version 1.2 = Microsoft Store release

## Immediate next step

1. Deploy the current branch build locally.
2. Reload Command Palette Extension.
3. Run a full manual smoke test.

## Manual test checklist for Version 1.0

1. API token
   - save valid token
   - verify masked token appears in settings only
   - clear token from token page
   - verify extension returns to Set API token state
2. Search
   - type short query below minimum length
   - type real query and confirm debounced results load
   - verify grouped structure headers and object rows
   - verify no redundant actions appear on the active search screen
3. Open actions
   - open result in Capacities app
   - open result in Capacities web
4. Append flow
   - append text then open in app
   - append text then open in web
   - verify markdown and escaped newline behavior
5. Create object flow
   - choose structure
   - create object with content
   - verify configured post-create behavior
6. Error handling
   - invalid token
   - empty token
   - no result search
   - rate-limited search scenario if reproducible

## Repo and release docs

Before 1.0 publish:

1. Finalize README.
2. Finalize API details doc.
3. Add release screenshots.
4. Confirm icon assets are publish-ready.
5. Confirm version references consistently say 0.9 or 1.0 where appropriate.

## Packaging and distribution prerequisites

Based on the CmdPal gallery contributing guidance:

1. The initial public distribution path for this project is winget.
2. After winget is live, submit the extension to the CmdPal gallery repository.
3. Microsoft Store is planned later, around Version 1.2.

## Release sequence

1. Version 1.0
   - deploy and validate
   - publish to winget
   - submit to CmdPal gallery with winget install source
2. Version 1.1
   - fix issues from initial public usage
   - refine icon mapping and row-level visual contrast
   - improve packaging and listing quality where needed
3. Version 1.2
   - publish to Microsoft Store
   - optionally update gallery metadata if Store should become the preferred install source

## CmdPal gallery submission checklist

Once the winget install source exists:

1. Fork the CmdPal-Extensions repository.
2. Create a branch for the gallery submission.
3. Add an extension folder under:
   - extensions/<author>/<extension-name>/
4. Add extension.json with:
   - id
   - title
   - shortDescription
   - description
   - author
   - icon
   - installSources
   - homepage
   - optional tags and categories
5. Add icon.png or icon.jpg.
6. Optionally add screenshots/ with up to 5 images.
7. Open PR to CmdPal-Extensions main.
8. Fix any CI validation issues.
9. Wait for maintainer review and merge.

## Suggested metadata prep for this project

Likely gallery metadata direction:

1. Category:
   - productivity
   - utilities-and-tools
2. Candidate tags:
   - capacities
   - notes
   - knowledge-management
   - command-palette
   - markdown

## Version 2.0 forward plan

After Version 1.2 distribution is in place:

1. Create Tasks
2. Create Weblinks
3. Create Daily Notes
4. Create media objects
5. Create or attach PDFs
6. Set tags for new objects
7. Add richer structure-specific create flows
8. Improve metadata-driven icons