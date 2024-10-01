using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ABCRetailApp_Functions.Services
{
    public class QueueStorageService
    {
        private readonly QueueClient _queueClient;
        private readonly ILogger<QueueStorageService> _logger;

        public QueueStorageService(string connectionString, string queueName, ILogger<QueueStorageService> logger)
        {
            _queueClient = new QueueClient(connectionString, queueName);
            _queueClient.CreateIfNotExists();
            _logger = logger;
        }

        public async Task SendMessageAsync(string message)
        {
            try
            {
                if (!string.IsNullOrEmpty(message))
                {
                    await _queueClient.SendMessageAsync(message);  // Sends the message to the Azure queue
                    _logger.LogInformation("Message sent to queue successfully.");
                }
                else
                {
                    _logger.LogWarning("Message is null or empty.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message to queue.");
                throw;
            }
        }

        public async Task<QueueMessage[]> ReceiveMessagesAsync(int maxMessages = 10)
        {
            try
            {
                var response = await _queueClient.ReceiveMessagesAsync(maxMessages);
                return response.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error receiving messages from the queue.");
                throw new ApplicationException("Could not receive messages from the queue.", ex);
            }
        }

        public async Task DeleteMessageAsync(string messageId, string popReceipt)
        {
            try
            {
                await _queueClient.DeleteMessageAsync(messageId, popReceipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting message from the queue.");
                throw new ApplicationException("Could not delete message from the queue.", ex);
            }
        }

    }
}
