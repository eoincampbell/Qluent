namespace Qluent
{
    using Builders;
    /// <summary>
    /// Static Builder class for creating an <see cref="IAzureStorageQueueBuilder{T}" />
    /// </summary>
    public static class Builder
    {
        /// <summary>
        /// Initializes a <see cref="IAzureStorageQueueBuilder{T}" /> for creating an <see cref="IAzureStorageQueue{T}"/>
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <returns>The AzureStorageQueueBuilder</returns>
        public static IAzureStorageQueueBuilder<T> CreateAQueueOf<T>()
        {
            return new AzureStorageQueueBuilder<T>();
        }

        /// <summary>
        /// Initializes a <see cref="IMessageConsumerBuilder{T}" /> for creating an <see cref="IMessageConsumer{T}"/>
        /// </summary>
        /// <typeparam name="T">The object</typeparam>
        /// <returns>The MessageConsumerBulder</returns>
        public static IMessageConsumerBuilder<T> CreateAMessageConsumerFor<T>()
        {
            return new MessageConsumerBuilder<T>();
        }
    }
}