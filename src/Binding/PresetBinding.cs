using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using OTD.PresetBinds.IPC;
using System.Linq;

namespace OTD.PresetBinds.Binding
{
    [PluginName("Preset")]
    public class PresetBinding: IStateBinding
    {
        public PresetBinding()
        {
            Remote.Driver.Connected += (s, e) => 
            {
                Remote.Connected = true;
            };

            if (!Remote.Connected)
                _ = Task.Run(() => Remote.Driver.Connect());
        }

        public static string[] ValidModes => Commands.GetPresets().Select(x => x.Name).ToArray();

        [Property("Selected"), PropertyValidated(nameof(ValidModes))]
        public string? Selected { set; get; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (Selected != null && Remote.Connected)
                _ = Task.Run(() => Commands.ApplyPresetAsync(Selected));
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            return;
        }
    }
}