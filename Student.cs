namespace Kreta_WPF
{
    internal class Student(int ID, string Name, string Password) : User(ID, Name, Password)
    {
        public List<Subject> Subjects { get; set; }

        public double AverageMark()
        {
            return Subjects.Average(x => x.AverageMark());
        }
    }
}
