using Avalonia.Controls;
using Avalonia.VisualTree;
using System.Threading.Tasks;
using OOP_Lab4.Models;

namespace OOP_Lab4.Tasks.Task2
{
    public partial class Task2View : UserControl
    {
        public Task2View() { InitializeComponent(); }
        protected override void OnDataContextChanged(System.EventArgs e)
        {
            base.OnDataContextChanged(e);
            if (DataContext is Task2ViewModel vm)
            {
                vm.OpenCompDialog = async (m) => { var d = new CompetitionEditWindow(m); return this.GetVisualRoot() is Window w ? await d.ShowDialog<CompetitionModel>(w) : null; };
                vm.OpenPerfDialog = async (m) => { var d = new PerformanceEditWindow(m); return this.GetVisualRoot() is Window w ? await d.ShowDialog<PerformanceModel>(w) : null; };
            }
        }
    }
}