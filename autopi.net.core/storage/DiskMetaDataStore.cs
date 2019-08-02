using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace autopi.net.core.storage
{
    public class DiskMetaDataStore : IMetaDataStore
    {
        System.IO.DirectoryInfo _root;

        private string EntityTagFile(Guid entityId)
        {
            return System.IO.Path.Combine(_root.FullName, entityId.ToString()) + ".json";
        }

        private IList<string> LoadTagsForEntity(Guid entityId)
        {
            var file = EntityTagFile(entityId);
            if (!System.IO.File.Exists(file)) return null;
            return JsonConvert.DeserializeObject<IList<string>>(System.IO.File.ReadAllText(file));
        }
        private void SaveTagsForEntitye(Guid entityId, IList<string> tags)
        {
            var file = EntityTagFile(entityId);
            if (System.IO.File.Exists(file)) System.IO.File.Delete(file);
            System.IO.File.WriteAllText(file, JsonConvert.SerializeObject(tags));
        }

        public Task<bool> TagEntity(Guid entityId, string tag, CancellationToken cancellationToken = default)
        {
            var tags = LoadTagsForEntity(entityId);
            if (tags == null) tags = new List<string>();
            if (!tags.Contains(tag)) tags.Add(tag);
            SaveTagsForEntitye(entityId, tags);
            return Task.FromResult(true);
        }
        public Task<IList<string>> GetTagsForEntity(Guid entityId, CancellationToken cancellationToken = default)
        {
            return Task.FromResult(LoadTagsForEntity(entityId));
        }

        public Task Initialize(System.IO.DirectoryInfo root = null)
        {
            if (root == null)
            {
                root = new System.IO.DirectoryInfo(System.IO.Path.Combine(System.Environment.CurrentDirectory, "MetaData"));
            }
            _root = root;
            if (!System.IO.Directory.Exists(root.FullName))
            {
                System.IO.Directory.CreateDirectory(root.FullName);
            }

            return Task.CompletedTask;
        }

    }
}