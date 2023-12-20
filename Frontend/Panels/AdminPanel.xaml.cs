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
using System.Windows.Shapes;

namespace MesseauftrittDatenerfassung_UI
{
    /// <summary>
    /// Interaction logic for AdminPanel.xaml
    /// </summary>
    public partial class AdminPanel : Window
    {
        public AdminPanel()
        {
            InitializeComponent();
        }

        private void TextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && textBox.Text == textBox.Tag.ToString())
            {
                textBox.Text = string.Empty;
            }
        }

        private void TextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (textBox != null && string.IsNullOrWhiteSpace(textBox.Text))
            {
                textBox.Text = textBox.Tag.ToString();
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            this.Close();
            mainWindow.Show();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            UpdateCheckBoxes(selectedCheckBox, true);
            UpdateTextBoxVisibility(selectedCheckBox, true);
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            CheckBox selectedCheckBox = sender as CheckBox;
            UpdateCheckBoxes(selectedCheckBox, false);
            UpdateTextBoxVisibility(selectedCheckBox, false);
        }

        private void UpdateCheckBoxes(CheckBox selectedCheckBox, bool isChecked)
        {
            foreach (ComboBoxItem item in FilterComboBox.Items)
            {
                CheckBox checkBox = (item.Content as CheckBox);
                if (checkBox != null && checkBox != selectedCheckBox)
                {
                    checkBox.IsChecked = false;
                }
            }
        }

        private void UpdateTextBoxVisibility(CheckBox selectedCheckBox, bool isChecked)
        {
            // Setze alle Textfelder auf unsichtbar
            FilterIdTextBox.Visibility = Visibility.Collapsed;
            FilterIdBorder.Visibility = Visibility.Collapsed;
            FilterNameTextBox.Visibility = Visibility.Collapsed;
            FilterNameBorder.Visibility = Visibility.Collapsed;

            // Wenn isChecked, mache das ausgewählte Textfeld sichtbar
            if (isChecked)
            {
                if (selectedCheckBox.Tag.ToString() == "IdCheckBox")
                {
                    FilterIdTextBox.Visibility = Visibility.Visible;
                    FilterIdBorder.Visibility = Visibility.Visible;
                }
                else if (selectedCheckBox.Tag.ToString() == "NameCheckBox")
                {
                    FilterNameTextBox.Visibility = Visibility.Visible;
                    FilterNameBorder.Visibility = Visibility.Visible;
                }
                else if (selectedCheckBox.Tag.ToString() == "BothCheckBox")
                {
                    FilterIdTextBox.Visibility = Visibility.Visible;
                    FilterIdBorder.Visibility = Visibility.Visible;
                    FilterNameTextBox.Visibility = Visibility.Visible;
                    FilterNameBorder.Visibility = Visibility.Visible;
                }
            }
        }

    }
}
