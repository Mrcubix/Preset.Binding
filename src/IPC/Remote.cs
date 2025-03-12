using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.Plugin;
using OTD.UX.Remote.Lib;
using StreamJsonRpc;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace OTD.PresetBinds.IPC
{
    public class Remote
    {
        /*static Remote()
        {
            Driver.Disconnected += OnClientDisconnected<IDriverDaemon>;
            UX.Disconnected += OnClientDisconnected<IUXRemote>;
        }*/

        public static bool IsReady => Driver.IsAttached;

        public static RpcClient<IDriverDaemon> Driver { get; } = new("OpenTabletDriver.Daemon");
        public static RpcClient<IUXRemote> UX { get; } = new("OTD.UX.Remote");

        public static async Task ApplySettingsAsync(Settings settings, string name)
        {
            bool isSuccess = true;

            try
            {
                await Driver.Instance.SetSettings(settings);
            }
            catch (Exception e)
            {
                HandleException(e, "Applying Settings");
                isSuccess = false;
            }

            if (isSuccess && UX.IsAttached && UX.Instance != null)
                await HandleUXAsync(name);
            else
                Log.Write("Preset Binding", $"Switched to '{name}' preset", LogLevel.Info, false, true);

        }

        private static async Task HandleUXAsync(string name)
        {
            Log.Debug("Preset Binding", $"Switched to '{name}' preset");

            try
            {
                await UX.Instance.Synchronize();
                await UX.Instance.SendNotification("Preset Binding", $"Switched to '{name}' preset");
            }
            catch (Exception e)
            {
                HandleException(e, "Synchronizing Settings in the UX");
            }
        }

        private static void HandleException(Exception e, string lastAction = "")
        {
            if ((e is ConnectionLostException) || (e is InvalidOperationException && e.Message.Contains("listening")))
                return;

            Log.Write("Preset Binding", $"An Error occured while {lastAction}", LogLevel.Error);
            Log.Write("Preset Binding", $"Error: {e}", LogLevel.Error);
        }

        /*private static void OnClientDisconnected<T>(object? sender, EventArgs e) where T : class
        {
            if (sender is RpcClient<T> client && client.IsConnected == false && client.IsConnecting == false)
                _ = Task.Run(() => UX.ConnectAsync());
        }*/
    }
}
