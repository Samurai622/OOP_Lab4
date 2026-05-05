using Avalonia.Controls;

namespace OOP_Lab4
{
    public partial class JsonWindow : Window
    {
        public JsonWindow() { InitializeComponent(); }
        
        public JsonWindow(string jsonData) : this()
        {
            JsonTextBox.Text = jsonData;
        }
    }
}