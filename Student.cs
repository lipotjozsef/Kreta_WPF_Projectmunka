using Newtonsoft.Json;

namespace Kreta_WPF
{
    public class Student(int ID, string Name, string Password) : User(ID, Name, Password)
    {
        [JsonProperty(Order = 1)]
        public List<Subject> Subjects { get; set; } = [];

        [JsonProperty(Order = 2)]
        public Dictionary<DateTime, int> Abscences { get; } = [];

        public double AverageMark() => Subjects.Where(x => x.AverageMark() != 0).Average(x => x.AverageMark());

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

        public void SetOrChangeAbsence(DateTime TimeOfAbsence, int MinutesLate)
        {
            Abscences[TimeOfAbsence] = MinutesLate;
        }

        public bool RemoveAbsence(DateTime TimeOfAbsence)
        {
            return Abscences.Remove(TimeOfAbsence);
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not Student other) return false;
            
            return ID == other.ID;
        }
    }
}
