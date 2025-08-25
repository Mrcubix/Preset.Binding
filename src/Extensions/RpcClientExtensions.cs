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
            _ = client.TryConnectAsync();
        }

        public static async Task TryConnectAsync<T>(this RpcClient<T> client) where T : class
        {
            try
            {
                if (client.IsConnecting == false && client.IsConnected == false && client.IsAttached == false)
                    await client.ConnectAsync();
            }
            catch (Exception e)
            {
                Log.Write("Preset Binding", $"An Error occured while connecting to the Remote", LogLevel.Error);
                Log.Write("Preset Binding", $"Error: {e}", LogLevel.Error);
            }
        }
        public static bool EnsureConnection<T>(this RpcClient<T> client) where T : class
        {
            return EnsureConnectionAsync(client).GetAwaiter().GetResult();
        }

        public static async Task<bool> EnsureConnectionAsync<T>(this RpcClient<T> client, int connectionTimeout = 2000) where T : class
        {
            var timeout = Task.Delay(connectionTimeout);
            var result = await Task.WhenAny(client.TryConnectAsync(), timeout);

            return result != timeout && client.IsAttached;
        }
    }
}