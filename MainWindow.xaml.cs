using Kreta_WPF.Pages;
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
        static string studentPath = "./JSONData/Students.json";
        static string teacherPath = "./JSONData/Teachers.json";
        static string classesPath = "./JSONData/Classes.json";
        static List<Student> students = User.ReadUsers<Student>(studentPath);
        static List<Teacher> teachers = User.ReadUsers<Teacher>(teacherPath);
        static List<Class> classes = Class.ReadClasses(classesPath);

        Student? loggedinUser = null;
        public MainWindow()
        {
            InitializeComponent();
            foreach(Class myClass in classes)
            {
                myClass.LoadSubjects(students);
            }
            /*Debug.WriteLine("\tPrinting Frames");
            foreach (var f in Frames) Debug.WriteLine($"\t\t{f.Name}");*/
        }

        static public IEnumerable<FrameworkElement> getElementsByTag(DependencyObject parent, object tag)
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
                int userid = -1;
                int.TryParse(loginUserID.Text, out userid);
                string password = loginPassword.Password;

                if (userid == -1) writeError("A felhasználó név megadása kötelező a bejelentkezéshez!", loginUserID);
                if (string.IsNullOrEmpty(password)) writeError("A jelszó megadása kötelező a bejelentkezéshez!", loginPassword);

                bool loggedIn = tryLogin(userid, password);
                if (loggedIn)
                {
                    loginPage.Visibility = Visibility.Collapsed;
                    activePage.Visibility = Visibility.Visible;
                    Page? newMainPage = null;
                    /*if (loggedinUser is Teacher) newMainPage = new TeacherPage(loggedinUser as Teacher);
                    else if(loggedinUser is Student) newMainPage = new StudentPage(loggedinUser as Student);

                    MainFrame.Navigate(newMainPage);*/
                    MainFrame.Navigate(new StudentPage(loggedinUser));
                }
                else writeError("A megadott felhasználónév/jelszó párral nem található felhasználó a rendszerben.", null);
            }
            catch (ArgumentNullException) { }
        }

        private bool tryLogin(int id, string ps)
        {
            foreach (Student user in students) {
                if (user.Login(id, ps))
                {
                    loggedinUser = user;
                    return true;
                }
            }

            /*List<Teacher> teachers = User.ReadUsers<Teacher>(teacherPath);

            foreach (User user in teachers) {
                if (user.Login(id, ps))
                {
                    loggedinUser = user;
                    return true;
                }
            }*/
            return false;
        }

        private void emptyLabel(object sender, RoutedEventArgs e)
        {
            TextBlock? myObject = sender as TextBlock;
            if(myObject is TextBlock) myObject.Text = "";
        }
    }
}