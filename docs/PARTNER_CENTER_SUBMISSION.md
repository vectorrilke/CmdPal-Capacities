# Microsoft Store Submission Guide

Complete step-by-step guide for uploading the Capacities Command Palette Extension to Microsoft Store via Partner Center.

## Overview

This guide covers the exact steps to submit your MSIX bundle to Microsoft Store and get certified.

**Timeline:** 
- Submission: 30 minutes
- Microsoft review: 1-3 days
- Total to live: 2-7 days

## Prerequisites

- ✅ Microsoft Store developer account (already registered)
- ✅ MSIX bundle created (`CapacitiesCommandPaletteExtension_X.X.X.X_Bundle.msixbundle`)
- ✅ 7 required icons in Assets folder
- ✅ Screenshots ready (1440×900 PNG or JPG, 5 recommended)
- ✅ Author information (name, company, contact)

## Step-by-Step Submission

### Step 1: Create Product in Partner Center

1. Go to [partner.microsoft.com/dashboard](https://partner.microsoft.com/dashboard)
2. Select **Apps and games** from Workspaces
3. Click **+ New Product**
4. Select **MSIX or PWA app**
5. Enter your extension name: `Capacities Command Palette Extension`
6. Click **Create**

**Important:** Save the reserved name - this will be your official Store name.

### Step 2: Get Identity Values from Partner Center

1. In your newly created product, go to **Product identity** (left sidebar under Product Management)
2. Copy these exact values - you'll need them:

```
Package/Identity/Name: ____________________________
Package/Identity/Publisher: ____________________________
Package/Properties/PublisherDisplayName: ____________________________
```

**These go in your code before building MSIX!**

### Step 3: Prepare Your Extension Code

Before building MSIX, update your project with Partner Center values:

**File:** `Package.appxmanifest`

```xml
<Identity
    Name="<YOUR_PACKAGE_IDENTITY_NAME>"
    Publisher="<YOUR_PACKAGE_IDENTITY_PUBLISHER>"
    Version="0.1.0.0" />

<Properties>
    <DisplayName>Capacities Command Palette Extension</DisplayName>
    <PublisherDisplayName><YOUR_PUBLISHER_DISPLAY_NAME></PublisherDisplayName>
    <Logo>Assets\StoreLogo.png</Logo>
</Properties>
```

**File:** `CapacitiesCommandPaletteExtension.csproj`

```xml
<PropertyGroup>
    <AppxPackageIdentityName><YOUR_PACKAGE_IDENTITY_NAME></AppxPackageIdentityName>
    <AppxPackagePublisher><YOUR_PACKAGE_IDENTITY_PUBLISHER></AppxPackagePublisher>
    <AppxPackageVersion>0.1.0.0</AppxPackageVersion>
</PropertyGroup>
```

### Step 4: Build MSIX Bundle

```powershell
cd src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension

# Build x64
dotnet build --configuration Release `
  -p:GenerateAppxPackageOnBuild=true `
  -p:Platform=x64 `
  -p:AppxPackageDir="AppPackages\x64\"

# Build ARM64
dotnet build --configuration Release `
  -p:GenerateAppxPackageOnBuild=true `
  -p:Platform=ARM64 `
  -p:AppxPackageDir="AppPackages\ARM64\"

# Create bundle
# First, create bundle_mapping.txt:
@"
[Files]
"AppPackages\CapacitiesCommandPaletteExtension_0.1.0.0_x64_Test\CapacitiesCommandPaletteExtension_0.1.0.0_x64.msix" "CapacitiesCommandPaletteExtension_0.1.0.0_x64.msix"
"AppPackages\CapacitiesCommandPaletteExtension_0.1.0.0_arm64_Test\CapacitiesCommandPaletteExtension_0.1.0.0_arm64.msix" "CapacitiesCommandPaletteExtension_0.1.0.0_arm64.msix"
"@ | Set-Content -Path "bundle_mapping.txt" -Encoding UTF8

# Create the bundle
& "C:\Program Files (x86)\Windows Kits\10\bin\10.0.22621.0\x64\makeappx.exe" bundle /f bundle_mapping.txt /p CapacitiesCommandPaletteExtension_0.1.0.0_Bundle.msixbundle

# Verify
dir *.msixbundle
```

### Step 5: Complete Product Submission Form

Back in Partner Center, you now need to fill out the submission form. Here's what each section needs:

#### **5a. Packages Section**

1. Go to **Packages** (left sidebar)
2. Click **Upload your application package**
3. Select your `.msixbundle` file
4. Click **Upload**
5. Wait for upload to complete and validation to pass

#### **5b. Pricing & Availability**

1. Go to **Pricing & availability**
2. Select:
   - Visibility: **Public** (unless you want private)
   - Markets: Choose applicable regions
   - Release date: Now or schedule
3. Save

#### **5c. Properties**

1. Go to **Properties**
2. Fill in:
   - **Category:** Productivity
   - **Subcategory:** Utilities
   - **Privacy policy URL:** `https://github.com/vectorrilke/cp_capabilities_extension`
3. Save

#### **5d. Age Rating**

1. Go to **Age ratings & rating certificates**
2. Fill the questionnaire (usually all "No" for this type of app)
3. Save

#### **5e. Product Listing - THE IMPORTANT PART**

1. Go to **Product listing**

**Language:** English (United States)

2. **Short description (required):**
```
Capacities Command Palette Extension
```

3. **Full description (required):**
```
The Capacities Command Palette Extension integrates seamlessly with the Windows Command Palette (PowerToys) to provide fast access to your Capacities workspace.

Features:
• Search and open objects without leaving Command Palette
• Create new objects with structure selection
• Append text to existing objects
• Automatic updates via WinGet

Usage:
1. Install PowerToys Command Palette
2. Type "capacities" or your custom command to access the extension
3. Search, create, and manage objects directly from Command Palette

Requirements:
• Windows 10 (version 19041 or later)
• PowerToys Command Palette (version X.X or later)

For support and issues, visit: https://github.com/vectorrilke/cp_capabilities_extension
```

4. **Release notes (optional but recommended):**
```
Version 0.1.0 - Initial Release
• Search Capacities objects
• Create new objects
• Append to objects
• Save API token
• Support for all structure types (except excluded)
```

#### **5f. Screenshots Section**

1. Scroll to **Screenshots**
2. Click **Add images**
3. Upload your screenshots in order:
   - `1-search-objects.png` - "Search and open objects in Command Palette"
   - `2-create-object.png` - "Create new objects with structure selection"
   - `3-create-content.png` - "Enter content for new objects"
   - `4-append-to-object.png` - "Append text to existing objects"
   - `5-success.png` - "Success confirmation"

**Important:** 
- Minimum 1 screenshot (recommended 3-5)
- Size: 1440×900 pixels minimum
- Format: PNG or JPG
- Add descriptive captions for each

#### **5g. Supplemental info - Additional Testing Information**

1. Go to **Supplemental info**
2. Click **Additional Testing Information**
3. Add test instructions:

```
Testing Instructions:

Prerequisites:
• PowerToys must be installed (available in Microsoft Store)
• Command Palette must be enabled in PowerToys settings

Setup:
1. Install the extension
2. Open Command Palette (Win+Alt+Space)
3. Type "capacities" or your configured command

Test Flows:
1. Search: Type 3+ characters to search objects
2. Create: When no query, option to create appears
3. Token: Set up API token from settings
4. Append: Select object and append content

Requirements:
• Windows 10 version 19041 or later
• PowerToys and Command Palette running
```

4. Save

#### **5h. Compliance**

1. Go to **Compliance**
2. Review and check boxes as applicable
3. Save

### Step 6: Submit for Certification

1. Review all sections for completeness (Partner Center will show warnings)
2. Click **Submit to the Store** (top right)
3. Confirm submission

**You're done!** Microsoft will now review your submission.

---

## After Submission

### Monitoring Your Submission

1. Go back to Partner Center dashboard
2. Find your extension in the list
3. Status will show:
   - **In submission** - Currently being reviewed
   - **Certification in progress** - Being tested
   - **In the Store** - Approved and live
   - **Failed** - Issues to fix (review email)

### If Certification Fails

1. Check email for failure details
2. Fix issues
3. Create new submission with updated version
4. Resubmit

### Once Approved

✅ Extension is now live on Microsoft Store
✅ Users can install: Search "Capacities Command Palette Extension"
✅ Automatic updates when you submit new versions

---

## Future Updates

### To publish a new version:

1. Update version: `0.1.0.0` → `0.2.0.0`
   - In `Package.appxmanifest`
   - In `.csproj`
   - In `build-exe.ps1`

2. Build new MSIX bundle (repeat Step 4)

3. In Partner Center:
   - Go to **Packages**
   - Upload new `.msixbundle`
   - Update description/release notes if needed
   - Click **Submit to the Store**

4. Wait for certification again (1-3 days)

---

## Checklist for Submission

- [ ] MSIX bundle built successfully
- [ ] All 7 icons in Assets folder
- [ ] 5 screenshots ready (1440×900 PNG)
- [ ] Partner Center values in Package.appxmanifest
- [ ] Partner Center values in .csproj
- [ ] Description written
- [ ] Test instructions added
- [ ] Privacy policy URL set
- [ ] Category selected (Productivity)
- [ ] Screenshots uploaded with captions
- [ ] Compliance section reviewed

---

## Support & Help

- **Microsoft Store Certification:** [Publish Windows apps](https://learn.microsoft.com/en-us/windows/apps/publish/)
- **Command Palette Extension Docs:** [Official Docs](https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension-store)
- **Partner Center Help:** [Partner Center Dashboard](https://partner.microsoft.com/dashboard)
- **Your GitHub Issues:** [cp_capabilities_extension/issues](https://github.com/vectorrilke/cp_capabilities_extension/issues)
