using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.Plugin;
using OTD.PresetBinds.Extensions;
using OTD.UX.Remote.Lib;
using StreamJsonRpc;

namespace OTD.PresetBinds
{
    public class RemotePresetManager
    {
        #region Constants

        private const string PLUGIN_GROUP = "Preset Binding";
        private const int Timeout = 200;

        private static Stopwatch Timer { get; } = new Stopwatch();
        private static PresetManager PresetManager => AppInfo.PresetManager;

        #endregion

        public RemotePresetManager() => Timer.Start();

        #region RPC Clients

        public static RpcClient<IDriverDaemon> Driver { get; } = new("OpenTabletDriver.Daemon");
        public static RpcClient<IUXRemote> UX { get; } = new("OTD.UX.Remote");

        #endregion

        #region Methods

        public void ApplyPreset(string name)
        {
            // Prevent spamming as the clients might not be ready
            if (Timer.ElapsedMilliseconds > Timeout)
            {
                PresetManager.Refresh();

                var preset = PresetManager.FindPreset(name);

                if (preset != null)
                    _ = TryApplyPreset(preset);
                else
                    Log.Write(PLUGIN_GROUP, $"Error: The specified preset ({name}) couldn't be found", LogLevel.Error);
            }

            Timer.Restart();
        }

        private static async Task TryApplyPreset(Preset preset)
        {
            // The driver might not be connected yet
            if (Driver.EnsureConnection() == false)
            {
                Log.Write(PLUGIN_GROUP, "An attempt to Apply a Preset failed: Driver is not connected", LogLevel.Error);
                return;
            }

            bool isSuccess = true;

            try
            {
                await Driver.Instance.SetSettings(preset.GetSettings());
                Log.Write(PLUGIN_GROUP, $"Switched to '{preset.Name}' preset", LogLevel.Info, false, true);
            }
            catch (Exception e)
            {
                HandleException(e, "Applying Settings");
                isSuccess = false;
            }

            // Synchronize the UX if it's attached
            if (isSuccess)
                _ = Task.Run(TrySynchronizingUX);
        }

        private static async Task TrySynchronizingUX()
        {
            // UX.Remote might not be installed or ready
            if (await UX.EnsureConnectionAsync() == false)
                return;

            try
            {
                await UX.Instance.Synchronize();
            }
            catch (Exception e)
            {
                HandleException(e, "Synchronizing Settings in the UX");
            }
        }

        #endregion

        #region Static Methods

        public static IReadOnlyCollection<Preset> GetPresets()
        {
            PresetManager.Refresh();
            return PresetManager.GetPresets();
        }

        private static void HandleException(Exception e, string lastAction = "")
        {
            if ((e is ConnectionLostException) || (e is InvalidOperationException && e.Message.Contains("listening")))
                return;

            Log.Write(PLUGIN_GROUP, $"An Error occured while {lastAction}", LogLevel.Error);
            Log.Write(PLUGIN_GROUP, $"Error: {e}", LogLevel.Error);
        }

        #endregion
    }
}