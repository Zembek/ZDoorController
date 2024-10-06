using System;

namespace RPiButtons.SSD1306.Common
{
    public class DisplayConfiguration
    {
        public bool ProceedOnFail { get; set; }
        public byte DisplayAddress { get; set; }
        public Action FuncToCall { get; set; }
    }
}
