using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Security.AccessControl;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
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
            /*Debug.WriteLine("\tPrinting Frames");
            foreach (var f in Frames) Debug.WriteLine($"\t\t{f.Name}");*/
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

        private void loadTimeTable(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Temp Only!
            string[] possibleClasses = ["Matematika", "Történelem", "Adatbázis", "Asztali Alkalmazások Fejlesztése", "Backend", "Nyelvtan", "Irodalom", "Testnevelés", "Angol", "Web fejlesztés"];
            Random myRandom = new Random();
            UniformGrid? timeTable = sender as UniformGrid;
            if (timeTable is null) return;

            if (Equals(e.NewValue, true))
            {
                bool isStriped = false;
                for (int i = 0; i != 9; i++)
                {
                    for (int j = 0; j != 5; j++)
                    {
                        Label newClassLabel = new Label
                        {
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Style = (Style)ttTable.Resources["baseLabel"],
                            Content = possibleClasses[myRandom.Next(possibleClasses.Length)]
                        };
                        timeTable.Children.Add(newClassLabel);
                        if (isStriped) newClassLabel.Background = Brushes.WhiteSmoke;
                    }
                    isStriped = !isStriped;
                }
            }
            else timeTable.Children.Clear();
        }

        private void generateRowsColumnsByTag(object sender, EventArgs e)
        {
            Grid? generateGrid = sender as Grid;
            if (generateGrid is null && generateGrid?.Tag is null) return;
            string[] parts = generateGrid.Tag.ToString().Split('_');
            (int rowNum, int colNum) = (int.Parse(parts[0]), int.Parse(parts[1]));
            for (int i = 0; i != rowNum; i++)
                generateGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Star) });
            for (int j = 0; j != colNum; j++)
                generateGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Star) });
        }
    }
}