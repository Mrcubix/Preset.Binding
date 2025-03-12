using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Desktop;
using OTD.PresetBinds.IPC;
using System.Diagnostics;
using System;

namespace OTD.PresetBinds
{
    public class Commands
    {
        public const int Timeout = 200;

        public static Stopwatch Timer { get; } = new Stopwatch();

        public static void ApplyPreset(string name)
        {
            if (Timer.ElapsedMilliseconds > Timeout)
            {
                AppInfo.PresetManager.Refresh();

                var preset = AppInfo.PresetManager.FindPreset(name);

                if (preset != null)
                    _ = TryApplyPresetAsync(preset);
                else
                    Log.Write("Preset Binding", $"Error: The specified preset ({name}) couldn't be found", LogLevel.Error);
            }

            Timer.Restart();
        }

        public static async Task TryApplyPresetAsync(Preset preset)
        {
            try
            {
                await Remote.ApplySettingsAsync(preset.GetSettings(), preset.Name);
            }
            catch (Exception e)
            {
                Log.Write("Preset Binding", $"An Error occured while applying the preset", LogLevel.Error);
                Log.Write("Preset Binding", $"Error: {e}", LogLevel.Error);
            }
        }

        public static IReadOnlyCollection<Preset> GetPresets()
        {
            AppInfo.PresetManager.Refresh();

            return AppInfo.PresetManager.GetPresets();
        }
    }
}