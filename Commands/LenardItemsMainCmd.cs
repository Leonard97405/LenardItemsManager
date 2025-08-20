using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommandSystem;
using LabApi.Features.Wrappers;

namespace LenardItemsManager
{
    [CommandHandler(typeof(RemoteAdminCommandHandler))]
    public class LenardItemsMainCmd : ParentCommand
    {
        public override void LoadGeneratedCommands()
        {
        }

        protected override bool ExecuteParent(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            response = "LeonardItemsManager available commands:";
            response += "\n-list <- Fornisce una lista di tutti gli item custom";
            response += "\n-give <itemid> <playerid> <- Da un item custom al player selezionato";
            return false;
        }

        public override string Command { get; } = "itemManager";
        public override string[] Aliases { get; } = { "il", "leonardmanager", "lm" };
        public override string Description { get; } = "Parent command del role manager";
    }
    [CommandHandler(typeof(LenardItemsMainCmd))]
    public class GiveItemCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            string raw = string.Join(" ", arguments);
            List<string> args = TrovaArgomenti(raw);
            if (args.Count != 2)
            {
                response = "Format del comando: give <itemid> <playerid>";
                return false;
            } else if (Player.TryGet(sender, out var y) && !y.HasPermission(PlayerPermissions.GivingItems))
            {
                response =
                    "Non hai il permesso di usare questo comando. Permesso richiesto PlayerPermissions.GivingItems";
                return false;
            } else if (int.TryParse(args[1], out var pId) && Player.TryGet(pId, out var p))
            {
                CustomItem z = ItemsManager.Singleton.RegisteredItems.FirstOrDefault(c => c.ItemId == args[0]);
                if (z == null)
                {
                    response = "Item non trovato";
                    return false;
                }
                LenardItemsManager.ItemsManager.Singleton.SpawnItem(z, p: p);
                response = "Item aggiunto";
                return true;
            }

            response = "Player non valido";
            return false;
        }

        public static List<string> TrovaArgomenti(string input)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(input,
                @"(?<=^|\s)(?:""(?<q>[^""]*)""|(?<w>[^\s""]+))");

            return matches
                .Cast<Match>()    
                .Select(m => m.Groups["q"].Success ? m.Groups["q"].Value : m.Groups["w"].Value)
                .ToList();
        }
        public string Command { get; } = "give";
        public string[] Aliases { get; } = { "gv" };
        public string Description { get; } = "Da un item custom in base all'id inserito";
    }
    [CommandHandler(typeof(LenardItemsMainCmd))]
    public class ListItemsCommand : ICommand
    {
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Player.TryGet(sender, out var p) && !p.HasPermission(PlayerPermissions.GivingItems))
            {
                response =
                    "Non hai il permesso per eseguire questo comando. Permesso richiesto: PlayerPermissions.GivingItems";
                return false;
            }
            response = "\nItem custom:";
            foreach (var z in LenardItemsManager.ItemsManager.Singleton.RegisteredItems)
            {
                response += "\n" + z.ItemId + ": " + z.ItemDesc;
            }

            return true;
        }

        public string Command { get; } = "list";
        public string[] Aliases { get; } = { "l", "il", "itemlist" };
        public string Description { get; } = "Comando per dare una lista degli item custom";
    }
}