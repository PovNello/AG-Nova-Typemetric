using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Typemetric.UI.Controls
{
    public partial class VisualMouse : UserControl
    {
        public VisualMouse()
        {
            InitializeComponent();
        }

        public void UpdateHeatmap(Dictionary<string, int> freqs)
        {
            int max = freqs.Values.DefaultIfEmpty(0).Max();
            
            BtnLeft.Background = HeatmapRenderer.GetColorForFrequency(freqs.GetValueOrDefault("Left", 0), max);
            BtnRight.Background = HeatmapRenderer.GetColorForFrequency(freqs.GetValueOrDefault("Right", 0), max);
            BtnMiddle.Background = HeatmapRenderer.GetColorForFrequency(freqs.GetValueOrDefault("Middle", 0), max);
        }
    }
}
