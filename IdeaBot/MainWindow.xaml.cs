using Discord;
using Discord.WebSocket;
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

namespace IdeaBot
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private DiscordSocketClient _Client;

        private bool _OutputChannelIsGroup = false;

        public MainWindow()
        {
            InitializeComponent();

            _Client = new DiscordSocketClient();
            _Client.Log += _Client_Log;
            _Client.MessageReceived += _Client_MessageReceived;
            _Client.Connected += _Client_Connected;
            _Client.Disconnected += _Client_Disconnected;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (!SettingsWindow.IsSettingsValid())
            {
                btnBotState.IsEnabled = false;
                btnSettings_Click(null, null);
            }
        }

        private async void btnBotState_Click(object sender, RoutedEventArgs e)
        {
            if(btnBotState.Content.ToString() == "Start")
            {
                if (!SettingsWindow.IsSettingsValid())
                {
                    MessageBox.Show("Missing required settings.", "IdeaBot - Invalid Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
                    btnBotState.IsEnabled = false;
                    return;
                }
                
                await _Client.LoginAsync(Discord.TokenType.Bot, Properties.Settings.Default.BotToken);
                await _Client.StartAsync();
                btnBotState.Content = "Stop";
            }
            else
            {
                await _Client.LogoutAsync();
                await _Client.StopAsync();
                btnBotState.Content = "Start";
            }
        }

        private Task _Client_Connected()
        {
            _OutputChannelIsGroup = _Client.GroupChannels.Where(c => c.Id == Properties.Settings.Default.OutputChannelId).FirstOrDefault() != null;

            btnBotState.Dispatcher.Invoke(new Action(() => {
                btnBotState.Content = "Stop";
            }));

            return Task.CompletedTask;
        }

        private Task _Client_Disconnected(Exception arg)
        {
            btnBotState.Dispatcher.Invoke(new Action(() => {
                btnBotState.Content = "Start";
            }));

            return Task.CompletedTask;
        }

        private async Task _Client_MessageReceived(SocketMessage arg)
        {
            IMessage msg = await arg.Channel.GetMessageAsync(arg.Id);

            if((arg.Channel.Id == Properties.Settings.Default.InputChannelId && arg.Content.Contains(Properties.Settings.Default.IdeaIndicator)) || (arg.MentionedUsers.Contains(_Client.CurrentUser) && Properties.Settings.Default.MentionIsIdea))
            {
                String ReplyContent = "Idea by <@" + arg.Author.Id + "> \n" + arg.Content;

                await _Client.Guilds.First().GetTextChannel(Properties.Settings.Default.OutputChannelId).SendMessageAsync(ReplyContent);
                Log("[" + DateTime.Now.ToString("") + "] Found an idea by " + arg.Author.Username, Brushes.DarkCyan);
            }
        }

        private Task _Client_Log(LogMessage msg)
        {
            //TODO: Log to file depending on severity
            Console.WriteLine("[" + msg.Severity.ToString() + "] " + msg.Message);

            rtbLog.Dispatcher.Invoke(new Action(() => {

                String logText = "[" + DateTime.Now.ToString("") + "] [" + msg.Severity.ToString() + "] " + msg.Message;
                if (msg.Exception != null)
                    logText += FormatLogException(msg.Exception);

                Log(logText, LogSeverityToBrush(msg.Severity));
            }));

            return Task.CompletedTask;
        }

        private void Log(String text, Brush foreground)
        {
            rtbLog.Dispatcher.Invoke(new Action(() => {
                Run run = new Run(text);
                run.Foreground = foreground;

                rtbLog.Document.Blocks.Add(new Paragraph(run));
                rtbLog.ScrollToEnd();
            }));
        }

        private String FormatLogException(Exception ex, String indent = "\t")
        {
            String Str = "\n" + indent + "Error: " + ex.Message;
            Str += "\n" + indent + "\tStacktrace: " + ex.StackTrace;

            if(ex.InnerException != null)
                Str += "\n" + indent + "\tInner Exception:" + FormatLogException(ex.InnerException, indent + "\t");

            return Str;
        }

        private Brush LogSeverityToBrush(LogSeverity severity)
        {
            switch(severity)
            {
                case LogSeverity.Critical:
                    return Brushes.DarkRed;
                case LogSeverity.Debug:
                    return Brushes.Cyan;
                case LogSeverity.Error:
                    return Brushes.Red;
                case LogSeverity.Warning:
                    return Brushes.Yellow;
                default:
                    return Brushes.Lime;
            }
        }

        private void btnSettings_Click(object sender, RoutedEventArgs e)
        {
            SettingsWindow sw = new SettingsWindow() { Owner = this };
            sw.ShowDialog();

            btnBotState.IsEnabled = SettingsWindow.IsSettingsValid();
        }


    }
}
