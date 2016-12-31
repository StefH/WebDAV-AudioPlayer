using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace WebDav.AudioPlayer.IO
{
    internal sealed class InternalResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty prop = base.CreateProperty(member, memberSerialization);

            var propertyInfo = member as PropertyInfo;

            // Set must be internal or public
            if (propertyInfo != null)
            {
                var readable = propertyInfo.CanRead && (propertyInfo.GetMethod.IsPublic || propertyInfo.GetMethod.IsAssembly);
                var writable = propertyInfo.CanWrite && (propertyInfo.SetMethod.IsPublic || propertyInfo.SetMethod.IsAssembly);

                prop.Writable = readable && writable;
                prop.Readable = readable && writable;
            }

            return prop;
        }
    }
}
