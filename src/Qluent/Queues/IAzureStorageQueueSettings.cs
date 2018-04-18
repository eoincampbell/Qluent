namespace Qluent.Queues
{
    internal interface IAzureStorageQueueSettings
    {
        string ConnectionString { get; set; }
        string StorageQueueName { get; set; }
    }
}
