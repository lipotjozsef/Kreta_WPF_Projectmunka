using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kreta_WPF
{
    internal class User(int ID, string Name, string Password)
    {
        public int ID { get; set; } = ID;
        public string Name { get; set; } = Name;
        public string Password { get; set; } = Password;
    }
}
