using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Devices;
using IotDeviceFunctionEX.Models;
using Microsoft.Azure.Devices.Shared;

namespace IotDeviceFunctionEX
{
    public static class ConnectDevice
    {
        private static readonly string iothub = Environment.GetEnvironmentVariable("IotHub");
        private static readonly RegistryManager registryManager =
            RegistryManager.CreateFromConnectionString(Environment.GetEnvironmentVariable("IotHub"));

        [FunctionName("AddDevice")]
        public static async Task<IActionResult> Run(
           [HttpTrigger(AuthorizationLevel.Function, "post", Route = "devices")] HttpRequest req,
           ILogger log)
        {


            
            try
            {
                int interval = 10000;
                var body = JsonConvert.DeserializeObject<HttpDeviceRequest>(await new StreamReader(req.Body).ReadToEndAsync());
                if (string.IsNullOrEmpty(body.DeviceId))
                    return new BadRequestObjectResult(new HttpDeviceResponse("DeviceId is required"));

                var device = await registryManager.GetDeviceAsync(body.DeviceId); 
                if (device == null)
                    device = await registryManager.AddDeviceAsync(new Device(body.DeviceId));

                if (device != null)
                {
                    var twin = await registryManager.GetTwinAsync(device.Id);
                    twin.Properties.Desired["interval"] = 5000;
                    interval = twin.Properties.Desired["interval"];
                    var deviceName = twin.Properties.Reported["deviceName"].ToString();
                    await registryManager.UpdateTwinAsync(twin.DeviceId, twin, twin.ETag);
                }

                var returnDevice = new DeviceItem
                {
                    Interval = interval,
                    DeviceId = body.DeviceId,
                    DeviceConnectionString = $"{iothub.Split(";")[0]};DeviceId={device.Id};SharedAccessKey={device.Authentication.SymmetricKey.PrimaryKey}",
                };

                return new OkObjectResult(returnDevice);

            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new HttpDeviceResponse("Unable to connect the device", ex.Message));
            }
        }
    }
}
