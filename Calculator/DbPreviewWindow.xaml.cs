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

namespace Calculator
{
    /// <summary>
    /// Interaction logic for DbPreviewWindow.xaml
    /// </summary>
    public partial class DbPreviewWindow : Window
    {
        private DbConnection dbConnection = new DbConnection();

        public DbPreviewWindow()
        {
            InitializeComponent();

            listViewPreview.ItemsSource = dbConnection.PrivewSavedData();
        }
    }
}
