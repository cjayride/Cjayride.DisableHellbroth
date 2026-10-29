# DisableHellbroth — Valheim 1.0.21 release

## Goal
- Verify Release build against Valheim 1.0.21, update BepInExPack to 5.4.2360, changelog, Thunderstore `/dist` zip, git tag `v1.0.1`.

## Status
- Completed

## Verification
- `dotnet build -c Release` — succeeded (0 warnings, 0 errors) against `D:\SteamLibrary\steamapps\common\Valheim` (assembly_valheim.dll updated 2026-10-28).

## Changes
- `Package/manifest.json` — BepInEx `denikson-BepInExPack_Valheim-5.4.2360`.
- CHANGELOG (root + Package) — Valheim 1.0.21 + BepInEx notes under 1.0.1.
- `dist/` Thunderstore staging (zip gitignored).

## Output
- DLL: `D:\AI-Agent-Projects\cjaycraft\Cjayride.DisableHellbroth\bin\Release\net481\Cjayride.DisableHellbroth.dll`
- Zip: `D:\AI-Agent-Projects\cjaycraft\Cjayride.DisableHellbroth\dist\cjayride-DisableHellbroth-1.0.1.zip`
