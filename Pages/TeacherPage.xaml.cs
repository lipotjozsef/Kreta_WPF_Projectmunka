using System.Linq;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace Kreta_WPF.Pages
{
    /// <summary>
    /// Interaction logic for TeacherPage.xaml
    /// </summary>
    public partial class TeacherPage : Page
    {
        Teacher? loggedInUser;
        public IEnumerable<FrameworkElement>? Frames = null;
        string[]? classDesignations = null;
        public TeacherPage(Teacher? loggedInUser)
        {
            this.loggedInUser = loggedInUser;
            classDesignations = MainWindow.classes.Where(x => x.Teachers.Contains(loggedInUser.ID)).Select(x => x.ClassDesignation).ToArray();
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

        private void loadStudents(object sender, RoutedEventArgs e)
        {
            cbStudents.Items.Clear();
            ComboBoxItem? typeItem = (ComboBoxItem)cbClasses.SelectedItem;
            if (typeItem is null) return;
            string? selectedDes = typeItem.Content.ToString();
            if(string.IsNullOrEmpty(selectedDes)) return;

            Class? selectedClass = MainWindow.classes.Where(x => x.ClassDesignation == selectedDes).ToArray()[0];
            if (selectedClass is null) return;
            foreach(int stuId in selectedClass.Students)
            {
                Student? currentStudent = MainWindow.students.Where(stu => stu.ID.Equals(stuId)).ToArray()[0];
                if (currentStudent is null) continue;
                ComboBoxItem newName = new ComboBoxItem
                {
                    Content = currentStudent.Name
                };
                cbStudents.Items.Add(newName);
            }
            cbStudents.SelectedIndex = 0;
        }

        private void loadClasses(object sender, DependencyPropertyChangedEventArgs e)
        {
            ComboBox? senderBox = sender as ComboBox;
            if (senderBox is null || classDesignations is null) return;
            lMessage.Content = string.Empty;
            absDp.SelectedDate = DateTime.Now;
            if (Equals(e.NewValue, true))
            {
                foreach(string Desclass in classDesignations)
                {
                    ComboBoxItem newItem = new ComboBoxItem
                    {
                        Content = Desclass
                    };
                    senderBox.Items.Add(newItem);
                }
                senderBox.SelectedIndex = 0;
            }
            else senderBox.Items.Clear();
        }

        private void tryNewAbsence(object sender, RoutedEventArgs e)
        {
            lMessage.Content = "Mulasztás sikeresen rögzítve!";
        }

        private void changeMinutes(int amount)
        {
            int currentAmount = int.Parse(minutesLate.Text);
            currentAmount = Math.Clamp(currentAmount + amount, 5, 45);
            minutesLate.Text = currentAmount.ToString();
        }

        private void changeMinutesLate(object sender, RoutedEventArgs e)
        {
            Button? senderButton = sender as Button;
            if (senderButton is null) return;
            int changeAmount = 0;
            int.TryParse(senderButton.Tag.ToString(), out changeAmount);
            changeMinutes(changeAmount);
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
