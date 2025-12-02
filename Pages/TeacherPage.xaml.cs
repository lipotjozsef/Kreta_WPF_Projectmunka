using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Kreta_WPF.Pages
{
    /// <summary>
    /// Interaction logic for TeacherPage.xaml
    /// </summary>
    public partial class TeacherPage : Page
    {
        private readonly Teacher loggedInUser;
        public IEnumerable<FrameworkElement>? Frames = null;
        string[] classDesignations = [];
        Dictionary<int, int> grading = new();
        public TeacherPage(Teacher loggedInUser)
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

        private void ReturnHome(object sender, RoutedEventArgs e)
        {
            MainWindow? myWindow = Application.Current.MainWindow as MainWindow;
            if (myWindow is null) return;
            myWindow.logout();
        }

        private Student[] selectedStudents(string? selectedDes)
        {
            if (selectedDes is null) return [];
            List<Student> selected = new();

            Class? selectedClass = MainWindow.classes.Where(x => x.ClassDesignation == selectedDes).ToArray()[0];
            if (selectedClass is null) return [];
            foreach (int stuId in selectedClass.Students)
            {
                Student? currentStudent = MainWindow.students.Where(stu => stu.ID.Equals(stuId)).ToArray()[0];
                selected.Add(currentStudent);
            }
            return selected.ToArray();
        }

        private void loadStudents(object sender, RoutedEventArgs e)
        {
            cbStudents.Items.Clear();
            ComboBoxItem? typeItem = (ComboBoxItem)cbClasses.SelectedItem;
            if (typeItem is null) return;
            string? selectedDes = typeItem.Content.ToString();
            if(string.IsNullOrEmpty(selectedDes)) return;

            Student[] myStudents = selectedStudents(selectedDes);
            foreach (Student student in myStudents) {
                if (student is null) continue;
                ComboBoxItem newName = new ComboBoxItem
                {
                    Content = student.Name,
                    Tag = student.ID.ToString()
                };
                cbStudents.Items.Add(newName);
            }

            /*Class? selectedClass = MainWindow.classes.Where(x => x.ClassDesignation == selectedDes).ToArray()[0];
            if (selectedClass is null) return;
            foreach(int stuId in selectedClass.Students)
            {
                Student? currentStudent = MainWindow.students.Where(stu => stu.ID.Equals(stuId)).ToArray()[0];
                if (currentStudent is null) continue;
                ComboBoxItem newName = new ComboBoxItem
                {
                    Content = currentStudent.Name,
                    Tag = currentStudent.ID.ToString()
                };
                cbStudents.Items.Add(newName);
            }*/
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
                loadSubjects();
            }
            else senderBox.Items.Clear();
        }

        private void loadSubjects()
        {
            lMessage.Content = string.Empty;
            absDp.SelectedDate = DateTime.Now;
            grSubjectCB.Items.Clear();

            string ClassDesignation = grClassCB.Text;
            var SelectedClass = MainWindow.classes.First(x => x.ClassDesignation == ClassDesignation);
            foreach (var ClassSubjectsAndTeachers in SelectedClass.SubjectsAndTeachers.Where(x => x.Value == loggedInUser.ID))
            {
                ComboBoxItem newItem = new ComboBoxItem
                {
                    Content = ClassSubjectsAndTeachers.Key
                };
                grSubjectCB.Items.Add(newItem);
            }
            grSubjectCB.SelectedIndex = 0;
        }

        private void tryNewAbsence(object sender, RoutedEventArgs e)
        {
            Button? senderButton = (Button)sender;
            if (senderButton is null || !senderButton.IsEnabled) return;

            senderButton.IsEnabled = false;

            string classDes = cbClasses.Text.Trim();
            int studentID = -1;
            int.TryParse((cbStudents.SelectedItem as ComboBoxItem)?.Tag.ToString(), out studentID);
            int late = -1;
            int.TryParse(minutesLate.Text.ToString(), out late);
            DateTime lateDate = (DateTime)absDp.SelectedDate;

            bool setNewAbs = newAbsence(classDes, studentID, late, lateDate);
            if(setNewAbs) lMessage.Content = "A mulasztás sikeresen rögzítve!";
            else lMessage.Content = "A mulasztás rögzítése félre ment!";

            MainWindow.DelayAction(1500, new Action(() => { senderButton.IsEnabled = true; }));
        }

        private bool newAbsence(string classDes, int stuID, int minutes, DateTime dpDate)
        {
            return true;
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

        private void loadStudentsGrade(object sender, RoutedEventArgs e)
        {
            ComboBox? senderPanel = sender as ComboBox;
            if (senderPanel?.SelectedItem is null) return;
            ComboBoxItem? typeItem = (ComboBoxItem)grClassCB.SelectedItem;
            if (typeItem is null) return;
            string? selectedDes = typeItem.Content.ToString();
            if (string.IsNullOrEmpty(selectedDes)) return;

            gradeGrid.Children.Clear();
            grading.Clear();
            Student[] myStudents = selectedStudents(selectedDes);

            for(int stuIndex = 0; stuIndex != myStudents.Length; stuIndex++)
            {
                UniformGrid newStudentGrid = newStudentGrids(stuIndex+1, myStudents[stuIndex]);
                gradeGrid.Children.Add(newStudentGrid);
            }
        }

        private void appendNewGrade(Student myStudent, RadioButton selectedGrade)
        {

            int grade = -1;
            int.TryParse(selectedGrade.Content.ToString(), out grade);
            if (grade == -1) return;
            grading[myStudent.ID] = grade;
        }

        private UniformGrid newStudentGrids(int index, Student student)
        {
            UniformGrid newGrid = new UniformGrid
            {
                Columns = 4
            };

            CheckBox newCheck = new() {
                IsChecked = true
            };
            newGrid.Children.Add(newCheck);

            Label indexLabel = new Label
            {
                Content = index.ToString()
            };
            newGrid.Children.Add(indexLabel);

            Label nameLabel = new Label
            {
                Content = student.Name
            };
            newGrid.Children.Add(nameLabel);

            StackPanel newPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                VerticalAlignment = VerticalAlignment.Center
            };
            newGrid.Children.Add(newPanel);
            
            for (int j = 0; j != 5; j++)
            {
                RadioButton newButton = new RadioButton { Content = (j+1).ToString(), Style = (Style)grFrame.Resources["gradeButton"] };
                newButton.Click += (sender, e) =>
                {
                    if(Equals(newCheck.IsChecked, true))
                    {
                        appendNewGrade(student, newButton);
                    }
                };
                newPanel.Children.Add(newButton);
                if (j == 0) newButton.IsChecked = true;
            }
            return newGrid;
        }

        private void newGrading(object sender, RoutedEventArgs e)
        {
            foreach (var grade in grading) 
                MainWindow.students.First(x => x.ID == grade.Key).Subjects.First(x => x.Name == grSubjectCB.Text).Marks.Add(grade.Value);
        }

        private void tryNewHomework(object sender, RoutedEventArgs e)
        {

        }
    }
}
