using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using InventorySystem;
using InventorySystem.Items;
using InventorySystem.Items.Pickups;
using InventorySystem.Items.Usables;
using LabApi.Events.Arguments.PlayerEvents;
using LabApi.Events.Arguments.ServerEvents;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Features.Wrappers;
using LabApi.Loader.Features.Plugins;
using LabApi.Loader.Features.Plugins.Enums;
using MapGeneration;
using MapGeneration.Distributors;
using MEC;
using UnityEngine;
using Logger = LabApi.Features.Console.Logger;

namespace LenardItemsManager
{
    [Serializable]

    public class ItemsManager : Plugin<Config>
    {
        public static ItemsManager Singleton { get; private set; }

        public ItemEventManager Events { get; } = new ItemEventManager();

        public List<CustomItem> RegisteredItems = new List<CustomItem>();

        public override void Enable()
        {
            Singleton = this;
            CustomHandlersManager.RegisterEventsHandler(Events);
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(Events);
        }

        public override string Name { get; } = "LenardItemsManager";
        public override string Description { get; } = "Sistema per managing di item custom";
        public override string Author { get; } = "Lenard";
        public override Version Version { get; } = new Version(1, 1, 1);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
        public override LoadPriority Priority { get; } = LoadPriority.Highest;

        public bool HasItem(CustomItem I, Player p)
        {
            if (p.Inventory.UserInventory.Items.Values.Any(j => j.ItemSerial == I.Itemserial)) return true;
            return false;
        }

        public bool HasItemEquipped(CustomItem I, Player p)
        {
			if(p.CurrentItem == null) return false;
            if (p.CurrentItem.Serial == I.Itemserial) return true;
            return false;
        }

        public bool IsItemCustom(CustomItem I, ItemBase Z)
        {
            if(Z == null) return false;
            if (Z.ItemSerial == I.Itemserial) return true;
            return false;
        }

        public void RegisterItem(CustomItem item)
        {
            if (item == null)
            {
                Logger.Error("[RegisterItem] Non posso registrare un item inesistente");
                return;
            }

            if (RegisteredItems.Any(I => I.ItemId == item.ItemId))
            {
                Logger.Warn("[RegisterItem] Item già registrato");
                return;
            }
			
            item.Itemserial = ItemSerialGenerator.GenerateNext();
            RegisteredItems.Add(item);
            Logger.Info("[RegisterItem] Item registrato: " + item.ItemName + " con id " + item.ItemId);
        }

        private bool CanItemSpawn(CustomItem item)
        {
            if (item == null)
            {
                Logger.Error("[CanItemSpawn] Item inesistente");
                return false;
            }

            if (!item.SpawnOnStart)
            {
                return false;
            }
            
            return true;
        }
        

        public void TrySpawnItems()
        {
            var itemPossibili = RegisteredItems
                .Where(i => CanItemSpawn(i))
                .OrderByDescending(i => i.SpawnChance)
                .ToList();
            int j = 0;
            Logger.Info("[LenardItemsManager] Trying to spawn custom items");
            foreach (var z in itemPossibili)
            {
                int chance = RandomNumberGenerator.GetInt32(101);
                if (chance <= z.SpawnChance)
                {
                     j++;
                    if (z.SpawnAfter != 0)
                    {
                        Timing.RunCoroutine(spawnDelayed(z),j);
                        continue;
                        
                    }
                    SpawnItem(z);
                }
            }
            
        } 

        public IEnumerator<float> spawnDelayed(CustomItem i)
        {
            yield return Timing.WaitForSeconds(i.SpawnAfter);
            SpawnItem(i);
        }

        public void SpawnItem(CustomItem item, Player p = null)
        {
        
            if (p != null)
            {
                p.Inventory.ServerAddItem(item.ItemType, ItemAddReason.AdminCommand, itemSerial: item.Itemserial);
            }
            else
            {
                if (item.SpawnInLockersChance > 0)
                {
                    Pickup P = Pickup.Create(item.ItemType, new Vector3(750,-1000,750));
                    P.Base.Info.Serial = item.Itemserial;
                    foreach (var j in PedestalLocker.List)
                    {
                        if (RandomNumberGenerator.GetInt32(101) <= item.SpawnInLockersChance)
                        {
                            j.Chamber.AddItem(P.Type);
                            j.Chamber.GetAllItems().LastOrDefault().Base.Info.Serial = item.Itemserial;
                        }
                    }
                }
                foreach (var z in item.SpawnLocations)
                {
                    if (RandomNumberGenerator.GetInt32(101) <= z.Chance)
                    {
                        var stanza = RoomIdentifier.AllRoomIdentifiers.FirstOrDefault(r => r.Name == z.RoomName);
                        if (stanza != null)
                        {
                            Vector3 roomPos = stanza.transform.position;
                            Vector3 roomRotation = stanza.transform.eulerAngles;


                            Vector3 rotatedOffset = Quaternion.Euler(roomRotation) * z.Offset ;
                            Vector3 finalPosition = roomPos + rotatedOffset;
                            
                            Pickup P = Pickup.Create(item.ItemType,finalPosition);
                            P.Base.Info.Serial = item.Itemserial;
                            P.Base.Position = finalPosition;
                            ItemDistributor.SpawnPickup(P.Base);
                            
                        }
                    }
                }
            }
        }
        public class ItemEventManager : CustomEventsHandler
        {
            

            public override void OnServerRoundStarted()
            {
                Singleton.TrySpawnItems();
            }

            

            public override void OnPlayerPickedUpItem(PlayerPickedUpItemEventArgs ev)
            {
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.Item.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemPickedUp(ev.Player);
                }
            }

            public override void OnPlayerUsingItem(PlayerUsingItemEventArgs ev)
            {
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.UsableItem.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemUsing(ev.Player);
                }
            }

            public override void OnPlayerShootingWeapon(PlayerShootingWeaponEventArgs ev)
            {
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.FirearmItem.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemUsing(ev.Player);
                }
            }

            public override void OnPlayerShotWeapon(PlayerShotWeaponEventArgs ev)
            {
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.FirearmItem.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemUsed(ev.Player);
                }
            }

            public override void OnPlayerFlippingCoin(PlayerFlippingCoinEventArgs ev)
            {
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.CoinItem.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemUsing(ev.Player);
                }
            }

            public override void OnPlayerFlippedCoin(PlayerFlippedCoinEventArgs ev)
            {
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.CoinItem.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemUsed(ev.Player);
                }
            }

            public override void OnPlayerUsedItem(PlayerUsedItemEventArgs ev)
            {
                
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.UsableItem.Serial).ToList();
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemUsed(ev.Player);
                }
            }
            
            


            public override void OnPlayerChangedItem(PlayerChangedItemEventArgs ev)
            {
                if (ev.NewItem == null) return;
                var z = Singleton.RegisteredItems.Where(i => i.Itemserial == ev.NewItem.Serial);
                if (z.Any())
                {
                    z.FirstOrDefault().OnItemEquipped(ev.Player);
                }
            }

            
        }
        
        
    }
}