using System;
using CustomItemTemplate;
using LabApi.Events.CustomHandlers;
using LabApi.Features;
using LabApi.Loader.Features.Plugins;

namespace CustomItemTemplate
{
    public class Plugin : Plugin<Config>
    {
        public override void Enable()
        {
            LenardItemsManager.ItemsManager.Singleton.RegisterItem(Config.cI);
            CustomHandlersManager.RegisterEventsHandler(new ItemEvents());
        }

        public override void Disable()
        {
            CustomHandlersManager.UnregisterEventsHandler(new ItemEvents());
        }

        public override string Name { get; } = "CustomItemTemplate";
        public override string Description { get; } = "Template per item custom";
        public override string Author { get; } = "Lenard";
        public override Version Version { get; } = new Version(1, 0, 0);
        public override Version RequiredApiVersion { get; } = new Version(LabApiProperties.CompiledVersion);
    }
}

public class Config
{
    public CustomTemplate cI = new CustomTemplate();
}