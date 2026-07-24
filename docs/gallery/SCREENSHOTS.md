# Screenshot Guide

This directory contains screenshots for the Capacities Command Palette Extension used in marketing materials, store listings, and the Extension Gallery.

## Screenshot Locations

### Microsoft Store
Store all screenshots in: `docs/gallery/screenshots/`

### Command Palette Extension Gallery
Reference screenshots from this folder in the CmdPal-Extensions repository submission.

## Required Screenshots

### For Microsoft Store (Minimum 1, Recommended 3-5)

1. **Main Menu / Initial Load** (`1-main-menu.png`)
   - Shows Command Palette with extension loaded
   - Displays top-level Capacities extension menu
   - Shows: Search objects, Create object, Set API token options
   - Size: 1440×900 minimum, **1920×1080 or larger recommended**

2. **Search & Open Objects** (`2-search-objects.png`)
   - Shows Command Palette with "capacities" or similar search
   - Displays list of Capacities objects from search
   - Size: 1440×900 minimum, **1920×1080 or larger recommended**

3. **Create Object Flow** (`3-create-object.png`)
   - Shows "Choose Structure" page
   - List of available structures visible
   - Size: 1440×900 minimum, **1920×1080 or larger recommended**

4. **Create Object - Enter Content** (`4-create-content.png`)
   - Shows "Enter Content" page
   - Rich markdown editor with sample content
   - Size: 1440×900 minimum, **1920×1080 or larger recommended**

5. **Append to Object** (`5-append-to-object.png`)
   - Shows "Append to Object" command
   - Content being appended visible
   - Size: 1440×900 minimum, **1920×1080 or larger recommended**

6. **Success/Result** (`6-success.png`)
   - Shows success toast or result
   - Object opened in Capacities or confirmation
   - Size: 1440×900 minimum, **1920×1080 or larger recommended**

### For Extension Gallery (Optional)

- `hero-image.png` - Featured image (1024×576 or similar)
- Any of the above screenshots work well

## How to Create Screenshots

### Tools
- **Windows Snipping Tool**: Win+Shift+S
- **PowerToys Screen Sketch**: Included with PowerToys
- **Capture Workflow**: 
  1. Deploy extension in Visual Studio
  2. Open Command Palette (Win+Alt+Space)
  3. Interact with extension
  4. Capture using Snip & Sketch
  5. Save to `docs/gallery/screenshots/`

### Best Practices
- Use 1440×900 resolution for consistency
- Show real UI, not mockups
- Keep search/content representative but not revealing
- Avoid showing API tokens or sensitive data
- Use default Command Palette dark theme
- Ensure text is readable (good contrast)
- Show success states, not errors
- Capture 5-6 different scenarios

## File Naming Convention

```
1-main-menu.png
2-search-objects.png
3-create-object.png
4-create-content.png
5-append-to-object.png
6-success.png
```

## Next Steps

1. Take screenshots following the guide above
2. Save them to this folder
3. Once ready, use in Partner Center submission
4. Reference in Gallery submission if applicable

## Partner Center Upload Instructions

When submitting to Microsoft Store:

1. Go to Partner Center → Your Extension → Product listing
2. Scroll to **Screenshots** section
3. Click **Add images**
4. Upload PNG/JPG files from `docs/gallery/screenshots/`
5. For each screenshot, add a description:
   - "Search and open Capacities objects in Command Palette"
   - "Create new objects with structure selection"
   - "Enter content for new objects"
   - "Append content to existing objects"
   - "Success confirmation after operations"
6. Arrange in logical order
7. Save changes

**Important**: Screenshots must be 1440×900 minimum size, PNG or JPG format.
