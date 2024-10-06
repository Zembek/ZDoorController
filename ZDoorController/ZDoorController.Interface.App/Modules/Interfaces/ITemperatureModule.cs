using ZDoorController.Interface.App.Modules.Temperatures;

namespace ZDoorController.Interface.App.Modules.Interfaces
{
    public interface ITemperatureModule
    {
        public TemperatureConfiguration Configuration { get; }

        public double GetTemperature(string sensorId);
        public Dictionary<string,double> ListTemperatures();
    }
}
