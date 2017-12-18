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

namespace socketServer.Windows
{
    /// <summary>
    /// Interaction logic for Appendix.xaml
    /// </summary>
    public partial class Appendix : Window
    {
        public Appendix()
        {
            InitializeComponent();
        }

        private void button7_Click(object sender, RoutedEventArgs e)
        {
            pictureMaker  thePictureMaker = new pictureMaker();
            thePictureMaker.createPictureFromData();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            demoForTensorFlow.lineNear();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            SystemSave.theAppendixWindow = null;
        }

        private void button1_Click(object sender, RoutedEventArgs e)
        {
            new Codes.AccordNotNetUse().Linear();
        }
    }
}
