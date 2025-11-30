using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta_WPF
{
    public class Subject(string Name)
    {
        public string Name { get; set; } = Name;
        public List<int> Marks { get; set; } = [];

        public double AverageMark()
        {
            return Marks.Average();
        }

        public override bool Equals(object? obj)
        {
            if (obj == null) return false;
            if (obj is not Subject other) return false;
            
            return Name == other.Name;
        }
    }
}
