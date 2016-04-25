using System;
using System.IO;
using UnityEngine;
using BigNumber;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace CapitalistTools
{
    class TimeTools
    {
        //Loads the last time you played from the file
        public static DateTime LoadTime()
        {
            string path = Path.Combine(Application.persistentDataPath, "time.dat");
            FileInfo file = new FileInfo(path);
            if (file.Exists == false)
                SaveTime();
            DateTime savedTime;
            using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
            {
                long binSaved = reader.ReadInt64();
                savedTime = DateTime.FromBinary(binSaved);
                reader.Close();
            }
            return savedTime;
        }
        //Saves the current time on ApplicationExit
        public static void SaveTime()
        {
            string path = Path.Combine(Application.persistentDataPath, "time.dat");
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                long binNow = DateTime.Now.ToBinary();
                writer.Write(binNow);
                writer.Close();
            }
        }
        public static TimeSpan TimeSpentOffline()
        {
            return DateTime.Now.Subtract(LoadTime());
        }
    }
    class MoneyTools
    {
        static Scales scales;
        //Load money from file
        public static StdBigNumber LoadMoney()
        {
            string path = Path.Combine(Application.persistentDataPath, "money.dat");
            FileInfo file = new FileInfo(path);
            if (file.Exists == false)
            {
                StdBigNumber money = new StdBigNumber(1);
                SaveMoney(money);
                return money;
            }
            else
                using (BinaryReader reader = new BinaryReader(File.Open(path, FileMode.Open)))
                {
                    string moneyText = reader.ReadString();
                    reader.Close();
                    return new StdBigNumber(moneyText);
                }
        }
        public static void SaveMoney(StdBigNumber money)
        {
            string path = Path.Combine(Application.persistentDataPath, "money.dat");
            using (BinaryWriter writer = new BinaryWriter(File.Create(path)))
            {
                string moneyText = money.ToString();
                writer.Write(moneyText);
                writer.Close();
            }
        }
        //Convert money to a formated string used alot in UI
        public static string GetFormatedMoneyText(StdBigNumber money)
        {
            int allDigits = money.DigitCount;
            if(allDigits <= 5)
                return money.ToString();
            else
            {
                int left = allDigits % 3;
                if (left == 0)
                    left = 3;
                string moneyText = "";
                for(int i = 0; i < left; i++)
                    moneyText += money.GetDigit(allDigits - i - 1).ToString();
                moneyText += ",";
                for (int i = 0; i < 2; i++)
                    moneyText += money.GetDigit(allDigits - left - i - 1).ToString();
                return moneyText;
            }
        }
        //returns a scale name
        public static string GetScaleName(string moneyInString)
        {
            StdBigNumber number = new StdBigNumber(moneyInString);
            int allDigits = number.DigitCount;
            if(allDigits > 5)
            {
                int left = allDigits % 3;
                if (left == 0)
                    left = 3;
                int scale = allDigits - left;
                return scales.scales[scale / 3 - 1];
            }
            return " ";
        }
        public static string GetScaleName(StdBigNumber money)
        {
            int allDigits = money.DigitCount;
            if (allDigits > 5)
            {
                int left = allDigits % 3;
                if (left == 0)
                    left = 3;
                int scale = allDigits - left;
                return scales.scales[scale / 3 - 1];
            }
            return " ";
        }
        public static void LoadScales()
        {
            TextAsset textAsset = Resources.Load("scales") as TextAsset;
            XmlSerializer serializer = new XmlSerializer(typeof(Scales));
            TextReader reader = new StringReader(textAsset.text);
            scales = serializer.Deserialize(reader) as Scales;
            reader.Close();
        }
    }
    class States
    {
        //Business stuff
        public static void SaveBusinessState(BusinessState state, string _name)
        {
            string path = Path.Combine(Application.persistentDataPath, "BusinessStates");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists == false)
                dir.Create();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Create(Path.Combine(path, _name + ".bstate"));
            bf.Serialize(stream, state);
            stream.Close();
        }
        public static BusinessState LoadBusinessState(string _name)
        {
            string path = Path.Combine(Application.persistentDataPath, "BusinessStates");
            string file = Path.Combine(path, _name + ".bstate");
            FileInfo info = new FileInfo(file);
            if (info.Exists == false)
                return null;
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Open(file, FileMode.Open);
            BusinessState state = new BusinessState();
            state = bf.Deserialize(stream) as BusinessState;
            stream.Close();
            return state;
        }

        //Managers stuff
        public static void SaveManagerState(ManagerState state, string _name)
        {
            string path = Path.Combine(Application.persistentDataPath, "ManagerStates");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists == false)
                dir.Create();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Create(Path.Combine(path, _name + ".mstate"));
            bf.Serialize(stream, state);
            stream.Close();
        }
        public static ManagerState LoadManagerState(string _name)
        {
            string path = Path.Combine(Application.persistentDataPath, "ManagerStates");
            string file = Path.Combine(path, _name + ".mstate");
            FileInfo info = new FileInfo(file);
            if (info.Exists == false)
                return new ManagerState();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Open(file, FileMode.Open);
            ManagerState state = bf.Deserialize(stream) as ManagerState;
            stream.Close();
            return state;
        }

        //Upgrades stuff
        public static void SaveUpgradeState(UpgradeState state, string _name)
        {
            string path = Path.Combine(Application.persistentDataPath, "UpgradeStates");
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists == false)
                dir.Create();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Create(Path.Combine(path, _name + ".ustate"));
            bf.Serialize(stream, state);
            stream.Close();
        }
        public static UpgradeState LoadUpgradeState(string _name)
        {
            string path = Path.Combine(Application.persistentDataPath, "UpgradeStates");
            string file = Path.Combine(path, _name + ".ustate");
            FileInfo info = new FileInfo(file);
            if (info.Exists == false)
                return new UpgradeState();
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = File.Open(file, FileMode.Open);
            UpgradeState state = new UpgradeState();
            state = bf.Deserialize(stream) as UpgradeState;
            stream.Close();
            return state;
        }
    }
}