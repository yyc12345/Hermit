using System;
using System.Collections.Generic;
using System.Text;
using HermitLib;
using System.IO;
using Newtonsoft.Json;

namespace HermitServer {

    public class ServerConfig {

        public ServerConfig(string json_file) {
            file_path = json_file;

            if (!File.Exists(json_file)) {
                ConsoleAssistance.WriteLine("[Config] Generate default config...");
                Generate();
            }

            read:
            var file = new StreamReader(file_path, Information.UniversalEncoding);
            var str = file.ReadToEnd();
            file.Close();

            try {
                config = JsonConvert.DeserializeObject<ServerConfigItem>(str);
                ConsoleAssistance.WriteLine("[Config] Read config successfully.");
            } catch (Exception) {
                Generate();
                goto read;
            }
        }

        object lock_config = new Object();
        string file_path;
        ServerConfigItem config;

        public void Generate() {
            var cache = new ServerConfigItem() {
                userDatabasePath = Information.WorkPath.Enter("user.db").Path(),
                roomDatabasePath = Information.WorkPath.Enter("room.db").Path(),
                banDatabasePath = Information.WorkPath.Enter("ban.db").Path(),
                emotionDatabasePath = Information.WorkPath.Enter("emotion.db").Path(),
                ipv4Port = "8686",
                ipv6Port = "6161"
            };

            var file = new StreamWriter(file_path, false, Information.UniversalEncoding);
            file.Write(JsonConvert.SerializeObject(cache));
            file.Close();
        }

        public void Save() {
            lock (lock_config) {
                var file = new StreamWriter(file_path, false, Information.UniversalEncoding);
                file.Write(JsonConvert.SerializeObject(config));
                file.Close();
            }
        }

        public string this[string key] {
            get {
                switch (key) {
                    case "userDatabasePath":
                        return config.userDatabasePath;
                    case "roomDatabasePath":
                        return config.roomDatabasePath;
                    case "banDatabasePath":
                        return config.banDatabasePath;
                    case "emotionDatabasePath":
                        return config.emotionDatabasePath;
                    case "ipv4Port":
                        return config.ipv4Port;
                    case "ipv6Port":
                        return config.ipv6Port;
                    default:
                        return "";
                }
            }
        }


        public void Change(string key, string value) {
            lock (lock_config) {
                switch (key) {
                    case "userDatabasePath":
                        config.userDatabasePath = value;
                        break;
                    case "roomDatabasePath":
                        config.roomDatabasePath = value;
                        break;
                    case "banDatabasePath":
                        config.banDatabasePath = value;
                        break;
                    case "emotionDatabasePath":
                        config.emotionDatabasePath = value;
                        break;
                    case "ipv4Port":
                        config.ipv4Port = value;
                        break;
                    case "ipv6Port":
                        config.ipv6Port = value;
                        break;
                    default:
                        break;
                }
            }

            Save();
        }

    }

    //todo:finish config
    public class ServerConfigItem {
        public string userDatabasePath { get; set; }
        public string roomDatabasePath { get; set; }
        public string banDatabasePath { get; set; }
        public string emotionDatabasePath { get; set; }

        public string ipv4Port { get; set; }
        public string ipv6Port { get; set; }
    }

}
