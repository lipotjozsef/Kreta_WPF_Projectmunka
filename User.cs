using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta_WPF
{
    internal class User(int ID, string Name, string Password)
    {
        private int ID { get; set; } = ID;
        public string Name { get; set; } = Name;
        private string Password { get; set; } = Password;

        public override bool Equals(object? obj)
        {
            return ID == (obj as User)?.ID;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
