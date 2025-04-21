using OpenTabletDriver.Desktop;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using System.Linq;
using System.Collections.Generic;

namespace OTD.PresetBinds.Binding
{
    [PluginName("Preset")]
    public class PresetBinding: IStateBinding
    {
        private readonly RemotePresetManager _presetManager = new();

        // Fetch presets from the Manager
        public readonly static IReadOnlyCollection<Preset> Presets = RemotePresetManager.GetPresets();
        public static string[] ValidModes => Presets.Select(x => x.Name).ToArray();

        [Property("Selected"), PropertyValidated(nameof(ValidModes))]
        public string? Selected { set; get; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (Selected != null)
                _presetManager.ApplyPreset(Selected);
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            return;
        }
    }
}