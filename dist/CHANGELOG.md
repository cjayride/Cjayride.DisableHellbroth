# Changelog

## 1.0.2

- Verified against Valheim 1.0.21 (Release build).
- BepInEx dependency set to `denikson-BepInExPack_Valheim-5.4.2360`.

## 1.0.1

- Remove Odins Alchemy Wand from ObjectDB so it cannot be gambled, crafted, or spawned as an item. Hellbroth recipe and use blocks also cover the wand.

## 1.0.0

- Strip Hellbroth recipes from ObjectDB (including after PotionsPlus re-registers).
- Block crafting Hellbroth (fixes workbench Upgrade → Craft tab bypass).
- Remove Hellbroth from crafting UI; block using Hellbroth items.
