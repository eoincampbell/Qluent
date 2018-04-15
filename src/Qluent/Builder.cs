using Qluent.Builders;

namespace Qluent
{
    public static class Builder
    {
        public static IAzureStorageQueueBuilder<T> CreateAQueueOf<T>()
        {
            return new AzureStorageQueueBuilder<T>();
        }
    }
}