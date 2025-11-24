using System.Text.Json.Serialization;

namespace Kreta_WPF
{
    internal class Class
    {
        [JsonConstructor]
        public Class(string ClassDesignation, Dictionary<string, Teacher> SubjectsAndTeachers, List<Student> Students)
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
        public Dictionary<string, Teacher> SubjectsAndTeachers { get; set; }
        public List<string> Subjects { get; set; }
        public List<Teacher> Teachers { get; set; }
        public List<Student> Students { get; set; }
    }
}
