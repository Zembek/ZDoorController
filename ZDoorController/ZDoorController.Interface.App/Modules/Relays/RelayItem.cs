namespace ZDoorController.Interface.App.Modules.Relays
{
    public class RelayItem
    {
        public string Name { get; set; }
        public int Pin { get; set; }
        public bool IsActiveLow { get; set; }

        public bool IsActive { get; set; }
    }
}
