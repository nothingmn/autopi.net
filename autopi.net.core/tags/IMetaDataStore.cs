using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace autopi.net.core.tags
{
    //A generic method to store meta data in autopi entities
    //BYOD - bring your own driver...
    public interface IMetaDataStore
    {

        Task<bool> TagEntity(Guid entityId, string tag, CancellationToken cancellationToken = default(CancellationToken));
        Task<IList<string>> GetTagsForEntity(Guid entityId, CancellationToken cancellationToken = default(CancellationToken));

        Task Initialize(System.IO.DirectoryInfo root = null);
    }
}