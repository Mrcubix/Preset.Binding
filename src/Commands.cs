using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Desktop;
using OTD.PresetBinds.IPC;
using System;

namespace OTD.PresetBinds
{
    public class Commands
    {
        public static async Task ApplyPresetAsync(string name)
        {
            AppInfo.PresetManager.Refresh();

            var preset = AppInfo.PresetManager.FindPreset(name);

            if (preset != null)
                await Remote.ApplySettingsAsync(preset.GetSettings(), name);
            else
                Log.Write("OTD Presets", $"Error: The specified preset ({name}) couldn't be found", LogLevel.Error);
        }

        public static IReadOnlyCollection<Preset> GetPresets()
        {
            AppInfo.PresetManager.Refresh();

            return AppInfo.PresetManager.GetPresets();
        }
    }
}