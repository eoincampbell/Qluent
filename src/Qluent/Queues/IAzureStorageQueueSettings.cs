namespace Qluent.Queues
{
    internal interface IMessageConsumerSettings
    {
        string ConnectionString { get; set; }
        string StorageQueueName { get; set; }
    }
}
