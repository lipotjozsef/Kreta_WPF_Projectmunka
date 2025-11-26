using System.Text.Json.Serialization;

namespace Kreta_WPF
{
    internal class Class
    {
        [JsonConstructor]
        public Class(string ClassDesignation, Dictionary<string, int> SubjectsAndTeachers, List<int> Students)
        {
            this.ClassDesignation = ClassDesignation;
            this.SubjectsAndTeachers = SubjectsAndTeachers;
            Subjects = []; Teachers = [];

            foreach (var item in SubjectsAndTeachers)
            {
                Subjects.Add(item.Key);
                if (!Teachers.Contains(item.Value))
                    Teachers.Add(item.Value);
            }

            this.Students = Students;
        }

        public string ClassDesignation { get; set; }
        public Dictionary<string, int> SubjectsAndTeachers { get; set; }
        public List<string> Subjects { get; set; }
        public List<int> Teachers { get; set; }
        public List<int> Students { get; set; }
    }
}
