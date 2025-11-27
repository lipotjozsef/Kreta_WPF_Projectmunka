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
        // Temp Only!
        string[]? possibleSubjects = null;
        Random myRandom = new Random();

        private Student? loggedinUser = null;

        public StudentPage(Student? loggedinUser)
        {
            this.loggedinUser = loggedinUser;
            possibleSubjects = loggedinUser?.Subjects.Select(x => x.Name).ToArray();
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

        private void loadTimeTable(object sender, DependencyPropertyChangedEventArgs e)
        {
            UniformGrid? timeTable = sender as UniformGrid;
            if (timeTable is null || possibleSubjects is null) return;

            if (Equals(e.NewValue, true))
            {
                for (int i = 0; i != 5; i++)
                {
                    int randomClassCount = myRandom.Next(possibleSubjects.Length, 7);
                    for (int j = 0; j != randomClassCount; j++)
                    {
                        string currentContent = "";
                        if (15 < myRandom.Next(0, 100)) currentContent = possibleSubjects[myRandom.Next(possibleSubjects.Length)];
                        else j--;
                            Label newClassLabel = new Label
                            {
                                HorizontalContentAlignment = HorizontalAlignment.Center,
                                VerticalContentAlignment = VerticalAlignment.Center,
                                Style = (Style)ttTable.Resources["baseLabel"],
                                Background = j % 2 == 0 ? Brushes.WhiteSmoke : Brushes.White,
                                Content = currentContent
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
            if (gradePanel is null || possibleSubjects is null) return;

            if (Equals(e.NewValue, true))
            {
                for (int i = 0; i != possibleSubjects.Length; i++) {
                    UniformGrid generatedGradeCard = generateGradeCard(i, possibleSubjects[i]);
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
            Border parentBorder = generateNewCard(["2025. 11. 26", "Fizika", "TK 89/ 1. 2. 3."], true);
            return parentBorder;
        }

        private Border generateNewCard(string[] contents, bool isFlipped = false)
        {
            Brush bgBrush = isFlipped ? Brushes.White : Brushes.Gray;
            Brush fgBrush = isFlipped ? Brushes.Black : Brushes.WhiteSmoke;

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
                Background = isFlipped ? Brushes.White : Brushes.Gray
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
                Background = isFlipped ? Brushes.White : Brushes.Gray,
                Foreground = isFlipped ? Brushes.Black : Brushes.WhiteSmoke

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
