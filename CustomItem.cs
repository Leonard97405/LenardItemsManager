using System;
using System.Linq;
using HarmonyLib;
using HintServiceMeow.Core.Enum;
using HintServiceMeow.Core.Extension;
using HintServiceMeow.Core.Models.Hints;
using HintServiceMeow.Core.Utilities;
using HintServiceMeow.UI.Utilities;
using InventorySystem.Items;
using LabApi.Features.Wrappers;
using MapGeneration.Distributors;
using MEC;
using UnityEngine.Rendering.HighDefinition;

namespace LenardItemsManager
{
    [Serializable]

    public abstract class CustomItem
    {
        public abstract string ItemId { get; set; }
        
        public abstract string ItemName { get; set; }
        public abstract string ItemDesc { get; set; }

        public abstract ItemType ItemType { get; set; }

        public abstract int SpawnChance { get; set; }
        public virtual int SpawnInLockersChance { get; set; }= 0;

        public virtual bool SpawnOnStart { get; set; } = true;
        public virtual float SpawnAfter { get; set; } = 0;
        public virtual ushort Itemserial { get; set; } = 0;
        public virtual int MaxItemSpawn { get; set; } = 1;

        public virtual SpawnProperties[] SpawnLocations { get; set; } = new SpawnProperties[0];

        public virtual string HintColor { get; set; } = "purple";
        public virtual void OnItemPickedUp(Player p)
        {
            var pD = PlayerDisplay.Get(p);
            DynamicHint hint = new DynamicHint()
            {
                Text = $"Hai raccolto un <color={HintColor}>{ItemName}</color>",
                Id = ItemId + "hint",
                FontSize = 30,
                TargetY = 850,
                SyncSpeed = HintSyncSpeed.UnSync
            };
            p.AddHint(hint);
            Timing.CallDelayed(1.5f, () => p.RemoveHint(hint));
        }

        public virtual void OnItemEquipped(Player p)
        {
            var pD = PlayerDisplay.Get(p);
            DynamicHint hint = new DynamicHint()
            {
                Text = $"Hai equippaggiato un <color={HintColor}>{ItemName}</color>\n {ItemDesc}",
                Id = ItemId + "equiphint",
                FontSize = 30,
                TargetY = 850,
                SyncSpeed = HintSyncSpeed.UnSync
            };
            pD.AddHint(hint);
            Timing.CallDelayed(2f, () => p.RemoveHint(hint));
            foreach (var z in PedestalLocker.Dictionary.Keys)
            {
                
            }
        }

        public abstract void OnItemUsing(Player p);
        public abstract void OnItemUsed(Player p);
        
		

    }
}