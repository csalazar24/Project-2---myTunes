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

namespace myTunes
{
    /// <summary>
    /// Interaction logic for NewPlaylistWindow.xaml
    /// </summary>
    public partial class NewPlaylistWindow : Window
    {
        public string playlistAdded;
        public NewPlaylistWindow()
        {
            InitializeComponent();
        }

        private void saveButton_Click(object sender, RoutedEventArgs e)
        {
            playlistAdded = newPlaylist.Text;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
