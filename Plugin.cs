using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;

namespace Cjayride.DisableHellbroth
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency("com.odinplus.potionsplus", BepInDependency.DependencyFlags.SoftDependency)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginGuid = "cjayride.DisableHellbroth";
        public const string PluginName = "Disable Hellbroth";
        public const string PluginVersion = "1.0.0";

        internal static BepInEx.Logging.ManualLogSource Log;

        private void Awake()
        {
            Log = Logger;
            new Harmony(PluginGuid).PatchAll();
            var go = new GameObject("Cjayride.DisableHellbroth.Purge");
            DontDestroyOnLoad(go);
            go.AddComponent<DelayedRecipePurge>();
            Logger.LogInfo("Hellbroth potions: recipes purged and craft/use blocked (PotionsPlus Disabled bypass fix).");
        }
    }

    internal static class HellbrothIds
    {
        internal static bool IsHellbroth(string name)
        {
            return !string.IsNullOrEmpty(name)
                   && name.IndexOf("Hellbroth", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        internal static bool IsHellbroth(ItemDrop.ItemData item)
        {
            if (item?.m_shared == null)
                return false;

            if (item.m_dropPrefab != null && IsHellbroth(item.m_dropPrefab.name))
                return true;

            return IsHellbroth(item.m_shared.m_name);
        }

        internal static bool IsHellbroth(Recipe recipe)
        {
            return recipe?.m_item != null && IsHellbroth(recipe.m_item.m_itemData);
        }
    }

    internal static class HellbrothRecipePurge
    {
        internal static void PurgeObjectDb(ObjectDB db)
        {
            if (db?.m_recipes == null)
                return;

            int removed = db.m_recipes.RemoveAll(HellbrothIds.IsHellbroth);
            if (removed > 0)
                Plugin.Log?.LogInfo($"Removed {removed} Hellbroth recipe(s) from ObjectDB.");
        }
    }

    internal sealed class DelayedRecipePurge : MonoBehaviour
    {
        private void Start()
        {
            StartCoroutine(PurgeRepeatedly());
        }

        private static IEnumerator PurgeRepeatedly()
        {
            // PotionsPlus can register recipes after ObjectDB.Awake / config sync.
            float[] delays = { 0.5f, 2f, 5f, 15f, 30f };
            for (int i = 0; i < delays.Length; i++)
            {
                yield return new WaitForSeconds(delays[i]);
                if (ObjectDB.instance != null)
                    HellbrothRecipePurge.PurgeObjectDb(ObjectDB.instance);
            }
        }
    }

    [HarmonyPatch(typeof(ObjectDB), nameof(ObjectDB.Awake))]
    internal static class ObjectDbAwakePurgeHellbroth
    {
        private static void Postfix(ObjectDB __instance) => HellbrothRecipePurge.PurgeObjectDb(__instance);
    }

    [HarmonyPatch(typeof(ObjectDB), "CopyOtherDB")]
    internal static class ObjectDbCopyPurgeHellbroth
    {
        private static void Postfix(ObjectDB __instance) => HellbrothRecipePurge.PurgeObjectDb(__instance);
    }

    [HarmonyPatch(typeof(InventoryGui), "DoCrafting")]
    internal static class BlockHellbrothCrafting
    {
        private static readonly FieldInfo CraftRecipeField =
            AccessTools.Field(typeof(InventoryGui), "m_craftRecipe");

        private static bool Prefix(InventoryGui __instance, Player player)
        {
            var recipe = CraftRecipeField?.GetValue(__instance) as Recipe;
            if (!HellbrothIds.IsHellbroth(recipe))
                return true;

            if (player != null)
                player.Message(MessageHud.MessageType.Center, "$msg_blocked", 0);

            HellbrothRecipePurge.PurgeObjectDb(ObjectDB.instance);
            return false;
        }
    }

    [HarmonyPatch(typeof(InventoryGui), "UpdateCraftingPanel")]
    internal static class FilterHellbrothCraftingUi
    {
        private static readonly FieldInfo AvailableField =
            AccessTools.Field(typeof(InventoryGui), "m_availableRecipes");

        private static readonly FieldInfo SelectedField =
            AccessTools.Field(typeof(InventoryGui), "m_selectedRecipe");

        private static readonly FieldInfo CraftRecipeField =
            AccessTools.Field(typeof(InventoryGui), "m_craftRecipe");

        private static readonly FieldInfo RecipeFieldOnPair =
            AccessTools.Field(AccessTools.Inner(typeof(InventoryGui), "RecipeDataPair"), "Recipe");

        private static void Postfix(InventoryGui __instance)
        {
            var list = AvailableField?.GetValue(__instance) as IList;
            if (list == null || RecipeFieldOnPair == null)
                return;

            for (int i = list.Count - 1; i >= 0; i--)
            {
                if (list[i] == null)
                    continue;

                var recipe = RecipeFieldOnPair.GetValue(list[i]) as Recipe;
                if (HellbrothIds.IsHellbroth(recipe))
                    list.RemoveAt(i);
            }

            ClearSelectedIfHellbroth(__instance);

            var craftRecipe = CraftRecipeField?.GetValue(__instance) as Recipe;
            if (HellbrothIds.IsHellbroth(craftRecipe))
                CraftRecipeField?.SetValue(__instance, null);
        }

        private static void ClearSelectedIfHellbroth(InventoryGui gui)
        {
            var holder = SelectedField?.GetValue(gui);
            if (holder == null || RecipeFieldOnPair == null)
                return;

            var recipe = RecipeFieldOnPair.GetValue(holder) as Recipe;
            if (!HellbrothIds.IsHellbroth(recipe))
                return;

            SelectedField?.SetValue(gui, null);
            CraftRecipeField?.SetValue(gui, null);
        }
    }

    [HarmonyPatch(typeof(Humanoid), nameof(Humanoid.UseItem))]
    internal static class BlockHellbrothUse
    {
        private static bool Prefix(Humanoid __instance, ItemDrop.ItemData item)
        {
            if (!HellbrothIds.IsHellbroth(item))
                return true;

            if (__instance is Player player)
                player.Message(MessageHud.MessageType.Center, "$msg_blocked", 0);

            return false;
        }
    }

}
