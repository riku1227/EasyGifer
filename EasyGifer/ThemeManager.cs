using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EasyGifer
{
    class ThemeManager
    {
        private MainWindow window = null;
        public ThemeManager(MainWindow mainWindow)
        {
            window = mainWindow;
        }

        public void SetUpTheme()
        {
            window.Style = (Style)window.Resources["DarkWindow"];
            SetUpLabelTheme();
            SetUpTextBoxTheme();
            SetUpGroupBoxTheme();
            SetUpRadioButtonTheme();
            window.OptimizeGif.Style = (Style)window.Resources["DarkCheckBox"];
            SetUpButtonTheme();
        }

        private void SetUpLabelTheme()
        {
            var labelStyle = (Style)window.Resources["DarkLabel"];
            window.InputFileLabel.Style = labelStyle;
            window.OutputFileLabel.Style = labelStyle;
            window.SettingsLabel.Style = labelStyle;
        }

        private void SetUpTextBoxTheme()
        {
            var textBoxStyle = (Style)window.Resources["DarkTextBox"];
            window.InputPathTextBox.Style = textBoxStyle;
            window.OutputPathTextBox.Style = textBoxStyle;
            window.GifWidthTextBox.Style = textBoxStyle;
            window.FPSTextBox.Style = textBoxStyle;
        }

        private void SetUpGroupBoxTheme()
        {
            var groupBoxStyle = (Style)window.Resources["DarkGroupBox"];
            window.GifWidthGroupBox.Style = groupBoxStyle;
            window.FPSGroupBox.Style = groupBoxStyle;
            window.GifQualityGroupBox.Style = groupBoxStyle;
        }

        private void SetUpRadioButtonTheme()
        {
            var radioButtonStyle = (Style)window.Resources["DarkRadioButton"];
            window.GifQualityLow.Style = radioButtonStyle;
            window.GifQualityMedium.Style = radioButtonStyle;
            window.GifQualityHigh.Style = radioButtonStyle;
            window.GifQualityGod.Style = radioButtonStyle;
        }

        private void SetUpButtonTheme()
        {
            var buttonStyle = (Style)window.Resources["DarkButton"];
            window.InputFileButton.Style = buttonStyle;
            window.OutputFileButton.Style = buttonStyle;
            window.GenerateGifButton.Style = buttonStyle;
        }
    }
}
