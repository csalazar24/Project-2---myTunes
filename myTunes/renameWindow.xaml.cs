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
    /// Interaction logic for renameWindow.xaml
    /// </summary>
    public partial class renameWindow : Window
    {
        public string newName;
        public bool result = false;
        public renameWindow()
        {
            InitializeComponent();
        }
        private void renameButton_Click(object sender, RoutedEventArgs e)
        {
            newName = newPlaylist.Text;
            result = true;
            Close();
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
