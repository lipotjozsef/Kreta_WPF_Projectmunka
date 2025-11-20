using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta_WPF
{
    internal class Class
    {
        public Class(Dictionary<Subject, Teacher> SubjectsAndTeachers, List<Student> Students)
        {
            this.SubjectsAndTeachers = SubjectsAndTeachers;

            foreach (var item in SubjectsAndTeachers)
            {
                Subjects.Add(item.Key);
                if (!Teachers.Contains(item.Value))
                    Teachers.Add(item.Value);
            }

            this.Students = Students;
        }
        public Dictionary<Subject, Teacher> SubjectsAndTeachers { get; set; }
        public List<Subject> Subjects { get; set; } = [];
        public List<Teacher> Teachers { get; set; } = [];
        public List<Student> Students { get; set; }
    }
}
