﻿using System;
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
            DataSet music = new DataSet();
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

        }

        private void openButton_Click(object sender, RoutedEventArgs e)
        {

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
    }
}
