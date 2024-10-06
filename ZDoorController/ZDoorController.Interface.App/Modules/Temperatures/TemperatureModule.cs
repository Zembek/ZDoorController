using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App.Modules.Temperatures
{
    /// <summary>
    /// Core code from: https://www.codeproject.com/Articles/1224347/IoT-Starter-Raspberry-Pi-Thing
    /// </summary>
    public class TemperatureModule : ITemperatureModule
    {
        public string[] Sensors { get; private set; }

        public TemperatureModule(IConfiguration configuration)
        {
            Sensors = configuration.GetSection("TemperatureModule:Sensors").Get<string[]>();
        }


        public double GetTemperature(string sensorId)
        {
            DirectoryInfo deviceDir = new DirectoryInfo($"/sys/bus/w1/devices/{sensorId}");
            if (!deviceDir.Exists)
                throw new ArgumentException($"Sensor {sensorId} doesn't exist");

            var w1slavetext = deviceDir.GetFiles("w1_slave").FirstOrDefault().OpenText().ReadToEnd();
            string temptext = w1slavetext.Split(new string[] { "t=" }, StringSplitOptions.RemoveEmptyEntries)[1];
            return double.Parse(temptext) / 1000;
        }

        public Dictionary<string, double> ListTemperatures()
        {
            throw new NotImplementedException();
        }
    }
}
