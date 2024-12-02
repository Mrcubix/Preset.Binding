using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using OTD.PresetBinds.IPC;
using System.Linq;
using System.Collections.Generic;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.External.Common.RPC;

namespace OTD.PresetBinds.Binding
{
    [PluginName("Preset")]
    public class PresetBinding: IStateBinding
    {
        public PresetBinding()
        {
            Remote.UX.Disconnected += (s, e) => ConnectToUX(Remote.UX);

            ConnectToUX(Remote.Driver);
            ConnectToUX(Remote.UX);
        }

        public readonly static IReadOnlyCollection<Preset> Presets = Commands.GetPresets();
        public static string[] ValidModes => Presets.Select(x => x.Name).ToArray();

        [Property("Selected"), PropertyValidated(nameof(ValidModes))]
        public string? Selected { set; get; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (Selected != null && Remote.Driver.IsConnected)
                _ = Task.Run(() => Commands.ApplyPresetAsync(Selected));
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            return;
        }

        private static void ConnectToUX<T>(RpcClient<T> client) where T : class
        {
            if (client.IsConnected == false)
                _ = Task.Run(() => client.ConnectAsync());
        }
    }
}