// ReSharper disable once CheckNamespace
namespace Qluent.Policies.PoisonMessageBehavior
{
    /// <summary>
    /// An enumeration that specifies whether exceptions should be thrown or swallowed when a poison message is identified
    /// </summary>
    public enum By
    {
        /// <summary>
        /// Specifies that exceptions should be thrown
        /// </summary>
        ThrowingExceptions,
        /// <summary>
        /// The swallowing exceptions
        /// </summary>
        SwallowingExceptions
    }
}