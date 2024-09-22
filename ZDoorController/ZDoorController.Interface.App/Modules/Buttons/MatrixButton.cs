namespace ZDoorController.Interface.App.Modules.Buttons
{
    public class MatrixButton
    {
        public int ColumnPin { get; set; }
        public int RowPin { get; set; }
        public string Name { get; set; }

        public MatrixButton(string name, int columnPin, int rowPin)
        {
            Name = name;
            ColumnPin = columnPin;
            RowPin = rowPin;
        }
    }
}
