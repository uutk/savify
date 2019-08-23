using System;
using System.ComponentModel;
using System.Reflection;

namespace Savify.Core
{
    public class Enumerator
    {
        public static string GetDescription(Enum value)
        {
            FieldInfo info = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])info.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes != null && attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }                
        }

        public static T GetValueFromDescription<T>(string description)
        {
            var type = typeof(T);
            if (!type.IsEnum) throw new InvalidOperationException();
            foreach (var field in type.GetFields())
            {
                var attribute = Attribute.GetCustomAttribute(field,
                    typeof(DescriptionAttribute)) as DescriptionAttribute;
                if (attribute != null)
                {
                    if (attribute.Description == description)
                        return (T)field.GetValue(null);
                }
                else
                {
                    if (field.Name == description)
                        return (T)field.GetValue(null);
                }
            }
            return default(T);
        }
        
    }

    public enum Host
    {
        [Description("youtube.com")]
        YouTube,
        [Description("soundcloud.com")]
        Soundcloud,
        [Description("spotify.com")]
        Spotify
    }

    public enum Status
    {
        [Description("In Queue")]
        Waiting,
        [Description("Downloading")]
        Downloading,
        [Description("Downloaded")]
        Downloaded,
        [Description("Failed")]
        Failed
    }

    public enum Quality
    {
        [Description("32K")]
        _32kbps = 32,
        [Description("96K")]
        _96kbps = 96,
        [Description("128K")]
        _128kbps = 128,
        [Description("192K")]
        _192kbps = 192,
        [Description("256K")]
        _256kbps = 256,
        [Description("320K")]
        _320kbps = 320,
        [Description("9")]
        lowest = 9,
        [Description("0")]
        highest = 0
    }


    public enum Format
    {
        [Description(".aac")]
        aac,
        [Description(".flac")]
        flac,
        [Description(".mp3")]
        mp3,
        [Description(".m4a")]
        m4a,
        [Description(".opus")]
        opus,
        [Description(".vorbis")]
        vorbis,
        [Description(".wav")]
        wav//,
        //best
    }

    public enum LinkType
    {
        [Description("Song")]
        Song,
        [Description("Playlist")]
        Playlist,
        [Description("Album")]
        Album
    }
}
