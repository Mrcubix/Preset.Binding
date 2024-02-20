using OpenTabletDriver.Desktop;
using OpenTabletDriver.Desktop.Contracts;
using OpenTabletDriver.Desktop.RPC;
using System.Threading.Tasks;

namespace OTD.PresetBinds.IPC
{
    public class Remote
    {
        public static bool Connected { get; set; } = false;
        
        public static readonly RpcClient<IDriverDaemon> Driver = new RpcClient<IDriverDaemon>("OpenTabletDriver.Daemon");

        public static async Task ApplySettingsAsync(Settings settings)
        {
            await Driver.Instance.SetSettings(settings);
        }
    }
}
