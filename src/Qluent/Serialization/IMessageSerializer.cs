using System;
using System.Collections.Generic;
using System.Text;

namespace Qluent.Serialization
{
        public interface IMessageSerializer<T, K>
        {
            K Serialize(T entity);
            T Deserialize(K message);
        }
}
