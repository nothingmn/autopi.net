using System.Globalization;
using System.Threading.Tasks;
using autopi.net.core.tags;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace autopi.net.core
{
    public class Startup
    {
        public async Task Initialize()
        {
            JsonConvert.DefaultSettings = () => new JsonSerializerSettings
            {
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore,
                Converters = {
                    new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
                },
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            //some ugly stuff to have here, should consider leveraging DI for this stuff
            Logger = new Logger();
            MetaDataStorage = new DiskMetaDataStore() as IMetaDataStore;
            await MetaDataStorage.Initialize();

        }
        public ILogger Logger { get; private set; }
        public IMetaDataStore MetaDataStorage { get; private set; }
    }
}