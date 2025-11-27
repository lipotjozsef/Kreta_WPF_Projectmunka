using Newtonsoft.Json;
using System.IO;

namespace Kreta_WPF
{
    public class User(int ID, string Name, string Password)
    {
        public int ID { get; } = ID;
        public string Name { get; set; } = Name;
        private string Password { get; set; } = Password;

        public bool Login(string Password)
        {
            return this.Password == Password;
        }

        public static List<User> ReadUsers(string FilePath)
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
