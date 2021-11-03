using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Data;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using System.Resources;
using System.Collections.ObjectModel;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Win32;

namespace myTunes
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MusicRepo library;
        private readonly MediaPlayer mediaPlayer = new MediaPlayer();
        //private List<string> playlists = new List<string>();
        private ObservableCollection<string> playlists;
        private DataSet music = new DataSet();
        private Point startPoint;
        private Playlist currentPlaylist;
        private string selectedPlaylist;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                library = new MusicRepo();
            }
            catch (Exception e)
            {
                MessageBox.Show("Error loading file: " + e.Message, "MiniPlayer", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Places music from music.xml into the data grid
            music.ReadXmlSchema("music.xsd");
            music.ReadXml("music.xml");

            playlists = new ObservableCollection<string>(library.Playlists);
            playlists.Insert(0, "All Music");
            myDataGrid.ItemsSource = library.Songs.DefaultView;
            myListBox.ItemsSource = playlists;
            myListBox.SelectedIndex = 0;
        }

        private class Playlist
        {
            public string Name { get; set; }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }

        private void infoButton_Click(object sender, RoutedEventArgs e)
        {
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog();
        }

        private void addPlaylistButton_Click(object sender, RoutedEventArgs e)
        {
            NewPlaylistWindow newPlaylistForm = new NewPlaylistWindow();
            newPlaylistForm.ShowDialog();
            string result = newPlaylistForm.playlistAdded;

            if (library.AddPlaylist(result))
            {
                //playlists = new List<Playlist>();

                var allMusic = new Playlist { Name = "All Music"};
                currentPlaylist = allMusic;
                playlists.Add(result);
                myListBox.ItemsSource = playlists;
            }
        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.DefaultExt = ".mp3";
            openFileDialog.Filter = "MP3 documents (.mp3)|*.mp3| M4A documents (.m4a)|*.m4a| WMA documents (.wma)|*.wma| WAV documents (.wav)|*.wav";
            Nullable<bool> result = openFileDialog.ShowDialog();
            if(result == true)
            {
                Song s = library.AddSong(openFileDialog.FileName.ToString());
                if(myListBox.SelectedItem.ToString() == "All Music")
                {
                    myDataGrid.ItemsSource = library.Songs.DefaultView;
                }
                else
                {
                    library.AddSongToPlaylist(s.Id, myListBox.SelectedItem.ToString());
                    myDataGrid.ItemsSource = library.SongsForPlaylist(myListBox.SelectedItem.ToString()).DefaultView;
                }

                // Select newly added item in data grid
                myDataGrid.Focus();
                myDataGrid.SelectedIndex = myDataGrid.Items.Count - 1;
                myDataGrid.SelectedItem = myDataGrid.SelectedIndex;
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            DataRowView dataRowView = myDataGrid.SelectedItem as DataRowView;
            if (myDataGrid.SelectedItem != null)
            {
                int songId = Convert.ToInt32(dataRowView.Row.ItemArray[0]);
                mediaPlayer.Open(new Uri(library.GetSong(songId).Filename));
                mediaPlayer.Play();
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Stop();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            library.Save();
        }

        private void removeButton_Click(object sender, RoutedEventArgs e)
        {
            
            if (myListBox.SelectedItem != null)
            {
                System.Data.DataRowView song = (DataRowView)myDataGrid.SelectedItem;
                if (myListBox.SelectedItem.ToString() == "All Music")
                {
                    var confirmRemove = new RemoveSongWindow();
                    confirmRemove.ShowDialog();
                    bool confirmed = confirmRemove.RemoveConfirmation;
                    // Delete song
                    library.DeleteSong((int)song.Row[0]);
                    myDataGrid.ItemsSource = library.Songs.DefaultView;
                }
                else
                {
                    // Delete song from playlist
                    library.RemoveSongFromPlaylist(myDataGrid.SelectedIndex + 1, Convert.ToInt32(song.Row[0]), myListBox.SelectedItem.ToString());
                    myDataGrid.ItemsSource = library.SongsForPlaylist(myListBox.SelectedItem.ToString()).DefaultView;
                }
            }
        }

        private void myDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            var mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragDrop.DoDragDrop(myDataGrid, (myDataGrid.Items[myDataGrid.SelectedIndex] as DataRowView)[0], DragDropEffects.Copy);
            }
        }

        private void myListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //myDataGrid.ItemsSource = library.Playlists[0];
            string selectedplaylist = myListBox.SelectedItem as string;

            if (selectedplaylist == "All Music")
            {
                myDataGrid.ItemsSource = library.Songs.DefaultView;
                myDataGrid.IsReadOnly = false;
            }
            else
            {
                DataTable allPlaylists = library.SongsForPlaylist(selectedplaylist);
                myDataGrid.ItemsSource = allPlaylists.DefaultView;
                myDataGrid.IsReadOnly = true;
            }
        }

        private void myDataGrid_PreviewMouseMove(object sender, MouseEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }

        private void myListBox_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            Label playlist = (Label)sender;
            if(playlist != null)
            {
                e.Effects = DragDropEffects.Copy;
                selectedPlaylist = playlist.Content.ToString();
            }
        }

        private void myListBox_Drop(object sender, DragEventArgs e)
        {
            if(e.Data.GetDataPresent(DataFormats.StringFormat))
            {
                int datastring = Convert.ToInt32(e.Data.GetData(DataFormats.StringFormat));
                library.AddSongToPlaylist(datastring, selectedPlaylist);
            }
        }
    }
}
