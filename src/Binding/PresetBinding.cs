using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Plugin.Tablet;
using OpenTabletDriver.Plugin.Attributes;
using OTD.PresetBinds.IPC;
using System.Linq;
using System.Collections.Generic;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.External.Common.RPC;
using OTD.PresetBinds.Extensions;

namespace OTD.PresetBinds.Binding
{
    [PluginName("Preset")]
    public class PresetBinding: IStateBinding
    {
        static PresetBinding() => Commands.Timer.Start();

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
            if (Selected != null && Remote.IsReady && Remote.Driver.Instance != null)
                Commands.ApplyPreset(Selected);
        }

        public void Release(TabletReference tablet, IDeviceReport report)
        {
            return;
        }

        private static void ConnectToRemote<T>(RpcClient<T> client) where T : class
        {
            if (client.IsConnecting == false && client.IsConnected == false && client.IsAttached == false)
                client.TryConnect();
        }
    }
}