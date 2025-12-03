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
        DateTime? selectedAbsenceDate = null;
        int? selectedAbsenceMinutes = null;
        bool loadingAbsence = false;
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

        private void loadAbsences(object sender, RoutedEventArgs e)
        {
            selectedAbsenceDate = null;
            selectedAbsenceMinutes = null;
            absenceSelector.Children.Clear();
            if (cbStudentBrowser.Items.Count == 0) return;
            int studentID = -1;
            int.TryParse((cbStudentBrowser.SelectedItem as ComboBoxItem)?.Tag.ToString(), out studentID);
            Student selectedStudent = MainWindow.students.First(x => x.ID == studentID);

            if (selectedStudent.Abscences.Count == 0)
            {
                Label noAbsenceLabel = new Label
                {
                    Content = "Nincs mulasztása rögzített mulasztása!",
                    FontSize = 20
                };
                absenceSelector.Children.Add(noAbsenceLabel);
                return;
            }

            foreach (KeyValuePair<DateTime, int> absences in selectedStudent.Abscences)
            {
                RadioButton newAbsence = new RadioButton
                {
                    Content = $"{absences.Key.Year}. {absences.Key.Month}. {absences.Key.Day} - {absences.Value} perc késés",
                };
                newAbsence.Click += (sender, e) =>
                {
                    selectedAbsenceDate = absences.Key;
                    selectedAbsenceMinutes = absences.Value;
                };
                absenceSelector.Children.Add(newAbsence);
            }
        }

        private void selectAbsence(object sender, RoutedEventArgs e)
        {
            cbStudents.Items.Clear();
            cbClasses.Items.Clear();

            ComboBoxItem selectedClass = (ComboBoxItem)cbClassBrowser.SelectedItem;
            cbClassBrowser.Items.Remove(selectedClass);
            cbClasses.Items.Add(selectedClass);

            ComboBoxItem selectedStudent = (ComboBoxItem)cbStudentBrowser.SelectedItem;
            cbStudentBrowser.Items.Remove(selectedStudent);
            cbStudents.Items.Add(selectedStudent);

            absDp.SelectedDate = selectedAbsenceDate;
            minutesLate.Text = selectedAbsenceMinutes.ToString();

            stepBack(sender, e);
            changeVisiblity(absencePanel, AbsenceMenu);
        }

        private void loadStudents(object sender, RoutedEventArgs e)
        {
            if (loadingAbsence)
            {
                cbStudents.IsReadOnly = true;
                cbClasses.IsReadOnly = true;
                return;
            }
            else
            {
                cbStudents.IsReadOnly = false;
                cbClasses.IsReadOnly = false;
            }

            cbStudents.Items.Clear();
            cbStudentBrowser.Items.Clear();
            ComboBox? senderBox = sender as ComboBox;
            if (senderBox is null) return;

            ComboBox? selectedBox = null;
            switch ((senderBox.Parent as StackPanel).Name)
            {
                case ("absencePanel"):
                    selectedBox = cbStudents;
                    break;
                case ("absenceBrowser"):
                    selectedBox = cbStudentBrowser;
                    break;
            }

            ComboBoxItem? typeItem = (ComboBoxItem)senderBox.SelectedItem;
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

                selectedBox.Items.Add(newName);
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
            selectedBox.SelectedIndex = 0;
        }

        private void loadClasses(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (loadingAbsence) return;
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
                loadSubjects(senderBox);
            }
            else senderBox.Items.Clear();
        }

        private void loadSubjects(ComboBox sender)
        {
            if (loadingAbsence) return;
            lMessage.Content = string.Empty;
            absDp.SelectedDate = DateTime.Now;
            grSubjectCB.Items.Clear();

            string ClassDesignation = sender.Text;
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

        private void menuVisible(object sender, DependencyPropertyChangedEventArgs e)
        {
            absenceTitle.Content = "Mulasztások kezelése";
        }

        private void showAbsenceList(object sender, EventArgs e)
        {
            AbsenceMenu.Visibility = Visibility.Collapsed;
            absenceBrowser.Visibility = Visibility.Visible;
            loadingAbsence = true;
        }

        private void stepBack(object sender, EventArgs e)
        {
            absencePanel.Visibility = Visibility.Collapsed;
            absenceBrowser.Visibility= Visibility.Collapsed;
            AbsenceMenu.Visibility = Visibility.Visible;
        }

        private void changeVisiblity(FrameworkElement work1, FrameworkElement work2)
        {
            work1.Visibility = Visibility.Visible;
            work2.Visibility = Visibility.Collapsed;
        }

        private void createNewAbsence(object sender, EventArgs e)
        {
            loadingAbsence = false;
            changeVisiblity(absencePanel, AbsenceMenu);
            absenceTitle.Content = "Új Mulasztás Rögzítése";
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
            try
            {
                Student selectedStudent = MainWindow.students.First(x => x.ID == stuID);
                selectedStudent.SetOrChangeAbsence(dpDate, minutes);
                Student.WriteUsers(MainWindow.studentPath, MainWindow.students);
                return true;
            }
            catch (Exception e) { return false; }
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
                if (j == 0)
                {
                    appendNewGrade(student, newButton);
                    newButton.IsChecked = true;
                }
            }
            return newGrid;
        }

        private void newGrading(object sender, RoutedEventArgs e)
        {
            foreach (var grade in grading) 
                MainWindow.students.First(x => x.ID == grade.Key).Subjects.First(x => x.Name == grSubjectCB.Text).Marks.Add(grade.Value);
            User.WriteUsers(MainWindow.studentPath, MainWindow.students);
        }

        private void tryNewHomework(object sender, RoutedEventArgs e)
        {

        }
    }
}
