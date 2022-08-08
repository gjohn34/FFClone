using FFClone.Models;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Xml.Serialization;

namespace FFClone
{
    [Serializable]
    public struct SaveGame
    {
        public List<Hero> Party;
        public EncounterInfo EncounterInfo;
        public Inventory Inventory;
    }
    public sealed class SaveFile
    {
        private static SaveFile instance = null;
        private static readonly object padlock = new object();
        private static SaveGame _saveData;

        SaveFile()
        {
        }

        public static SaveFile Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new SaveFile();
                    }
                    return instance;
                }
            }
        }
        private static IsolatedStorageFileStream _isolatedFileStream;
        private static IsolatedStorageFile _dataFile;

        public static SaveGame Load()
        {
            _dataFile = IsolatedStorageFile.GetUserStoreForDomain();
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
            SaveGame saveData = new SaveGame();
            if (!_dataFile.FileExists("file.sav"))
            {
                Debug.WriteLine("File not found - start new game");
            }
            using (_isolatedFileStream = _dataFile.OpenFile("file.sav", FileMode.Open, FileAccess.ReadWrite))
            {
                saveData = (SaveGame)serializer.Deserialize(_isolatedFileStream);
                saveData.Party = new List<Hero>(){
                    new Hero("John", Job.Warrior, Color.Red, "Sprites/Idles/warrior-idle", "Sprites/Portraits/1p"),
                    new Hero("Luke", Job.Mage, Color.Blue, "Sprites/Idles/mage-idle", "Sprites/Portraits/2p"),
                    new Hero("Paul", Job.Thief, Color.Green, "Sprites/Idles/ninja-idle", "Sprites/Portraits/3p")
                };
                GameInfo.Instance.Initialize(saveData);
                // Loop through nested Lists
                _dataFile.Close();
                _isolatedFileStream.Close();
            }
            //new Hero("John", Job.Warrior, Color.Red, "Sprites/ninja-idle", "Sprites/Portraits/1p")
            return saveData;
        }

        internal static void New()
        {

            GameInfo.Instance.Initialize(new SaveGame
{
                Party = new List<Hero>()
                {
                    new Hero("John", Job.Warrior, Color.Red, "Sprites/mage-idle", "Sprites/Portraits/1p")
                },
                EncounterInfo = new EncounterInfo(),
                Inventory = new Inventory(new List<Item>()
                {
                    new Item(1, "Potion", 1),
                    new Item(2, "Hi-Potion", 1),
                    new Item(4, "Phoenix Down", 1)
                })
            });
        }

        public static void Save()
        {
            _dataFile = IsolatedStorageFile.GetUserStoreForDomain();
            //_dataFile = IsolatedStorageFileIsolatedStorageFile.GetStore(IsolatedStorageScope.Application, null, null);
            XmlSerializer serializer = new XmlSerializer(typeof(SaveGame));
            if (_dataFile.FileExists("file.sav"))
            {
                Debug.WriteLine("File found - deleting it");
                _dataFile.DeleteFile("file.sav");
            }
            using (_isolatedFileStream = _dataFile.CreateFile("file.sav"))
            {
                _isolatedFileStream.Seek(0, SeekOrigin.Begin);

                SaveGame gameData = GameInfo.Instance.ToSave;

                serializer.Serialize(_isolatedFileStream, gameData);

                _isolatedFileStream.SetLength(_isolatedFileStream.Position);
            }
            _dataFile.Close();
            _isolatedFileStream.Dispose();
        }
    }
}