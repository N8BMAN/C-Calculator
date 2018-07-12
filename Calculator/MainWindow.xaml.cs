using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Calculator
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        bool firstIn = false;

        // Keyboard input to the window
        public void windowKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.D1:
                case Key.NumPad1:
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                    {
                        btnTextEntry_Click(btnFactorial, null);
                    }
                    else
                    {
                        btnTextEntry_Click(btn1, null);
                    }
                    break;
                case Key.D2:
                case Key.NumPad2:
                    btnTextEntry_Click(btn2, null);
                    break;
                case Key.D3:
                case Key.NumPad3:
                    btnTextEntry_Click(btn3, null);
                    break;
                case Key.D4:
                case Key.NumPad4:
                    btnTextEntry_Click(btn4, null);
                    break;
                case Key.D5:
                case Key.NumPad5:
                    btnTextEntry_Click(btn5, null);
                    break;
                case Key.D6:
                case Key.NumPad6:
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                    {
                        btnTextEntry_Click(btnExp, null);
                        break;
                    }
                    btnTextEntry_Click(btn6, null);
                    break;
                case Key.D7:
                case Key.NumPad7:
                    btnTextEntry_Click(btn7, null);
                    break;
                case Key.D8:
                case Key.NumPad8:
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                    {
                        btnTextEntry_Click(btnMult, null);
                        break;
                    }
                    btnTextEntry_Click(btn8, null);
                    break;
                case Key.D9:
                case Key.NumPad9:
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                    {
                        btnTextEntry_Click(btnOpenPar, null);
                        break;
                    }
                    btnTextEntry_Click(btn9, null);
                    break;
                case Key.D0:
                case Key.NumPad0:
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                    {
                        btnTextEntry_Click(btnClosePar, null);
                        break;
                    }
                    btnTextEntry_Click(btn0, null);
                    break;
                case Key.Add:
                    btnTextEntry_Click(btnPlus, null);
                    break;
                case Key.OemMinus:
                case Key.Subtract:
                    btnTextEntry_Click(btnMinus, null);
                    break;
                case Key.Multiply:
                    btnTextEntry_Click(btnMult, null);
                    break;
                case Key.Divide:
                case Key.OemQuestion:
                    btnTextEntry_Click(btnDiv, null);
                    break;
                case Key.C:
                case Key.OemClear:
                    btnClear_Click(this, null);
                    break;
                case Key.OemPlus:
                    if (e.KeyboardDevice.IsKeyDown(Key.LeftShift) || e.KeyboardDevice.IsKeyDown(Key.RightShift))
                    {
                        btnTextEntry_Click(btnPlus, null);
                        break;
                    }
                    btnEqual_Click(this, null);
                    break;
                case Key.Enter:
                    btnEqual_Click(btnEqual, null);
                    break;
                case Key.Back:
                    btnBack_Click(this, null);
                    break;
                case Key.OemPeriod:
                case Key.Decimal:
                    btnTextEntry_Click(btnDot, null);
                    break;
                case Key.OemComma:
                    btnTextEntry_Click(btnComma, null);
                    break;
                case Key.X:
                    btnTextEntry_Click(btnVar, null);
                    break;
                case Key.E:
                    btnTextEntry_Click(btnE, null);
                    break;
                case Key.Up:
                    btnUp_Click(btnUp, null);
                    break;
                case Key.Down:
                    btnDown_Click(btnDown, null);
                    break;
            }
        }

        //handlers for each button click
        public void btnClear_Click(object sender, RoutedEventArgs e) { tbMainText.Clear(); previousIndex = 1; }

        public void btnBack_Click(object sender, RoutedEventArgs e) { if (tbMainText.Text.Length == 0) return; tbMainText.Text = tbMainText.Text.Substring(0, tbMainText.Text.Length - 1); }

        List<string> previousEntry = new List<string>();
        int previousIndex = -1;

        public void btnEqual_Click(object sender, RoutedEventArgs e)
        {
            //calculate using parser class, output to mainText and history box
            string expression = tbMainText.Text;
            try
            {
                Parser parser = new Parser(expression);
                parser.usingDegrees = btnDegRad.Content.Equals("deg");
                previousEntry.Add(tbMainText.Text);
                previousIndex = 1;
                tbMainText.Text = parser.Calculate();
                lbHistory.Items.Add(expression + "=" + tbMainText.Text);
                lbHistory.ScrollIntoView(lbHistory.Items[lbHistory.Items.Count - 1]);
            }
            catch (Exception error)
            {
                tbMainText.Text = "ERR: " + error.Message;
            }
            firstIn = true;
        }

        public void lbHistory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //stop it from looping
            if (lbHistory.SelectedItem == null) return;
            //substring to just the result, output to mainText, deselect
            string temp = lbHistory.SelectedValue.ToString();
            tbMainText.Text += temp.Substring(temp.IndexOf('=') + 1);
            lbHistory.UnselectAll();
        }

        public void btnDegRad_Click(object sender, RoutedEventArgs e)
        {
            if (btnDegRad.Content.Equals("deg"))
                btnDegRad.Content = "rad";
            else btnDegRad.Content = "deg";
        }

        public void btnTextEntry_Click(object sender, RoutedEventArgs e)
        {
            if(firstIn)
            {
                tbMainText.Clear();
                firstIn = false;
            }
            Button b = (Button)sender;
            tbMainText.Text += b.Tag;
            
            tbMainText.CaretIndex = tbMainText.Text.Length;
            var rect = tbMainText.GetRectFromCharacterIndex(tbMainText.CaretIndex);
            tbMainText.ScrollToHorizontalOffset(rect.Left);
        }

        bool dyslexiaMode = false;
        public void adaMode_Button_Click(object sender, RoutedEventArgs e)
        {
            if (dyslexiaMode)
            {
                foreach (Button btn in FindVisualChildren<Button>(windCalc))
                {
                    btn.Background = (Brush)new BrushConverter().ConvertFrom("#FF454545");
                    btn.Foreground = new SolidColorBrush(Colors.White);
                    btn.FontFamily = new FontFamily("Open Sans Light");
                }
                tbMainText.Background = (Brush)new BrushConverter().ConvertFrom("#FF454545");
                tbMainText.Foreground = new SolidColorBrush(Colors.White);
                tbMainText.FontFamily = new FontFamily("Open Sans Light");
                lbHistory.Background = (Brush)new BrushConverter().ConvertFrom("#FF454545");
                lbHistory.Foreground = new SolidColorBrush(Colors.White);
                lbHistory.FontFamily = new FontFamily("Open Sans Light");

                btnADA_mode.Content = "Dyscalculia Mode";
                dyslexiaMode = false;
            }
            else
            {
                foreach (Button btn in FindVisualChildren<Button>(windCalc))
                {
                    btn.Background = new SolidColorBrush(Colors.Yellow);
                    btn.Foreground = new SolidColorBrush(Colors.Black);
                    btn.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#OpenDyslexicAlta");
                }
                tbMainText.Background = new SolidColorBrush(Colors.Yellow);
                tbMainText.Foreground = new SolidColorBrush(Colors.Black);
                tbMainText.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#OpenDyslexicAlta");
                lbHistory.Background = new SolidColorBrush(Colors.Yellow);
                lbHistory.Foreground = new SolidColorBrush(Colors.Black);
                lbHistory.FontFamily = new FontFamily(new Uri("pack://application:,,,/"), "./Fonts/#OpenDyslexicAlta");

                btnADA_mode.Content = "Standard Mode";
                dyslexiaMode = true;
            }
        }

        //Clear Everything
        public void btnClearHist_Click(object sender, RoutedEventArgs e)
        {
            tbMainText.Clear();
            lbHistory.Items.Clear();
            previousEntry.Clear();
            previousIndex = -1;
        }

        public void btnUp_Click(object sender, RoutedEventArgs e)
        {
            if (previousIndex > 0)
            {
                tbMainText.Text = previousEntry[previousEntry.Count-previousIndex];
                if(previousIndex < previousEntry.Count)
                    previousIndex++;
            }
        }

        public void btnDown_Click(object sender, RoutedEventArgs e)
        {
            if (previousIndex > 1)
            {
                if(previousEntry.Count - previousIndex > -1)
                    previousIndex--;
                tbMainText.Text = previousEntry[previousEntry.Count - previousIndex];
            }
        }

        //Shamelessly ripped from https://stackoverflow.com/questions/974598/find-all-controls-in-wpf-window-by-type
        //Needed for changing 
        public static IEnumerable<T> FindVisualChildren<T>(DependencyObject depObj) where T : DependencyObject
        {
            if (depObj != null)
            {
                for (int i = 0; i < VisualTreeHelper.GetChildrenCount(depObj); i++)
                {
                    DependencyObject child = VisualTreeHelper.GetChild(depObj, i);
                    if (child != null && child is T)
                    {
                        yield return (T)child;
                    }

                    foreach (T childOfChild in FindVisualChildren<T>(child))
                    {
                        yield return childOfChild;
                    }
                }
            }
        }
    }
}
