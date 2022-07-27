using System;
using System.Collections.Generic;
using System.IO;
using System.IO.IsolatedStorage;
using SolutionCleaner.Core.Services.Contracts;

namespace SolutionCleaner.Core.Services
{
    public class LocalStorage : ILocalStorage
    {
        public readonly IsolatedStorageFile IsolatedStorage = IsolatedStorageFile.GetUserStoreForAssembly();

        private List<string> _existingStorageFiles = new ();

        public long FreeSpaceInBytes => IsolatedStorage.AvailableFreeSpace;

        public void WriteString(string name, string value)
        {
            DeleteItem(name);
            using var stream = new IsolatedStorageFileStream(name, FileMode.CreateNew, IsolatedStorage);
            using var writer = new StreamWriter(stream);
            writer.WriteLine(value);
        }

        public void WriteByteArray(string name, byte[] value)
        {
            DeleteItem(name);
            using (var stream = new IsolatedStorageFileStream(name, FileMode.CreateNew, IsolatedStorage))
            {
                stream.Write(value ?? Array.Empty<byte>(), 0, value?.Length ?? 0);
            }

            _existingStorageFiles.Add(name);
        }

        public void WriteDictionary(string name, Dictionary<string, string> dict)
        {
            DeleteItem(name);

            using var stream = new IsolatedStorageFileStream(name, FileMode.Create, IsolatedStorage);
            using var writer = new StreamWriter(stream);
            foreach (var pair in dict)
            {
                writer.WriteLine("{0}, {1}", pair.Key, pair.Value);
            }
        }

        public string ReadString(string name)
        {
            if (!IsolatedStorage.FileExists(name)) return null;

            using var stream = new IsolatedStorageFileStream(name, FileMode.Open, IsolatedStorage);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

        public byte[] ReadByteArray(string name)
        {
            if (!IsolatedStorage.FileExists(name)) return null;

            using var stream = new IsolatedStorageFileStream(name, FileMode.Open, IsolatedStorage);
            var result = new byte[(int)stream.Length];
            stream.Read(result, 0, (int)stream.Length);

            return result;
        }

        public Dictionary<string, string> ReadDictionary(string name)
        {
            if (!IsolatedStorage.FileExists(name)) return null;

            var dictionary = new Dictionary<string, string>();
            using var stream = new IsolatedStorageFileStream(name, FileMode.Open, IsolatedStorage);
            using var reader = new StreamReader(stream);
            string str;
            while ((str = reader.ReadLine()) != null)
            {
                var strArray = str.Split(new[] { ',' });
                dictionary.Add(strArray[0], strArray[1]);
            }
            return dictionary;
        }

        public void DeleteItem(string name)
        {
            if (IsolatedStorage.FileExists(name))
            {
                IsolatedStorage.DeleteFile(name);
            }
            if (_existingStorageFiles.Contains(name))
                _existingStorageFiles.Remove(name);
        }

        public bool ItemExists(string name)
        {
            return IsolatedStorage.FileExists(name);
        }

        public void ClearAllRemainingFiles()
        {
            foreach (var item in _existingStorageFiles)
            {
                if (IsolatedStorage.FileExists(item))
                    IsolatedStorage.DeleteFile(item);
            }

            _existingStorageFiles = new();
        }
    }
}
