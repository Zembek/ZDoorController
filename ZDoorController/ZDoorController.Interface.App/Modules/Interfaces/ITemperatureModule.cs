namespace ZDoorController.Interface.App.Modules.Interfaces
{
    public interface ITemperatureModule
    {
        public string[] Sensors { get; }

        public double GetTemperature(string sensorId);
        public Dictionary<string,double> ListTemperatures();
    }
}
