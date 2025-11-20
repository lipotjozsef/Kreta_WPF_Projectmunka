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
            foreach (var f in Frames) Debug.WriteLine(f.Name);
        }

        public IEnumerable<FrameworkElement> getElementsByTag(DependencyObject parent, object tag)
        {
            IEnumerable<FrameworkElement> elements = new List<FrameworkElement>();

            foreach(var chil in LogicalTreeHelper.GetChildren(parent))
            {
                if (chil is FrameworkElement fe)
                {
                    if (Equals(fe.Tag, tag)) elements.Append(chil);
                }
            }
            return elements;
        }
    }
}