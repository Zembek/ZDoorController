﻿using Microsoft.Extensions.Configuration;
using ZDoorController.Interface.App.Modules.Interfaces;

namespace ZDoorController.Interface.App.Modules.Temperatures
{
    /// <summary>
    /// Core code from: https://www.codeproject.com/Articles/1224347/IoT-Starter-Raspberry-Pi-Thing
    /// </summary>
    public class TemperatureModule : ITemperatureModule
    {
        public TemperatureConfiguration Configuration { get; private set; }


        public TemperatureModule(IConfiguration configuration)
        {
            Configuration = configuration.GetSection("Modules:TemperatureModule").Get<TemperatureConfiguration>();
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
            Dictionary<string, double> result = [];

            foreach (string sensor in Configuration.Sensors)
                result.Add(sensor, GetTemperature(sensor));

            return result;
        }
    }
}
