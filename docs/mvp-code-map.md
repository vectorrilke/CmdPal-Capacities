# Code Map (current)

This map reflects the current implementation, not scaffold tasks.

## Core settings and state

1. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Settings/ExtensionSettings.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Settings/ExtensionSettings.cs)
   - Typed settings model.
   - Includes create-object post-action behavior.

2. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Settings/CapacitiesSettingsManager.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Settings/CapacitiesSettingsManager.cs)
   - Exposes CmdPal settings.
   - Stores target title in settings and target UUID in hidden local storage.
   - Emits in-session save flags for UI feedback.

## API integration

1. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Capacities/CapacitiesClient.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Capacities/CapacitiesClient.cs)
   - Append content: POST /blocks/append
   - Search objects: POST /objects/search
   - Load structures: GET /space/structures
   - Create object: POST /object/markdown
   - Load space id: GET /space
   - Build web URL for object open behavior.

2. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Capacities/CapacitiesContentComposer.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Capacities/CapacitiesContentComposer.cs)
   - Composes markdown payloads.

## Command handlers

1. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/AddToCapacitiesCommand.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/AddToCapacitiesCommand.cs)
   - Validates settings and sends input to Capacities.

2. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/SaveApiTokenCommand.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/SaveApiTokenCommand.cs)
   - Saves API token and returns to previous page.

3. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/SaveTargetObjectCommand.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/SaveTargetObjectCommand.cs)
   - Saves active target and returns to previous page.

4. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/CreateObjectCommand.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Commands/CreateObjectCommand.cs)
   - Creates object.
   - Sets created object as active target.
   - Executes post-create open behavior.

## Pages and UX flow

1. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CapacitiesCommandPaletteExtensionPage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CapacitiesCommandPaletteExtensionPage.cs)
   - Main actions and status subtitles.

2. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/SendTextPage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/SendTextPage.cs)
   - Primary input flow for sending content.

3. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/SaveApiTokenPage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/SaveApiTokenPage.cs)
   - API token save flow.

4. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/SelectTargetObjectPage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/SelectTargetObjectPage.cs)
   - Target object search and selection flow.

5. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CreateObjectChooseStructurePage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CreateObjectChooseStructurePage.cs)
   - Step 1: choose/search structure.

6. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CreateObjectEnterNamePage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CreateObjectEnterNamePage.cs)
   - Step 2: enter object name.

7. [src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CreateObjectEnterContentPage.cs](../src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Pages/CreateObjectEnterContentPage.cs)
   - Step 3: enter content and create.

## Immediate next step

1. Deploy and reload extension.
2. Run end-to-end test for send, target selection, and create-object post-action modes.
