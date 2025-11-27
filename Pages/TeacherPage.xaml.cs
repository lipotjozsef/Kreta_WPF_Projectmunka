using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    /// Interaction logic for TeacherPage.xaml
    /// </summary>
    public partial class TeacherPage : Page
    {
        Teacher? loggedInUser;
        public IEnumerable<FrameworkElement>? Frames = null;

        public TeacherPage(Teacher? loggedInUser)
        {
            this.loggedInUser = loggedInUser;
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
