using System;
using System.Threading.Tasks;
using OpenTabletDriver.External.Common.RPC;
using OpenTabletDriver.Plugin;

namespace OTD.PresetBinds.Extensions
{
    public static class RpcClientExtensions
    {
        public static void TryConnect<T>(this RpcClient<T> client) where T : class
        {
            try
            {
                _ = Task.Run(() => client.ConnectAsync());
            }
            catch (Exception e)
            {
                Log.Write("Preset Binding", $"An Error occured while connecting to the Remote", LogLevel.Error);
                Log.Write("Preset Binding", $"Error: {e}", LogLevel.Error);
            }
        }
    }
}