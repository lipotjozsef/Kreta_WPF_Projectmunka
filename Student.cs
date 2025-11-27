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
            var strings = new List<string>();
            foreach (var item in this.Subjects)
            {
                strings.Add(item.Name);
            }

            foreach (var subject in Subjects.Where(subject => !strings.Contains(subject)))
                this.Subjects.Add(new Subject(subject));
        }

        public override bool Equals(object? obj)
        {
            return this.Name == (obj as Student).Name;
        }
    }
}
