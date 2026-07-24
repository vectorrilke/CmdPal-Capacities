# Publishing Checklist & Timeline

Master guide for publishing Capacities Command Palette Extension to both WinGet and Microsoft Store.

## 📅 Timeline Overview

```
Week 1:
  Day 1-2: Prepare (author info, icons, screenshots)
  Day 3-5: WinGet submission
  Day 6-7: GitHub Actions setup

Week 2:
  Day 1-3: Microsoft Store build & submission
  Day 4+: Wait for certification & approvals
```

**Total time to live on both platforms: 1-2 weeks (first release)**

---

## 🎯 Phase 1: Preparation (Do First)

### Author Information

- [ ] Create `AUTHORS.md` in repo root with your details:
  ```markdown
  # Authors
  
  **Vector Rilke**
  - GitHub: [@vectorrilke](https://github.com/vectorrilke)
  - Email: your.email@example.com
  - Website: your-website.com
  ```

- [ ] Create `LICENSE` file (MIT recommended)

- [ ] Update README.md with author section (DONE ✅)

### Icons for Store

Required Store icons (create these):

- [ ] `Square44x44Logo.scale-200.png` (44×44)
- [ ] `Square150x150Logo.scale-200.png` (150×150) 
- [ ] `Wide310x150Logo.scale-200.png` (310×150)
- [ ] `SplashScreen.scale-200.png` (620×300)
- [ ] `StoreLogo.scale-100.png` (50×50)

**Action:** Use Visual Studio's asset generation tool or create them manually

**Save to:** `src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension/Assets/`

**Commit these to git**

### Screenshots

- [ ] Take 5 screenshots (1440×900 PNG format) showing:
  1. Search & Open Objects
  2. Create Object - Structure Selection
  3. Create Object - Content Entry
  4. Append to Object
  5. Success/Result

**Action:** Follow guide in `docs/gallery/SCREENSHOTS.md`

**Save to:** `docs/gallery/screenshots/`

**Name them:**
- `1-search-objects.png`
- `2-create-object.png`
- `3-create-content.png`
- `4-append-to-object.png`
- `5-success.png`

**Commit to git**

---

## 🌐 Phase 2: WinGet Publishing (Do Second - Faster)

### 2A: Build Installers

**Duration:** ~30 minutes first time

Prerequisites:
- [ ] Install WinGet Create: `winget install Microsoft.WingetCreate`
- [ ] Verify: `wingetcreate --version`
- [ ] Install GitHub CLI: `winget install GitHub.cli`
- [ ] Verify: `gh --version`

**Create build scripts in extension folder:**

- [ ] Create `build-exe.ps1` - PowerShell build script
- [ ] Create `setup-template.iss` - Inno Setup template

**First build:**
```powershell
cd src/CapacitiesCommandPaletteExtension/CapacitiesCommandPaletteExtension
.\build-exe.ps1 -ExtensionName "CapacitiesCommandPaletteExtension" -Version "0.1.0" -Platform @("x64", "arm64")
```

- [ ] Verify both installers created:
  - `bin\Release\installer\CapacitiesCommandPaletteExtension-Setup-0.1.0-x64.exe`
  - `bin\Release\installer\CapacitiesCommandPaletteExtension-Setup-0.1.0-arm64.exe`

- [ ] Commit `build-exe.ps1` and `setup-template.iss` to git

### 2B: Upload Installers to GitHub Release

- [ ] Create GitHub release: `git tag v0.1.0 && git push origin v0.1.0`

- [ ] Upload installers to release:
  1. Go to GitHub repo → Releases → Create Release
  2. Upload both .exe files
  3. Publish release

