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
            ConnectToRemote(Remote.Driver);
            ConnectToRemote(Remote.UX);
        }

        public readonly static IReadOnlyCollection<Preset> Presets = Commands.GetPresets();
        public static string[] ValidModes => Presets.Select(x => x.Name).ToArray();

        [Property("Selected"), PropertyValidated(nameof(ValidModes))]
        public string? Selected { set; get; }

        public void Press(TabletReference tablet, IDeviceReport report)
        {
            if (Selected != null && Remote.Driver.IsAttached)
                _ = Task.Run(() => Commands.ApplyPresetAsync(Selected));
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            return;
        }

        private static void ConnectToRemote<T>(RpcClient<T> client) where T : class
        {
            if (client.IsConnecting == false && client.IsConnected == false)
                _ = Task.Run(() => client.ConnectAsync());
        }
    }
}