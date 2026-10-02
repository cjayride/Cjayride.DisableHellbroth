# Disable Hellbroth

PotionsPlus `Crafting Station = Disabled` for Hellbroth does not always remove recipes (e.g. workbench **Upgrade** tab → **Craft** tab can still offer Hellbroth of Flames).

This mod:

- Removes Hellbroth recipes from `ObjectDB` (including after PotionsPlus re-registers them).
- Blocks `InventoryGui.DoCrafting` for Hellbroth results.
- Strips Hellbroth from the crafting UI recipe list.
- Blocks drinking/using Hellbroth items.

Install on **dedicated server and all clients**.

## Changelog

### 1.0.0

- Initial release.