- [ ] Get download URLs for each installer (you'll need these for manifest)

### 2C: Generate WinGet Manifest

**Duration:** ~20 minutes

- [ ] Fork `microsoft/winget-pkgs` on GitHub (one-time)

- [ ] Clone your fork:
  ```powershell
  git clone https://github.com/YOUR_USERNAME/winget-pkgs.git
  ```

- [ ] Generate manifest using WingetCreate:
  ```powershell
  wingetcreate new `
    --urls "https://github.com/vectorrilke/cp_capabilities_extension/releases/download/v0.1.0/CapacitiesCommandPaletteExtension-Setup-0.1.0-x64.exe" `
    --version 0.1.0
  ```

- [ ] This creates manifest folder: `manifests/v/VectorRilke/CapacitiesCommandPaletteExtension/0.1.0/`

### 2D: Edit WinGet Manifest Files

**Duration:** ~15 minutes

Edit the generated YAML files to add author info:

**File:** `*.locale.en-US.yaml`

- [ ] Set:
  - `Author: Vector Rilke`
  - `AuthorUrl: https://github.com/vectorrilke`
  - `Publisher: Vector Rilke`
  - `PublisherUrl: https://github.com/vectorrilke`
  - `License: MIT`
  - `LicenseUrl: https://github.com/vectorrilke/cp_capabilities_extension/blob/main/LICENSE`

- [ ] Add Tags (CRITICAL):
  ```yaml
  Tags:
    - windows-commandpalette-extension
    - capacities
    - productivity
  ```

**File:** `*.installer.yaml`

- [ ] Verify both x64 and arm64 installers listed
- [ ] Add Windows App SDK dependency:
  ```yaml
  Dependencies:
    PackageDependencies:
      - PackageIdentifier: Microsoft.WindowsAppRuntime.1.6
  ```

- [ ] Save files

### 2E: Submit to microsoft/winget-pkgs

**Duration:** ~20 minutes (first time)

- [ ] Create branch in your fork:
  ```powershell
  cd winget-pkgs
  git checkout -b add-capacities-extension
  ```

- [ ] Copy your manifest files to correct location in fork

- [ ] Commit:
  ```powershell
  git add manifests/v/VectorRilke/CapacitiesCommandPaletteExtension/
  git commit -m "Add CapacitiesCommandPaletteExtension 0.1.0"
  git push origin add-capacities-extension
  ```

- [ ] Create Pull Request on GitHub
  - Go to `microsoft/winget-pkgs`
  - Click "Compare & pull request"
  - Add description: "First submission of Capacities Command Palette Extension"
  - Submit

- [ ] Monitor PR for validation issues
  - GitHub bots will run automated checks
  - Fix any validation errors
  - Maintainers will review

- [ ] Once merged: ✅ **WinGet live!** Users can: `winget install VectorRilke.CapacitiesCommandPaletteExtension`

### 2F: Set Up GitHub Actions for Future Updates

**Duration:** ~15 minutes

- [ ] Create `.github/workflows/winget-publish.yml` in your repo

- [ ] Configure to auto-submit manifest updates on version tags

- [ ] Test: Create test tag `git tag v0.1.1 && git push origin v0.1.1`

- [ ] Verify GitHub Actions workflow runs and creates PR to winget-pkgs

- [ ] Once confirmed, remove test tag if desired

---

## 🏪 Phase 3: Microsoft Store Publishing (Do Third)

### 3A: Prepare Extension Code

**Duration:** ~20 minutes

Prerequisites:
- [ ] Have Partner Center values copied (from when you created product earlier)

**Update `Package.appxmanifest`:**

- [ ] Replace placeholder values with Partner Center values:
  ```xml
  <Identity
      Name="<PACKAGE_IDENTITY_NAME>"
      Publisher="<PACKAGE_IDENTITY_PUBLISHER>"
      Version="0.1.0.0" />
  
  <Properties>
      <DisplayName>Capacities Command Palette Extension</DisplayName>
      <PublisherDisplayName><PUBLISHER_DISPLAY_NAME></PublisherDisplayName>
      <Logo>Assets\StoreLogo.png</Logo>
  </Properties>
  ```

**Update `.csproj`:**

- [ ] Add Partner Center values in PropertyGroup:
  ```xml
  <AppxPackageIdentityName>...</AppxPackageIdentityName>
  <AppxPackagePublisher>...</AppxPackagePublisher>
  <AppxPackageVersion>0.1.0.0</AppxPackageVersion>
  ```

**Verify icons:**

- [ ] All 5 required icons present in `Assets/` folder
- [ ] Run Visual Studio asset generator if needed

### 3B: Build MSIX Bundle

**Duration:** ~10-15 minutes

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

# Create bundle mapping and generate bundle
# (See docs/PARTNER_CENTER_SUBMISSION.md Step 4 for full commands)
```

- [ ] Verify `.msixbundle` file created:
  - `CapacitiesCommandPaletteExtension_0.1.0.0_Bundle.msixbundle`

### 3C: Submit to Partner Center

**Duration:** ~30-45 minutes (form filling)

- [ ] Go to [Partner Center](https://partner.microsoft.com/dashboard)

- [ ] Follow step-by-step guide: `docs/PARTNER_CENTER_SUBMISSION.md`

Key sections to complete:

- [ ] **Packages:** Upload `.msixbundle`
- [ ] **Pricing & Availability:** Set visibility & markets
- [ ] **Properties:** Set category, privacy policy
- [ ] **Age Rating:** Fill questionnaire
- [ ] **Product Listing:**
  - [ ] Short description
  - [ ] Full description
  - [ ] Release notes
- [ ] **Screenshots:** Upload all 5 screenshots with captions
- [ ] **Supplemental Info:** Add testing instructions
- [ ] **Compliance:** Review and check boxes

- [ ] Click **Submit to the Store**

### 3D: Monitor Certification

- [ ] Check Partner Center dashboard daily for status

- [ ] If failed: Check email for issues, fix, and resubmit

- [ ] Once approved: ✅ **Microsoft Store live!**

---

## 📋 Submission Checklist Template

### Before WinGet Submission
- [ ] Icons created & committed
- [ ] Build scripts created & tested (build-exe.ps1, setup-template.iss)
- [ ] Installers building successfully (x64 + ARM64)
- [ ] Installers uploaded to GitHub Release
- [ ] WinGet manifest generated with author info
- [ ] Tags include "windows-commandpalette-extension"
- [ ] Dependencies section filled (WindowsAppSdk)
- [ ] LICENSE file in repo
- [ ] AUTHORS.md created

### Before Store Submission
- [ ] Package.appxmanifest updated with Partner Center values
- [ ] .csproj updated with Partner Center values
- [ ] All 5 icons present in Assets/
- [ ] MSIX x64 and ARM64 built successfully
- [ ] .msixbundle created successfully
- [ ] 5 screenshots taken (1440×900 PNG)
- [ ] Description written
- [ ] Test instructions prepared
- [ ] Privacy policy URL ready

---

## 🚀 First Update Process (After Initial Release)

Once both platforms are live:

**For each new version:**

1. **Update version numbers** (e.g., 0.1.0 → 0.2.0)
   - Package.appxmanifest
   - .csproj
   - build-exe.ps1
   - GitHub tag

2. **WinGet (Automated):**
   - Tag release: `git tag v0.2.0 && git push origin v0.2.0`
   - GitHub Actions auto-creates PR to winget-pkgs
   - Merge and done! (1-3 days)

3. **Store (Manual):**
   - Build new MSIX bundle
   - Go to Partner Center → Create new submission
   - Upload new bundle
   - Update description/release notes if needed
   - Submit (1-3 days for certification)

---

## 📞 Support & References

- **WinGet Docs:** https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension-winget
- **Store Docs:** https://learn.microsoft.com/en-us/windows/powertoys/command-palette/publish-extension-store
- **Gallery Docs:** https://github.com/microsoft/CmdPal-Extensions
- **This Repo:** https://github.com/vectorrilke/cp_capabilities_extension
- **WinGet Repo:** https://github.com/microsoft/winget-pkgs

---

## 🎓 Key Takeaways

| Platform | Speed | Effort | Reach | Updates |
|----------|-------|--------|-------|---------|
| **WinGet** | ⚡ Fast (1-3 days) | 📝 Easy | 👥 Dev-focused | 🤖 Automated |
| **Store** | 🐢 Slow (2-7 days) | 📦 More steps | 🌐 Broad | 📋 Manual |

**Recommendation:** Launch WinGet first (faster), then Store (broader reach).

Both can coexist peacefully!
