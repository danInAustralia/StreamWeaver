using SocketStreamer;
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

namespace StreamerWPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MicPlayer player;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartClicked(object sender, RoutedEventArgs e)
        {
            player = new MicPlayer();

            player.NetworkPlaybackServer("134.115.93.89", 1090);
        }

        private void ClientClicked(object sender, RoutedEventArgs e)
        {
            player = new MicPlayer();
            player.MicToServer("134.115.93.89", 1090);
        }

        private void MicPlayback(object sender, RoutedEventArgs e)
        {
            player = new MicPlayer();
            player.MicPlayback();
        }
    }
}
