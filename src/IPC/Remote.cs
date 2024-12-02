using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.Plugin;
using OTD.UX.Remote.Lib;
using System;
using System.Threading.Tasks;

namespace OTD.PresetBinds.IPC
{
    public class Remote
    {
        public static bool Connected { get; set; } = false;

        public static RpcClient<IDriverDaemon> Driver { get; } = new("OpenTabletDriver.Daemon");
        public static RpcClient<IUXRemote> UX { get; } = new("OTD.UX.Remote");

        public static async Task ApplySettingsAsync(Settings settings, string name)
        {
            try
            {
                await Driver.Instance.SetSettings(settings);

                Log.Debug("OTD Presets", $"Switched to '{name}' preset");

                if (UX.IsConnected && UX.Instance != null)
                {
                    await UX.Instance.Synchronize();
                    await UX.Instance.SendNotification("Preset Binding", $"Switched to '{name}' preset");
                }
                else
                    Log.Write("OTD Presets", $"Switched to '{name}' preset", LogLevel.Info, false, true);
            }
            catch (Exception e)
            {
                Log.Write("OTD Presets", $"Error: {e}", LogLevel.Error);
            }
        }
    }
}
