# Build MSIX Bundle for Microsoft Store Submission
# This script builds x64 + ARM64, creates bundle_mapping.txt, and generates the .msixbundle

param(
    [string]$ExtensionName = "CapacitiesCommandPaletteExtension",
    [string]$VersionNumber = "0.1.1.0"
)

Write-Host "Building MSIX Bundle for Microsoft Store" -ForegroundColor Cyan

# Generated package output must not become content in the second architecture build.
if (Test-Path "AppPackages") {
    Remove-Item -Path "AppPackages" -Recurse -Force
}

# Step 1: Build x64 + ARM64 MSIX
Write-Host "`nBuilding x64 MSIX..." -ForegroundColor Yellow
dotnet build --configuration Release -p:RuntimeIdentifier=win-x64 -p:SelfContained=true -p:WindowsPackageType=MSIX -p:GenerateAppxPackageOnBuild=true -p:AppxBundle=Never -p:Platform=x64 -p:AppxPackageDir="AppPackages\x64\"
if ($LASTEXITCODE -ne 0) {
    Write-Host "x64 build failed" -ForegroundColor Red
    exit 1
}
Write-Host "x64 build complete" -ForegroundColor Green

Write-Host "`nBuilding ARM64 MSIX..." -ForegroundColor Yellow
dotnet build --configuration Release -p:RuntimeIdentifier=win-arm64 -p:SelfContained=true -p:WindowsPackageType=MSIX -p:GenerateAppxPackageOnBuild=true -p:AppxBundle=Never -p:Platform=ARM64 -p:AppxPackageDir="AppPackages\ARM64\"
if ($LASTEXITCODE -ne 0) {
    Write-Host "ARM64 build failed" -ForegroundColor Red
    exit 1
}
Write-Host "ARM64 build complete" -ForegroundColor Green

# Step 2: Verify both MSIX files exist
Write-Host "`nVerifying MSIX files..." -ForegroundColor Yellow
$x64MSIXPattern = "$($ExtensionName)_$($VersionNumber)_x64.msix"
$arm64MSIXPattern = "$($ExtensionName)_$($VersionNumber)_arm64.msix"

$x64MSIX = Get-ChildItem -Path "AppPackages" -Filter $x64MSIXPattern -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1
$arm64MSIX = Get-ChildItem -Path "AppPackages" -Filter $arm64MSIXPattern -Recurse -ErrorAction SilentlyContinue | Select-Object -First 1

if (-not $x64MSIX -or -not $arm64MSIX) {
    Write-Host "MSIX files not found" -ForegroundColor Red
    Write-Host "Expected under AppPackages (recursive): $x64MSIXPattern" -ForegroundColor Red
    Write-Host "Expected under AppPackages (recursive): $arm64MSIXPattern" -ForegroundColor Red
    exit 1
}

Write-Host "Found x64: $($x64MSIX.FullName)" -ForegroundColor Green
Write-Host "Found ARM64: $($arm64MSIX.FullName)" -ForegroundColor Green

# Step 3: Create bundle_mapping.txt
Write-Host "`nCreating bundle_mapping.txt..." -ForegroundColor Yellow

$x64Relative = $x64MSIX.FullName -replace [regex]::Escape((Get-Location).Path + "\"), ""
$arm64Relative = $arm64MSIX.FullName -replace [regex]::Escape((Get-Location).Path + "\"), ""

$x64Line = "`"$x64Relative`" `"$($ExtensionName)_$($VersionNumber)_x64.msix`""
$arm64Line = "`"$arm64Relative`" `"$($ExtensionName)_$($VersionNumber)_arm64.msix`""
$bundleContent = "[Files]`n$x64Line`n$arm64Line"

$bundleContent | Out-File -FilePath "bundle_mapping.txt" -Encoding ASCII
Write-Host "Created bundle_mapping.txt" -ForegroundColor Green
Write-Host $bundleContent

# Step 4: Find and run makeappx
Write-Host "`nFinding makeappx.exe..." -ForegroundColor Yellow

$arch = switch ($env:PROCESSOR_ARCHITECTURE) { 
    "AMD64" { "x64" } 
    "x86" { "x86" } 
    "ARM64" { "arm64" } 
    default { "x64" } 
}
Write-Host "Detected architecture: $arch"

$makeappx = Get-ChildItem "C:\Program Files (x86)\Windows Kits\10\bin\*\$arch\makeappx.exe" -ErrorAction SilentlyContinue | 
            Sort-Object Name -Descending | 
            Select-Object -First 1

if (-not $makeappx) {
    Write-Host "makeappx.exe not found" -ForegroundColor Red
    Write-Host "Please install Windows App SDK or Windows 10/11 SDK" -ForegroundColor Red
    exit 1
}

Write-Host "Found makeappx: $($makeappx.FullName)" -ForegroundColor Green

# Step 5: Create bundle
Write-Host "`nCreating .msixbundle..." -ForegroundColor Yellow

$bundleOutput = "$($ExtensionName)_$($VersionNumber)_Bundle.msixbundle"
if (Test-Path $bundleOutput) {
    Remove-Item -Path $bundleOutput -Force
}
& $makeappx.FullName bundle /bv $VersionNumber /f bundle_mapping.txt /p $bundleOutput

if ($LASTEXITCODE -ne 0) {
    Write-Host "Bundle creation failed" -ForegroundColor Red
    exit 1
}

# Step 6: Verify bundle
if (Test-Path $bundleOutput) {
    $bundleSize = (Get-Item $bundleOutput).Length / 1MB
    Write-Host "`nSUCCESS: Bundle created" -ForegroundColor Green
    Write-Host "File: $bundleOutput" -ForegroundColor Green
    Write-Host "Size: $([math]::Round($bundleSize, 2)) MB" -ForegroundColor Green
    Write-Host "`nReady to upload to Partner Center!" -ForegroundColor Cyan
} else {
    Write-Host "Bundle file not found after creation" -ForegroundColor Red
    exit 1
}
