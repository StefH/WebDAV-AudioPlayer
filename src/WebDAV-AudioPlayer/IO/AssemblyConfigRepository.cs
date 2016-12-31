using System.IO;
using Newtonsoft.Json;

namespace WebDav.AudioPlayer.IO
{
    public abstract class AssemblyConfigRepository<T> where T : AssemblyConfigRepository<T>, new()
    {
        private static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            NullValueHandling = NullValueHandling.Ignore,
            //DefaultValueHandling = DefaultValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor,
            ContractResolver = new InternalResolver()
        };

        /// <summary>
        /// Value indicating whether this settings has been loaded from file or is the default.
        /// </summary>
        public bool IsDefault { get; private set; }

        protected AssemblyConfigRepository()
        {
            IsDefault = true;
        }

        /// <summary>
        /// Loads the settings from the persistant storage.
        /// </summary>
        /// <returns>Settings.</returns>
        public static T Load()
        {
            var path = GetPath();
            if (File.Exists(path))
            {
                var text = File.ReadAllText(path);
                if (!string.IsNullOrWhiteSpace(text))
                {
                    var obj = JsonConvert.DeserializeObject<T>(text, Settings);
                    obj.IsDefault = false;
                    return obj;
                }
            }
            return new T();
        }

        private static string GetPath()
        {
            var fullPath = typeof(T).Assembly.Location;
            return Path.ChangeExtension(fullPath, "jconfig");
        }

        /// <summary>
        /// Saves the settings to a persistant storage.
        /// </summary>
        public void Save()
        {
            var serialized = JsonConvert.SerializeObject(this, Settings);

            var path = GetPath();
            if (File.Exists(path))
                File.Delete(path);
            File.WriteAllText(path, serialized);
            IsDefault = false;
        }
    }
}