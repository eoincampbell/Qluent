namespace Qluent.Queues
{
    internal class AzureStorageQueueSettings : IMessageConsumerSettings
    {
        public string ConnectionString { get; set; } = "UseDevelopmentStorage=true";
        public string StorageQueueName { get; set; } = null;
    }
}
