using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Typemetric.UI.Controls
{
    public partial class VisualKeyboard : UserControl
    {
        private Dictionary<Key, Border> _keyBorders = new Dictionary<Key, Border>();

        public class KeyDef
        {
            public string Label { get; set; }
            public Key WpfKey { get; set; } = Key.None;
            public double WidthMult { get; set; } = 1.0;
            public double HeightMult { get; set; } = 1.0;
            public double MarginRight { get; set; } = 2.0;
            public double MarginBottom { get; set; } = 2.0;
        }

        public VisualKeyboard()
        {
            InitializeComponent();
            BuildKeyboard();
        }

        private void BuildKeyboard()
        {
            var r1 = new[] {
                new KeyDef { Label = "ESC", WpfKey = Key.Escape, MarginRight = 20 },
                new KeyDef { Label = "F1", WpfKey = Key.F1 }, new KeyDef { Label = "F2", WpfKey = Key.F2 }, new KeyDef { Label = "F3", WpfKey = Key.F3 }, new KeyDef { Label = "F4", WpfKey = Key.F4, MarginRight = 10 },
                new KeyDef { Label = "F5", WpfKey = Key.F5 }, new KeyDef { Label = "F6", WpfKey = Key.F6 }, new KeyDef { Label = "F7", WpfKey = Key.F7 }, new KeyDef { Label = "F8", WpfKey = Key.F8, MarginRight = 10 },
                new KeyDef { Label = "F9", WpfKey = Key.F9 }, new KeyDef { Label = "F10", WpfKey = Key.F10 }, new KeyDef { Label = "F11", WpfKey = Key.F11 }, new KeyDef { Label = "F12", WpfKey = Key.F12 }
            };
            var r2 = new[] {
                new KeyDef { Label = "`", WpfKey = Key.OemTilde }, new KeyDef { Label = "1", WpfKey = Key.D1 }, new KeyDef { Label = "2", WpfKey = Key.D2 }, new KeyDef { Label = "3", WpfKey = Key.D3 },
                new KeyDef { Label = "4", WpfKey = Key.D4 }, new KeyDef { Label = "5", WpfKey = Key.D5 }, new KeyDef { Label = "6", WpfKey = Key.D6 }, new KeyDef { Label = "7", WpfKey = Key.D7 },
                new KeyDef { Label = "8", WpfKey = Key.D8 }, new KeyDef { Label = "9", WpfKey = Key.D9 }, new KeyDef { Label = "0", WpfKey = Key.D0 }, new KeyDef { Label = "-", WpfKey = Key.OemMinus },
                new KeyDef { Label = "=", WpfKey = Key.OemPlus }, new KeyDef { Label = "BACK", WpfKey = Key.Back, WidthMult = 2.0 }
            };
            var r3 = new[] {
                new KeyDef { Label = "TAB", WpfKey = Key.Tab, WidthMult = 1.5 }, new KeyDef { Label = "Q", WpfKey = Key.Q }, new KeyDef { Label = "W", WpfKey = Key.W }, new KeyDef { Label = "E", WpfKey = Key.E },
                new KeyDef { Label = "R", WpfKey = Key.R }, new KeyDef { Label = "T", WpfKey = Key.T }, new KeyDef { Label = "Y", WpfKey = Key.Y }, new KeyDef { Label = "U", WpfKey = Key.U },
                new KeyDef { Label = "I", WpfKey = Key.I }, new KeyDef { Label = "O", WpfKey = Key.O }, new KeyDef { Label = "P", WpfKey = Key.P }, new KeyDef { Label = "[", WpfKey = Key.OemOpenBrackets },
                new KeyDef { Label = "]", WpfKey = Key.OemCloseBrackets }, new KeyDef { Label = "\\", WpfKey = Key.OemPipe, WidthMult = 1.5 }
            };
            var r4 = new[] {
                new KeyDef { Label = "CAPS", WpfKey = Key.CapsLock, WidthMult = 1.75 }, new KeyDef { Label = "A", WpfKey = Key.A }, new KeyDef { Label = "S", WpfKey = Key.S }, new KeyDef { Label = "D", WpfKey = Key.D },
                new KeyDef { Label = "F", WpfKey = Key.F }, new KeyDef { Label = "G", WpfKey = Key.G }, new KeyDef { Label = "H", WpfKey = Key.H }, new KeyDef { Label = "J", WpfKey = Key.J },
                new KeyDef { Label = "K", WpfKey = Key.K }, new KeyDef { Label = "L", WpfKey = Key.L }, new KeyDef { Label = ";", WpfKey = Key.OemSemicolon }, new KeyDef { Label = "'", WpfKey = Key.OemQuotes },
                new KeyDef { Label = "ENTER", WpfKey = Key.Enter, WidthMult = 2.25 }
            };
            var r5 = new[] {
                new KeyDef { Label = "SHIFT", WpfKey = Key.LeftShift, WidthMult = 2.25 }, new KeyDef { Label = "Z", WpfKey = Key.Z }, new KeyDef { Label = "X", WpfKey = Key.X }, new KeyDef { Label = "C", WpfKey = Key.C },
                new KeyDef { Label = "V", WpfKey = Key.V }, new KeyDef { Label = "B", WpfKey = Key.B }, new KeyDef { Label = "N", WpfKey = Key.N }, new KeyDef { Label = "M", WpfKey = Key.M },
                new KeyDef { Label = ",", WpfKey = Key.OemComma }, new KeyDef { Label = ".", WpfKey = Key.OemPeriod }, new KeyDef { Label = "/", WpfKey = Key.OemQuestion }, new KeyDef { Label = "SHIFT", WpfKey = Key.RightShift, WidthMult = 2.75 }
            };
            var r6 = new[] {
                new KeyDef { Label = "CTRL", WpfKey = Key.LeftCtrl, WidthMult = 1.25 }, new KeyDef { Label = "WIN", WpfKey = Key.LWin, WidthMult = 1.25 }, new KeyDef { Label = "ALT", WpfKey = Key.LeftAlt, WidthMult = 1.25 },
                new KeyDef { Label = "SPACE", WpfKey = Key.Space, WidthMult = 6.25 },
                new KeyDef { Label = "ALT", WpfKey = Key.RightAlt, WidthMult = 1.25 }, new KeyDef { Label = "WIN", WpfKey = Key.RWin, WidthMult = 1.25 }, new KeyDef { Label = "MENU", WpfKey = Key.Apps, WidthMult = 1.25 },
                new KeyDef { Label = "CTRL", WpfKey = Key.RightCtrl, WidthMult = 1.25 }
            };

            var n1 = new[] { new KeyDef { Label = "PRT", WpfKey = Key.PrintScreen }, new KeyDef { Label = "SCR", WpfKey = Key.Scroll }, new KeyDef { Label = "PAU", WpfKey = Key.Pause } };
            var n2 = new[] { new KeyDef { Label = "INS", WpfKey = Key.Insert }, new KeyDef { Label = "HM", WpfKey = Key.Home }, new KeyDef { Label = "PUP", WpfKey = Key.PageUp } };
            var n3 = new[] { new KeyDef { Label = "DEL", WpfKey = Key.Delete }, new KeyDef { Label = "END", WpfKey = Key.End }, new KeyDef { Label = "PDN", WpfKey = Key.PageDown } };
            var n4 = new KeyDef[0];
            var n5 = new[] { new KeyDef { Label = "", WpfKey = Key.None, WidthMult=1 }, new KeyDef { Label = "▲", WpfKey = Key.Up }, new KeyDef { Label = "", WpfKey = Key.None, WidthMult=1 } };
            var n6 = new[] { new KeyDef { Label = "◄", WpfKey = Key.Left }, new KeyDef { Label = "▼", WpfKey = Key.Down }, new KeyDef { Label = "►", WpfKey = Key.Right } };

            var num1 = new KeyDef[0];
            var num2 = new[] { new KeyDef { Label = "NLCK", WpfKey = Key.NumLock }, new KeyDef { Label = "/", WpfKey = Key.Divide }, new KeyDef { Label = "*", WpfKey = Key.Multiply }, new KeyDef { Label = "-", WpfKey = Key.Subtract } };
            var num3 = new[] { new KeyDef { Label = "7", WpfKey = Key.NumPad7 }, new KeyDef { Label = "8", WpfKey = Key.NumPad8 }, new KeyDef { Label = "9", WpfKey = Key.NumPad9 }, new KeyDef { Label = "+", WpfKey = Key.Add, HeightMult=2.06, MarginBottom=-28 } };
            var num4 = new[] { new KeyDef { Label = "4", WpfKey = Key.NumPad4 }, new KeyDef { Label = "5", WpfKey = Key.NumPad5 }, new KeyDef { Label = "6", WpfKey = Key.NumPad6 } };
            var num5 = new[] { new KeyDef { Label = "1", WpfKey = Key.NumPad1 }, new KeyDef { Label = "2", WpfKey = Key.NumPad2 }, new KeyDef { Label = "3", WpfKey = Key.NumPad3 }, new KeyDef { Label = "ENT", WpfKey = Key.Return, HeightMult=2.06, MarginBottom=-28 } };
            var num6 = new[] { new KeyDef { Label = "0", WpfKey = Key.NumPad0, WidthMult=2.0 }, new KeyDef { Label = ".", WpfKey = Key.Decimal } };


            var mainPanel = new StackPanel { Orientation = Orientation.Horizontal };
            
            var leftBlock = new StackPanel { Margin = new Thickness(0,0,10,0) };
            leftBlock.Children.Add(CreateRow(r1, true));
            leftBlock.Children.Add(CreateRow(r2));
            leftBlock.Children.Add(CreateRow(r3));
            leftBlock.Children.Add(CreateRow(r4));
            leftBlock.Children.Add(CreateRow(r5));
            leftBlock.Children.Add(CreateRow(r6));

            var midBlock = new StackPanel { Margin = new Thickness(0,0,10,0) };
            midBlock.Children.Add(CreateRow(n1, true));
            midBlock.Children.Add(CreateRow(n2));
            midBlock.Children.Add(CreateRow(n3));
            midBlock.Children.Add(new Border{Height=28}); // empty row
            midBlock.Children.Add(CreateRow(n5));
            midBlock.Children.Add(CreateRow(n6));

            var rightBlock = new StackPanel();
            rightBlock.Children.Add(new Border{Height=38}); // num1 space
            rightBlock.Children.Add(CreateRow(num2));
            rightBlock.Children.Add(CreateRow(num3));
            rightBlock.Children.Add(CreateRow(num4));
            rightBlock.Children.Add(CreateRow(num5));
            rightBlock.Children.Add(CreateRow(num6));

            mainPanel.Children.Add(leftBlock);
            mainPanel.Children.Add(midBlock);
            mainPanel.Children.Add(rightBlock);

            MainGrid.Children.Add(mainPanel);
        }

        private StackPanel CreateRow(KeyDef[] keys, bool isTopRow = false)
        {
            var row = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(0, 0, 0, 2) };
            double baseSize = 28;
            if (isTopRow) row.Margin = new Thickness(0, 0, 0, 10);

            foreach (var k in keys)
            {
                var border = new Border
                {
                    Width = (baseSize * k.WidthMult) + ((k.WidthMult - 1) * 2),
                    Height = (baseSize * k.HeightMult) + ((k.HeightMult - 1) * 2),
                    Margin = new Thickness(0, 0, k.MarginRight, k.MarginBottom),
                    Background = (SolidColorBrush)Application.Current.Resources["NovaPanel"],
                    BorderBrush = (SolidColorBrush)Application.Current.Resources["NovaBorder"],
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(2)
                };

                if (!string.IsNullOrEmpty(k.Label))
                {
                    var text = new TextBlock
                    {
                        Text = k.Label,
                        Foreground = (SolidColorBrush)Application.Current.Resources["NovaTextMuted"],
                        FontSize = 9,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontFamily = new FontFamily("Consolas")
                    };
                    border.Child = text;
                }
                else
                {
                    border.Visibility = Visibility.Hidden;
                }

                if (k.WpfKey != Key.None)
                {
                    if (!_keyBorders.ContainsKey(k.WpfKey))
                    {
                        _keyBorders[k.WpfKey] = border; 
                    }
                }

                row.Children.Add(border);
            }
            return row;
        }

        public void UpdateHeatmap(Dictionary<string, int> freqs)
        {
            int max = freqs.Values.DefaultIfEmpty(0).Max();

            foreach (var kvp in _keyBorders)
            {
                string keyName = kvp.Key.ToString();
                int count = freqs.GetValueOrDefault(keyName, 0);
                
                // Fallbacks for Left/Right shifts and modifiers if the GlobalHook only returned the generic 'Shift' key.
                if (count == 0)
                {
                    if (kvp.Key == Key.LeftShift || kvp.Key == Key.RightShift) count = freqs.GetValueOrDefault("LeftShift", 0) / 2;
                }

                kvp.Value.Background = HeatmapRenderer.GetColorForFrequency(count, max);
            }
        }
    }
}
