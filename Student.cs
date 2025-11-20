using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta_WPF
{
    internal class Student(int ID, string Name, string Password, List<Subject> Subjects) : User(ID, Name, Password)
    {
        public List<Subject> Subjects { get; set; } = Subjects;

        public double AverageMark()
        {
            return Subjects.Average(x => x.AverageMark());
        }
    }
}
