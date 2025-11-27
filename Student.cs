namespace Kreta_WPF
{
    public class Student(int ID, string Name, string Password) : User(ID, Name, Password)
    {
        public List<Subject> Subjects { get; set; } = [];

        public double AverageMark()
        {
            return Subjects.Average(x => x.AverageMark());
        }

        public void AddSubjects(List<string> Subjects)
        {
            foreach (var subject in Subjects.Where(subject => !Subjects.Contains(subject)))
                this.Subjects.Add(new Subject(subject));
        }
    }
}
