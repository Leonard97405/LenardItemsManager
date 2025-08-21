using LabApi.Events.CustomHandlers;
using LabApi.Features.Wrappers;
using LenardItemsManager;
using MapGeneration;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace CustomItemTemplate
{
    public class CustomTemplate : CustomItem
    {
        public override void OnItemUsing(Player p)
        {
            throw new System.NotImplementedException();
        }

        public override void OnItemUsed(Player p)
        {
            Logger.Info("Item custom usato");
        }

        public override string ItemId { get; set; } = "Template Item";
        public override string ItemName { get; set; } = "Item prova";
        public override string ItemDesc { get; set; } = "Item di prova";
        public override ItemType ItemType { get; set; } = ItemType.Adrenaline;
        public override int SpawnChance { get; set; } = 100;
        // Da usare per spawnare item nei locker
        public override int SpawnInLockersChance { get; set; } = 0;

        public override SpawnProperties[] SpawnLocations { get; set; } = new[]
        {
            new SpawnProperties()
            {
                RoomName = RoomName.Lcz330,
                Offset = new Vector3(1.45f, 1.9f, -2.54f),
                Chance = 100,
            }
        };
    }
    
    public class ItemEvents : CustomEventsHandler
    {
     // 
    }
}