using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Newtonsoft.Json;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;

namespace Save
{
    public static class SaveFileWriter
    {
        private static readonly string SaveFilePath = Application.persistentDataPath + "/세이브.에오";

        public static void Save(PlayerSaveData playerSaveData)
        {
            // var formatter = new BinaryFormatter();
            // var stream = new FileStream(SaveFilePath, FileMode.OpenOrCreate);
            //
            // formatter.Serialize(stream, playerSaveData);
            // stream.Close();
            var stream = new FileStream(SaveFilePath, FileMode.OpenOrCreate);
            var data = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(playerSaveData));
            stream.Write(data, 0, data.Length);
            stream.Close();
        }
        
        public static PlayerSaveData Load()
        {
            if (!File.Exists(SaveFilePath))
            {
                return null;
            }

            // var formatter = new BinaryFormatter();
            // var stream = new FileStream(SaveFilePath, FileMode.Open);
            //
            // var saveData = formatter.Deserialize(stream) as PlayerSaveData;
            // stream.Close();
            // return saveData;
            var stream = new FileStream(SaveFilePath, FileMode.Open);
            var data = new byte[stream.Length];
            stream.Read(data, 0, data.Length);
            stream.Close();

            var jsonData = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<PlayerSaveData>(jsonData);

        }
        
        public static void Delete()
        {
            if (!File.Exists(SaveFilePath))
            {
                return;
            }

            File.Delete(SaveFilePath);
        }
    }
}
