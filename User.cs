using System.Diagnostics;
using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Kreta_WPF
{
    public class User(int ID, string Name, string Password)
    {
        public int ID { get; } = ID;
        public string Name { get; set; } = Name;
        public string Password { get; set; } = Password;

        public bool Login(int ID, string Password)
        {
            return this.ID == ID && this.Password == Password;
        }

        public static List<T> ReadUsers<T>(string FilePath)
        {
            var JsonString = File.ReadAllText(FilePath);
            var Users = JsonConvert.DeserializeObject<List<T>>(JsonString);
            return Users!;
        }

        public static void WriteUsers<T>(string FilePath,  List<T> Users)
        {
            var JsonString = JsonConvert.SerializeObject(Users, Formatting.Indented);
            using var streamWriter = new StreamWriter(FilePath, false);
            streamWriter.Write(JsonString);
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
