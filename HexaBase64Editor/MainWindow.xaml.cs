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

namespace HexaBase64Editor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool updating = false;

        private readonly Brush defaultTextBoxBorderBrush;
        private readonly Thickness defaultTextBoxBorderThickness;

        private readonly Brush errorTextBoxBorderBrush;
        private readonly Thickness errorTextBoxBorderThickness;

        public MainWindow()
        {
            InitializeComponent();

            defaultTextBoxBorderBrush = Base64TextBox.BorderBrush;
            defaultTextBoxBorderThickness = Base64TextBox.BorderThickness;

            errorTextBoxBorderBrush = Brushes.Red;
            errorTextBoxBorderThickness = new Thickness(2);
        }

        private void HexaTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!updating)
            {
                byte[] number;
                if (TryParseHexaToByteArray(HexaTextBox.Text, out number))
                {
                    updating = true;
                    Base64TextBox.Text = Convert.ToBase64String(number);
                    ClearErrorTextBox(Base64TextBox);
                    ClearErrorTextBox(HexaTextBox);
                    updating = false;
                }
                else
                {
                    SetErrorTextBox(HexaTextBox);
                }
            }
        }

        private void Base64TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!updating)
            {
                try
                {
                    var number = Convert.FromBase64String(Base64TextBox.Text);
                    updating = true;
                    HexaTextBox.Text = string.Join("", number.Select(b => b.ToString("X2")).ToArray());
                    ClearErrorTextBox(Base64TextBox);
                    ClearErrorTextBox(HexaTextBox);
                    updating = false;
                }
                catch
                {
                    SetErrorTextBox(Base64TextBox);
                }
            }
        }
        private void SetErrorTextBox(TextBox textBox)
        {
            textBox.BorderBrush = errorTextBoxBorderBrush;
            textBox.BorderThickness = errorTextBoxBorderThickness;
        }

        private void ClearErrorTextBox(TextBox textBox)
        {
            textBox.BorderBrush = defaultTextBoxBorderBrush;
            textBox.BorderThickness = defaultTextBoxBorderThickness;
        }

        private bool TryParseHexaToByteArray(string hexStr, out byte[] result)
        {
            result = new byte[0];

            if (hexStr.Length % 2 != 0)
                return false;
            else
            {
                var hexGroupsArray = new string[1 + hexStr.Length / 2];

                for (int i = 0; i < hexStr.Length; i += 2)
                    hexGroupsArray[i / 2] = new string(new[] { hexStr[i], hexStr[i + 1] });

                result = new byte[hexGroupsArray.Length];
                bool success = true;

                for (var i = 0; i < hexGroupsArray.Length; i++)
                {
                    try
                    {
                        result[i] = Convert.ToByte(hexGroupsArray[i], 16);
                    }
                    catch
                    {
                        success = false;
                        break;
                    }
                }

                return success;
            }
        }
    }
}
