using System;
using System.Threading.Tasks;
using autopi.net.core;
using autopi.net.core.API;
using autopi.net.core.auth;
using autopi.net.core.auth.API;
using autopi.net.core.Models;
using autopi.net.core.storage;
using Newtonsoft.Json;

namespace autopi.net.console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var startup = new Startup();
            await startup.Initialize();

            var metaDataStorage = new DiskMetaDataStore() as IMetaDataStore;
            await metaDataStorage.Initialize();


            Credentials credentials = null;
            try
            {
                var credentialsFile = "../../autopi.net.credentials.json";
                if (System.IO.File.Exists(credentialsFile))
                {
                    credentials = JsonConvert.DeserializeObject<Credentials>(await System.IO.File.ReadAllTextAsync(credentialsFile));
                }
            }
            catch
            {

            }
            var auth = new AuthManager(AutoPiApiClient.Client, startup.Logger);
            var loginResult = await auth.CreateLogin(credentials);

            if (loginResult != null && !string.IsNullOrEmpty(loginResult.Token))
            {
                Console.WriteLine("Login successfull!");
                var logBookManager = new LogBookManager(AutoPiApiClient.Client, startup.Logger);

                Console.WriteLine("Attempting to retreive the list logbook fields");
                var fields = await logBookManager.GetStorageFields();
                foreach (var field in fields)
                {
                    Console.WriteLine("{1} - {0}", field.Field, field.Type);
                }

                Console.WriteLine("Attempting to retreive the list of dongles");

                var dm = new DongleManager(AutoPiApiClient.Client, startup.Logger);
                var dongles = await dm.GetDongleDevices();

                if (dongles == null)
                {
                    Console.WriteLine("No dongles found.");
                    return;
                }
                foreach (var dongle in dongles)
                {

                    Console.WriteLine("Call Name:{0}, Display:{1}, Last Communication:{2}", dongle.CallName, dongle.Display, dongle.LastCommunication);
                    if (dongle.Vehicle != null)
                    {
                        Console.WriteLine("\tVehicle Call Name:{0}, Vin:{1}", dongle.Vehicle.CallName, dongle.Vehicle.Vin);
                    }
                    Console.WriteLine("Attempting to retreive the list of trips");
                    var trips = await logBookManager.GetTrips(dongle.Id);
                    if (trips == null)
                    {
                        Console.WriteLine("No trips were found for this device.");
                        continue;
                    }
                    foreach (var trip in trips)
                    {
                        Console.WriteLine("\t\tStart:{2} at {0}, End:{3} at {1}, Distance (km):{4:0.0}, Duration:{5}", trip.StartTimeUtc, trip.EndTimeUtc, trip.StartDisplay, trip.EndDisplay, trip.DistanceKm, trip.DurationTS);

                    }

                    //var readFuelLevel = await logBookManager.GetStorageRead(dongle.Id, DateTime.Now.AddHours(-24), field: "obd.fuel_level.value");
                    //var raw = await logBookManager.GetStorageRaw(dongle.Id, DateTime.Now.AddHours(-24));
                    //var position = await logBookManager.GetStorageRaw(dongle.Id, DateTime.Now.AddHours(-24), field: "track.pos.loc");

                }



            }
        }
    }
}
