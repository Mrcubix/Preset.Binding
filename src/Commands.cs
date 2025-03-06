using System.Collections.Generic;
using System.Threading.Tasks;
using OpenTabletDriver.Plugin;
using OpenTabletDriver.Desktop;
using OTD.PresetBinds.IPC;
using System.Diagnostics;

namespace OTD.PresetBinds
{
    public class Commands
    {
        static Commands() => Timer.Start();

        public const int Timeout = 300;
        public static Stopwatch Timer { get; } = new Stopwatch();

        public static async Task ApplyPresetAsync(string name)
        {
            if (Timer.ElapsedMilliseconds > Timeout)
            {
                AppInfo.PresetManager.Refresh();

                var preset = AppInfo.PresetManager.FindPreset(name);

                if (preset != null)
                    await Remote.ApplySettingsAsync(preset.GetSettings(), name);
                else
                    Log.Write("Preset Binding", $"Error: The specified preset ({name}) couldn't be found", LogLevel.Error);
            }

            Timer.Restart();
        }

        public static IReadOnlyCollection<Preset> GetPresets()
        {
            AppInfo.PresetManager.Refresh();

            return AppInfo.PresetManager.GetPresets();
        }
    }
}