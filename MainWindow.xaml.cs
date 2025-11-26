using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace Kreta_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public IEnumerable<FrameworkElement>? Frames = null;
        // Temp Only!
        string[] possibleClasses = ["Matematika", "Történelem", "Adatbázis", "Asztali Alkalmazások Fejlesztése", "Backend", "Nyelvtan", "Irodalom", "Testnevelés", "Angol", "Web fejlesztés"];
        Random myRandom = new Random();

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
            UniformGrid? timeTable = sender as UniformGrid;
            if (timeTable is null) return;

            if (Equals(e.NewValue, true))
            {
                for (int i = 0; i != 9; i++)
                {
                    for (int j = 0; j != 5; j++)
                    {
                        Label newClassLabel = new Label
                        {
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Style = (Style)ttTable.Resources["baseLabel"],
                            Background = i % 2 == 0 ? Brushes.WhiteSmoke : Brushes.White,
                            Content = possibleClasses[myRandom.Next(possibleClasses.Length)]
                        };
                        timeTable.Children.Add(newClassLabel);
                    }
                }
            }
            else timeTable.Children.Clear();
        }

        private void loadGrades(object sender, DependencyPropertyChangedEventArgs e)
        {
            StackPanel? gradePanel = sender as StackPanel;
            if (gradePanel is null) return;

            if (Equals(e.NewValue, true))
            {
                for(int i = 0; i != possibleClasses.Length; i++)
                {
                    UniformGrid generatedGradeCard = generateGradeCard(i, possibleClasses[i]);
                    gradePanel.Children.Add(generatedGradeCard);
                }
            }
            else gradePanel.Children.Clear();
        }

        private void loadAbsences(object sender, DependencyPropertyChangedEventArgs e)
        {
            StackPanel? absencePanel = sender as StackPanel;
            if(absencePanel is null) return;

            if (Equals(e.NewValue, true))
            {
                Border generatedBorder = generateNewAbsenceCard();
                absencePanel.Children.Add(generatedBorder);
            }
            else absencePanel.Children.Clear();
        }

        private void loadHomework(object sender, DependencyPropertyChangedEventArgs e)
        {
            StackPanel? homeworkPanel = sender as StackPanel;
            if (homeworkPanel is null) return;

            if(Equals(e.NewValue, true))
            {
                Border generatedBorder = generateNewHomeworkCard();
                homeworkPanel.Children.Add(generatedBorder);
            }
            else homeworkPanel.Children.Clear();
        }

        private UniformGrid generateGradeCard(int index = 0, string content = "", bool flipColors = false)
        {
            UniformGrid gridParent = new UniformGrid
            {
                Columns = 16,
                Rows = 1,
                Background = index % 2 == 0 ? Brushes.WhiteSmoke : Brushes.White,
            };
            Label indexLabel = new Label
            {
                Content = (index + 1).ToString()
            };
            Label nameLabel = new Label
            {
                Content = content
            };

            gridParent.Children.Add(indexLabel);
            gridParent.Children.Add(nameLabel);

            for (int j = 0; j != 14; j++)
            {
                Label gradeLabel = new Label
                {
                    Content = "-"
                };
                gridParent.Children.Add(gradeLabel);
            }
            return gridParent;
        }

        private Border generateNewAbsenceCard()
        {
            Border parentBorder = generateNewCard(["November 6", "Csütörtök", "-"]);
            return parentBorder;
        }

        private Border generateNewHomeworkCard()
        {
            Border parentBorder = generateNewCard(["2025. 11. 26", "Fizika", "TK 89/ 1. 2. 3."]);
            return parentBorder;
        }

        private Border generateNewCard(string[] contents)
        {
            Border parentBorder = new Border();
            UniformGrid parentGrid = new UniformGrid()
            {
                Columns = 2,
                Rows = 1
            };
            parentBorder.Child = parentGrid;
            StackPanel stackParent = new StackPanel
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
            };
            parentGrid.Children.Add(stackParent);
            Label title = new Label()
            {
                Content = contents[0]
            };
            Label title2 = new Label()
            {
                Content = contents[1]
            };
            stackParent.Children.Add(title);
            stackParent.Children.Add(title2);

            Label desc = new Label()
            {
                Content = contents[2],
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = Brushes.Gray,
                Foreground = Brushes.WhiteSmoke

            };
            parentGrid.Children.Add(desc);
            return parentBorder;
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