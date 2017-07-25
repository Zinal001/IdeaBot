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

namespace IdeaBot
{
    /// <summary>
    /// Interaction logic for SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();

            txtBotToken.Text = Properties.Settings.Default.BotToken;
            txtInputChannelId.Text = Properties.Settings.Default.InputChannelId.ToString();
            txtOutputChannelId.Text = Properties.Settings.Default.OutputChannelId.ToString();
            txtIdeaIndicator.Text = Properties.Settings.Default.IdeaIndicator;
            cbMentionIsIdea.IsChecked = Properties.Settings.Default.MentionIsIdea;

        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            if(String.IsNullOrEmpty(txtBotToken.Text))
            {
                MessageBox.Show("A bot token is required.\nSee https://discord.foxbot.me/docs/guides/getting_started/intro.html#adding-your-bot-to-a-server for more information.", "IdeaBot - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ulong InputChannelId;
            if (String.IsNullOrEmpty(txtInputChannelId.Text) || txtInputChannelId.Text == "0")
            {
                MessageBox.Show("", "IdeaBot - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(!ulong.TryParse(txtInputChannelId.Text, out InputChannelId))
            {
                MessageBox.Show("The Input Channel Id is not in the correct format.", "IdeaBot - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ulong OutputChannelId;
            if (String.IsNullOrEmpty(txtOutputChannelId.Text) || txtOutputChannelId.Text == "0")
            {
                MessageBox.Show("", "IdeaBot - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!ulong.TryParse(txtOutputChannelId.Text, out OutputChannelId))
            {
                MessageBox.Show("The Output Channel Id is not in the correct format.", "IdeaBot - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if(String.IsNullOrEmpty(txtIdeaIndicator.Text))
            {
                MessageBox.Show("The Idea Indicator field is required.", "IdeaBot - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Properties.Settings.Default.BotToken = txtBotToken.Text;
            Properties.Settings.Default.OutputChannelId = OutputChannelId;
            Properties.Settings.Default.InputChannelId = InputChannelId;
            Properties.Settings.Default.IdeaIndicator = txtIdeaIndicator.Text;
            Properties.Settings.Default.MentionIsIdea = cbMentionIsIdea.IsChecked.Value;
            Properties.Settings.Default.Save();

            this.DialogResult = true;
            this.Close();
        }

        internal static bool IsSettingsValid()
        {
            return !String.IsNullOrEmpty(Properties.Settings.Default.BotToken) && Properties.Settings.Default.OutputChannelId > 0 && Properties.Settings.Default.InputChannelId > 0;
        }
    }
}
