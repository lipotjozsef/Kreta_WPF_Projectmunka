using System.Diagnostics;
using System.Security.AccessControl;
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
using System.Xml.Linq;
using static System.Runtime.InteropServices.JavaScript.JSType;

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

        private void revertBorder(object sender, RoutedEventArgs e)
        {
            Control? myControl = sender as Control;
            if (myControl != null) myControl.BorderBrush = Brushes.Gray;
        }

        private void submitLogin(object sender, RoutedEventArgs e)
        {
            Action<string, Control?> writeError = (string errorText, Control? elementToChange) =>
            {
                errorBlock.Text = errorText;
                if (elementToChange != null) elementToChange.BorderBrush = Brushes.Red;
                throw new ArgumentNullException(errorText);
            };

            try
            {
                string username = loginUsername.Text;
                string password = loginPassword.Password;

                if (string.IsNullOrEmpty(username)) writeError("A felhasználó név megadása kötelező a bejelentkezéshez!", loginUsername);
                if (string.IsNullOrEmpty(password)) writeError("A jelszó megadása kötelező a bejelentkezéshez!", loginPassword);

                bool loggedIn = tryLogin(username, password);
                if (loggedIn)
                {
                    loginPage.Visibility = Visibility.Collapsed;
                    activePage.Visibility = Visibility.Visible;
                }
                else writeError("A megadott felhasználónév/jelszó párral nem található felhasználó a rendszerben.", null);
            }
            catch (ArgumentNullException) { }
        }

        private bool secondOnlyLogin = false;
        private bool tryLogin(string us, string ps)
        {
            // TODO ADD LOGIN FUNCTIONALITY
            bool returnVal = secondOnlyLogin;
            secondOnlyLogin = !secondOnlyLogin;
            return returnVal;
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
                f.Visibility = Equals(f.Name, activeName) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void emptyLabel(object sender, RoutedEventArgs e)
        {
            TextBlock? myObject = sender as TextBlock;
            if(myObject is TextBlock) myObject.Text = "";
        }
    }
}