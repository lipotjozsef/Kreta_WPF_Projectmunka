using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

namespace Kreta_WPF.Pages
{
    /// <summary>
    /// Interaction logic for StudentPage.xaml
    /// </summary>
    public partial class StudentPage : Page
    {
        public IEnumerable<FrameworkElement>? Frames = null;
        string[] possibleSubjects = [];
        Random myRandom = new Random();

        Brush darkBlue = new SolidColorBrush(Color.FromRgb(43, 52, 103));
        Brush lightBlue = new SolidColorBrush(Color.FromRgb(186, 215, 233));

        Brush stripeLightBlue = new SolidColorBrush(Color.FromRgb(200, 231, 250));
        Brush stripeDarkBlue = new SolidColorBrush(Color.FromRgb(165, 192, 209));

        private readonly Student loggedinUser;

        public StudentPage(Student loggedinUser)
        {
            this.loggedinUser = loggedinUser;
            possibleSubjects = loggedinUser.Subjects.Select(x => x.Name).ToArray();
            InitializeComponent();
            Frames = MainWindow.getElementsByTag(this, "contentFrame");
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
            foreach (FrameworkElement f in Frames)
            {
                f.Visibility = Equals(f.Name, activeName) ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        private void ReturnHome(object sender, RoutedEventArgs e)
        {
            MainWindow? myWindow = Application.Current.MainWindow as MainWindow;
            if (myWindow is null) return;
            myWindow.logout();
        }

        private void loadTimeTable(object sender, DependencyPropertyChangedEventArgs e)
        {
            UniformGrid? timeTable = sender as UniformGrid;
            if (timeTable is null || possibleSubjects is null) return;

            if (Equals(e.NewValue, true))
            {
                bool isStriped = false;
                for (int i = 0; i != 5; i++)
                {
                    int randomClassCount = myRandom.Next(possibleSubjects.Length, 7);
                    for (int j = 0; j != randomClassCount; j++)
                    {
                        string currentContent = "";
                        if (5 < myRandom.Next(0, 100)) currentContent = possibleSubjects[myRandom.Next(possibleSubjects.Length)];
                        Label newClassLabel = new Label
                        {
                            HorizontalContentAlignment = HorizontalAlignment.Center,
                            VerticalContentAlignment = VerticalAlignment.Center,
                            Style = (Style)ttTable.Resources["baseLabel"],
                            Background = isStriped ? stripeLightBlue : stripeDarkBlue,
                            Content = currentContent
                        };
                        timeTable.Children.Add(newClassLabel);
                    }
                    isStriped = !isStriped;
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
                for (int i = 0; i != loggedinUser.Subjects.Count; i++) {
                    UniformGrid generatedGradeCard = generateGradeCard(i, loggedinUser.Subjects[i]);
                    gradePanel.Children.Add(generatedGradeCard);
                }
            }
            else gradePanel.Children.Clear();
        }

        private void loadAbsences(object sender, DependencyPropertyChangedEventArgs e)
        {
            StackPanel? absencePanel = sender as StackPanel;
            if (absencePanel is null) return;

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

            if (Equals(e.NewValue, true))
            {
                Border generatedBorder = generateNewHomeworkCard();
                homeworkPanel.Children.Add(generatedBorder);
            }
            else homeworkPanel.Children.Clear();
        }

        private UniformGrid generateGradeCard(int index = 0, Subject? currSub = null, bool flipColors = false)
        {
            UniformGrid gridParent = new UniformGrid
            {
                Columns = 16,
                Rows = 1,
                Background = index % 2 == 0 ? stripeLightBlue : stripeDarkBlue,
            };
            Label indexLabel = new Label
            {
                Content = (index + 1).ToString()
            };
            Label nameLabel = new Label
            {
                Content = currSub?.Name
            };

            gridParent.Children.Add(indexLabel);
            gridParent.Children.Add(nameLabel);

            for (int j = 0; j != 13; j++)
            {
                string gradeContent = "-";
                if(j < currSub?.Marks.Count) gradeContent = currSub.Marks[j].ToString();

                Label gradeLabel = new Label
                {
                    Content = gradeContent
                };
                gridParent.Children.Add(gradeLabel);
            }

            Label avgLabel = new Label
            {
                Content = "-"
            };

            if (currSub?.Marks.Count > 0)
            {
                avgLabel.Content = Math.Round((decimal)(currSub?.AverageMark()), 2).ToString();
            }

            gridParent.Children.Add(avgLabel);
            return gridParent;
        }

        private Border generateNewAbsenceCard()
        {
            Border parentBorder = generateNewCard(["November 6", "Csütörtök", "-"]);
            return parentBorder;
        }

        private Border generateNewHomeworkCard()
        {
            Border parentBorder = generateNewCard(["2025. 11. 26", "Fizika", "TK 89/ 1. 2. 3."], true);
            return parentBorder;
        }

        private Border generateNewCard(string[] contents, bool isFlipped = false)
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
                Background = isFlipped ? darkBlue : lightBlue
            };
            parentGrid.Children.Add(stackParent);
            Label title = new Label()
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = contents[0],
                Foreground = isFlipped ? Brushes.White : darkBlue
            };
            Label title2 = new Label()
            {
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                Content = contents[1],
                Foreground = isFlipped ? Brushes.White : darkBlue
            };
            stackParent.Children.Add(title);
            stackParent.Children.Add(title2);

            Label desc = new Label()
            {
                Content = contents[2],
                HorizontalContentAlignment = HorizontalAlignment.Right,
                VerticalContentAlignment = VerticalAlignment.Center,
                Background = isFlipped ? lightBlue : darkBlue,
                Foreground = isFlipped ? darkBlue : Brushes.White

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
