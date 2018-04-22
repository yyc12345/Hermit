using HermitLib;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Text;

namespace HermitServer {

    public class ServerDatabase {

        public ServerDatabase() {

            //connect database
            var existed = false;

            existed = System.IO.File.Exists(General.serverConfig["userDatabasePath"]);
            userDatabaseCon = new SqliteConnection("Data Source=" + General.serverConfig["userDatabasePath"]);
            userDatabaseCon.Open();
            userDatabaseCmd = userDatabaseCon.CreateCommand();
            if (!existed) Generate(userDatabaseCmd, "user");
            ConsoleAssistance.WriteLine("[Database] Read user database successfully.");

            existed = System.IO.File.Exists(General.serverConfig["roomDatabasePath"]);
            roomDatabaseCon = new SqliteConnection("Data Source=" + General.serverConfig["roomDatabasePath"]);
            roomDatabaseCon.Open();
            roomDatabaseCmd = roomDatabaseCon.CreateCommand();
            if (!existed) Generate(roomDatabaseCmd, "room");
            ConsoleAssistance.WriteLine("[Database] Read room database successfully.");

            existed = System.IO.File.Exists(General.serverConfig["banDatabasePath"]);
            banDatabaseCon = new SqliteConnection("Data Source=" + General.serverConfig["banDatabasePath"]);
            banDatabaseCon.Open();
            banDatabaseCmd = banDatabaseCon.CreateCommand();
            if (!existed) Generate(banDatabaseCmd, "ban");
            ConsoleAssistance.WriteLine("[Database] Read ban database successfully.");

            existed = System.IO.File.Exists(General.serverConfig["emotionDatabasePath"]);
            emotionDatabaseCon = new SqliteConnection("Data Source=" + General.serverConfig["emotionDatabasePath"]);
            emotionDatabaseCon.Open();
            emotionDatabaseCmd = emotionDatabaseCon.CreateCommand();
            if (!existed) Generate(emotionDatabaseCmd, "emotion");
            ConsoleAssistance.WriteLine("[Database] Read emotion database successfully.");


        }

        SqliteConnection userDatabaseCon;
        SqliteConnection roomDatabaseCon;
        SqliteConnection banDatabaseCon;
        SqliteConnection emotionDatabaseCon;

        SqliteCommand userDatabaseCmd;
        SqliteCommand roomDatabaseCmd;
        SqliteCommand banDatabaseCmd;
        SqliteCommand emotionDatabaseCmd;

        public void Close() {
            userDatabaseCon.Close();
            roomDatabaseCon.Close();
            banDatabaseCon.Close();
            emotionDatabaseCon.Close();
        }

        public void Generate(SqliteCommand cmd, string type) {
            switch (type) {
                case "user":
                    cmd.CommandText = "CREATE TABLE user(name TEXT PRIMARY KEY NOT NULL,nickname TEXT NOT NULL,isAdmin INT NOT NULL,avatarGuid TEXT NOT NULL,salt1 TEXT NOT NULL,salt2 TEXT NOT NULL,saltHash TEXT NOT NULL";
                    break;
                case "room":
                    cmd.CommandText = "CREATE TABLE room(name TEXT PRIMARY KEY NOT NULL,type TEXT NOT NULL,password TEXT NOT NULL,host TEXT NOT NULL,users TEXT NOT NULL";
                    break;
                case "ban":
                    cmd.CommandText = "CREATE TABLE ban(value TEXT PRIMARY KEY NOT NULL,type TEXT NOT NULL";
                    break;
                case "emotion":
                    cmd.CommandText = "CREATE TABLE emotion(name TEXT PRIMARY KEY NOT NULL,emotionGuid TEXT NOT NULL";
                    break;
                default:
                    return;
            }

            cmd.ExecuteNonQuery();
            ConsoleAssistance.WriteLine("[Database] Generate new database file successfully.");
        }

        //todo:finish database operation
        #region user

        object lock_user_database = new Object();


        #endregion

        #region room

        object lock_room_database = new Object();


        #endregion

        #region ban

        object lock_ban_database = new Object();



        #endregion

        #region emotion

        object lock_emotion_database = new Object();



        #endregion

    }
}
