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
        private List<Playlist> playlists = new List<Playlist>();
        private DataSet music = new DataSet();
        private Point startPoint;
        private Playlist currentPlaylist;
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
            myDataGrid.ItemsSource = music.Tables["song"].DefaultView;
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
                playlists.Clear();
                playlists = new List<Playlist>();

                var allMusic = new Playlist { Name = "All Music"};
                currentPlaylist = allMusic;
                playlists.Add(allMusic);
                playlists.AddRange(library.Playlists.Select(playlist => new Playlist { Name = playlist}));
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
                library.Save();
                music.Clear();
                music.ReadXml("music.xml");

                // Select newly added item in data grid
                myDataGrid.Focus();
                DataRow row = music.Tables["song"].Rows.Find(s.Id);
                myDataGrid.SelectedIndex = music.Tables["song"].Rows.IndexOf(row);
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
            RemoveSongWindow confirmRemove = new RemoveSongWindow();
            confirmRemove.ShowDialog();

            if (confirmRemove.RemoveConfirmation)
            {
                DataRowView rowView = myDataGrid.SelectedItem as DataRowView;
                if (rowView != null)
                {
                    int songId = Convert.ToInt32(rowView.Row.ItemArray[0]);
                    library.RemoveSongFromPlaylist(myDataGrid.SelectedIndex + 1, songId, currentPlaylist.Name);
                    //library.DeleteSong(songId);
                    myDataGrid.ItemsSource = null;
                }
            }
            else
            {
                e.Handled = true;
            }
        }

        private void removeButton_Click2(object sender, RoutedEventArgs e)
        {
            var confirmRemove = new RemoveSongWindow();
            confirmRemove.ShowDialog();
            bool confirmed = confirmRemove.RemoveConfirmation;

            // Delete song
            if (confirmed)
            {
                // Remove the row matching the given ID
                DataTable table = music.Tables["song"];
                if (table != null)
                {
                    DataRow row = table.Rows[myDataGrid.SelectedIndex];
                    if (row != null && library.DeleteSong((int)music.Tables["song"].Rows[myDataGrid.SelectedIndex][0]))
                    {
                        music.Tables["song"].Rows.Remove(row);
                    }
                }
            }
        }

        private void myDataGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var mousePos = e.GetPosition(null);
            Vector diff = startPoint - mousePos;

            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance))
            {
                DragDrop.DoDragDrop(myDataGrid, (myDataGrid.Items[myDataGrid.SelectedIndex] as DataRowView)[0], DragDropEffects.Copy);
            }
        }

        private void myDataGrid_MouseMove(object sender, MouseEventArgs e)
        {
            startPoint = e.GetPosition(null);
        }
    }
}
