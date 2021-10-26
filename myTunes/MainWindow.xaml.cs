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

namespace myTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private List<Playlist> playlists = new List<Playlist>();
        private MusicRepo library = new MusicRepo();

        public MainWindow()
        {
            InitializeComponent();
        }

        private class Playlist
        {
            public string Name { get; set; }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var allMusic = new Playlist { Name = "All Music" };
            playlists.Add(allMusic);
            playlists.AddRange(library.Playlists.Select(playlist => new Playlist { Name = playlist }));
            myListBox.ItemsSource = playlists;
        }
    }
}
