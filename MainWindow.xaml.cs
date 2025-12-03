using Kreta_WPF.Pages;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;

namespace Kreta_WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string studentPath = "./JSONData/Students.json";
        public static List<Student> students = User.ReadUsers<Student>(studentPath);

        public static string teacherPath = "./JSONData/Teachers.json";
        public static List<Teacher> teachers = User.ReadUsers<Teacher>(teacherPath);

        public static string classesPath = "./JSONData/Classes.json";
        public static List<Class> classes = Class.ReadClasses(classesPath);

        public MainWindow()
        {
            InitializeComponent();
            loginUserID.Focus();
            foreach (Class myClass in classes)
            {
                myClass.LoadSubjects(students);
            }

            MainFrame.Navigated += (s, e) =>
            {
                MainFrame.NavigationService.RemoveBackEntry();
            };
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

        public static void DelayAction(int millisecond, Action action)
        {
            var timer = new DispatcherTimer();
            timer.Tick += delegate

            {
                action.Invoke();
                timer.Stop();
            };

            timer.Interval = TimeSpan.FromMilliseconds(millisecond);
            timer.Start();
        }

        private void revertBorder(object sender, RoutedEventArgs e)
        {
            Control? myControl = sender as Control;
            if (myControl != null) myControl.BorderBrush = Brushes.Gray;
        }
        private void loginPanelKeydown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter) submitLogin(sender, e);
        }

        private void submitLogin(object sender, RoutedEventArgs e)
        {
            Action<string, Control?> writeError = (string errorText, Control? elementToChange) =>
            {
                errorBlock.Text = errorText;
                if (elementToChange != null) elementToChange.BorderBrush = new SolidColorBrush(
                    Color.FromRgb(235, 69, 95)
                    );
                throw new ArgumentNullException(errorText);
            };

            try
            {
                int userid = -1;
                int.TryParse(loginUserID.Text.Trim(), out userid);
                string password = loginPassword.Password.Trim();

                if (userid == -1 || userid == 0) writeError("Az azonosító megadása kötelező a bejelentkezéshez!", loginUserID);
                if (string.IsNullOrEmpty(password)) writeError("A jelszó megadása kötelező a bejelentkezéshez!", loginPassword);

                User? loggedinUser = tryLogin(userid, password);
                if (!Equals(loggedinUser, null))
                {
                    loginPage.Visibility = Visibility.Collapsed;
                    activePage.Visibility = Visibility.Visible;
                    Page? newMainPage = null;
                    if (loggedinUser is Teacher) newMainPage = new TeacherPage(loggedinUser as Teacher);
                    else if(loggedinUser is Student) newMainPage = new StudentPage(loggedinUser as Student);

                    MainFrame.Navigate(newMainPage);
                }
                else writeError("A megadott felhasználónév/jelszó párral nem található felhasználó a rendszerben.", null);
            }
            catch (ArgumentNullException) { }
        }

        private User? tryLogin(int id, string ps)
        {
            foreach (Student stu in students) {
                if (stu.Login(id, ps)) return stu;
            }

            foreach (Teacher teach in teachers) {
                if (teach.Login(id, ps)) return teach;
            }

            return null;
        }

        public void logout()
        {
            MainFrame.Content = null;
            loginPage.Visibility = Visibility.Visible;
            activePage.Visibility = Visibility.Collapsed;
        }

        private void emptyLabel(object sender, DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement? myObject = sender as FrameworkElement;
            if(myObject is TextBlock) (myObject as TextBlock).Text = "";
            if(myObject is PasswordBox) (myObject as PasswordBox).Password = "";
            if (myObject is TextBox) (myObject as TextBox).Text= "";
        }
    }
}