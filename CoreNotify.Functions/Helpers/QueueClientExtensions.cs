using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CoreNotify.Functions.Helpers
{
    public static class QueueClientExtensions
    {
        public static async Task<SendReceipt> SendJsonAsync<T>(this QueueClient queueClient, T @object)
        {
            var json = JsonSerializer.Serialize(@object);
            return await SendAsync(queueClient, json);
        }

        public static async Task<SendReceipt> SendAsync(this QueueClient queueClient, string message)
        {
            var bytes = Encoding.UTF8.GetBytes(message);
            var base64string = Convert.ToBase64String(bytes);
            return await queueClient.SendMessageAsync(base64string);
        }
    }
}
