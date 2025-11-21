using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Kreta_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable<FrameworkElement>? Frames = null;

        public MainWindow()
        {
            InitializeComponent();
            Frames = getElementsByTag(this, "contentFrame");
            Debug.WriteLine("\tPrinting Frames");
            foreach (var f in Frames) Debug.WriteLine($"\t\t{f.Name}");
        }

        public IEnumerable<FrameworkElement> getElementsByTag(DependencyObject parent, object tag)
        {
            var elements = new List<FrameworkElement>();

            foreach (var child in LogicalTreeHelper.GetChildren(parent))
            {
                if (child is FrameworkElement fe)
                {
                    if (Equals(fe.Tag, tag)) elements.Add(fe);

                    elements.AddRange(getElementsByTag(fe, tag));
                }
            }
            return elements;
        }

        private void menuButtonClick(object sender, RoutedEventArgs e)
        {
            if (sender is null) return;
            Button? senderBtn = sender as Button;
            changeActiveFrameTo(senderBtn?.Tag);
        }

        private void changeActiveFrameTo(object? activeName)
        {
            if (Frames is null || activeName is null) return;
            foreach(FrameworkElement f in Frames)
            {
                f.Visibility = Equals(f.Name, activeName) ? Visibility.Visible : Visibility.Hidden;
            }
        }
    }
}