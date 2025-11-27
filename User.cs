using Newtonsoft.Json;
using System.IO;

namespace Kreta_WPF
{
    internal class User(int ID, string Name, string Password)
    {
        private int ID { get; set; } = ID;
        public string Name { get; set; } = Name;
        private string Password { get; set; } = Password;

        public bool Login(string Password)
        {
            return this.Password == Password;
        }

        static public List<User> ReadUsers(string FilePath)
        {
            var JSONString = File.ReadAllText(FilePath);
            var Users = JsonConvert.DeserializeObject<List<User>>(JSONString);
            return Users!;
        }

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
