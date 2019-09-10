using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.CoverArt;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Savify.Core
{
    public class Song
    {
        private readonly string FFMPEG_COVERART = @"-i ""{0}"" -i ""{1}"" -map 0:0 -map 1:0 -codec copy -id3v2_version 3 -metadata:s:v title=""Album cover"" -metadata:s:v comment=""Cover(front)"" ""{2}""";
        private readonly string MUSICBRAINZ_HEAD = @"Savify/0.1.2 ( https://l4rry2k.github.io/savify/ )";


        //private readonly string PLAYLIST_ARGS = @" --output ""{5}%(playlist)s\%(title)s.%(ext)s"" --yes-playlist";
        //private readonly string WRITE_THUMBNAIL = @" --write-thumbnail --embed-thumbnail";
        //private readonly string FFMPEG_GET_METADATA = @" -i ""{0}""";
        //private readonly string FFMPEG_CLEAR_METADATA = @" -i ""{0}"" -map_metadata -1 ""{1}""";
        //private readonly string FFMPEG_WRITE_METADATA = @" -i ""{0}"" -metadata title=""{1}"" -metadata artist=""{2}"" -metadata album=""{3}"" -id3v2_version 3 -write_id3v1 1 ""{4}""";

        public string Search { get; set; }
        public string Title { get; set; }
        public string Artists { get; set; }
        public string Album { get; set; }
        public string TrackNumber { get; set; }
        public string Year { get; set; }
        public Uri CoverArt { get; set; }
        public Uri FileLocation { get; set; }
        public Status Status { get; set; }

        private bool foundCoverart = false;

        public Song(string search)
        {
            Search = search;
            this.Status = Status.Waiting;
        }

        private Format GetFormat()
        {
            return Enumerator.GetValueFromDescription<Format>(Path.GetExtension(FileLocation.LocalPath));
        }

        public void Download(string args)
        {
            Status = Status.Downloading;
            string output = Youtubedl.Run(args);
            DownloadManager.Log(output);

            Match match = new Regex(@"(?<=\[ffmpeg\] Destination: )(.*?)(?=\n)").Match(output);

            if (match.Success)
            {
                Status = Status.Downloaded;
                FileLocation = new Uri(match.Value);

                if (GetFormat() == Format.mp3)
                {
                    ReadMetadada();
                    DownloadCoverart();
                    WriteMetadata();
                }

                Rename();

                Console.WriteLine("\nDownloaded: " + FileLocation.LocalPath + " [RETURN]");
            }

            
        }

        private void DownloadCoverart()
        {
            try
            {
                Query query = new Query(MUSICBRAINZ_HEAD);
                Guid artistMbid = query.FindArtists("artist:" + Artists).Results[0].MbId;
                Guid releaseMbid = query.FindReleases("recording:" + Title + " AND arid:" + artistMbid).Results[0].MbId;
                Album = query.LookupRelease(releaseMbid).Title;
                Year = query.LookupRelease(releaseMbid).Date.Year.ToString();

                if (query.LookupRelease(releaseMbid).CoverArtArchive.Front)
                {
                    CoverArt ca = new CoverArt(MUSICBRAINZ_HEAD);
                    CoverArt = new Uri(Path.GetTempPath() + releaseMbid + ".jpg");
                    ca.FetchFront(releaseMbid).Decode().Save(CoverArt.LocalPath);
                    foundCoverart = true;
                }
                else
                {
                    CoverArt = new Uri(Environment.CurrentDirectory + @"\assets\savify-cover-default.jpg");
                }
            }
            catch
            {
                CoverArt = new Uri(Environment.CurrentDirectory + @"\assets\savify-cover-default.jpg");
            }                    
        }

        private string GetFilename()
        {
            return Path.GetFileNameWithoutExtension(FileLocation.LocalPath);
        }

        private string GetPath()
        {
            return Path.GetDirectoryName(FileLocation.LocalPath) + @"\";
        }

        private string GetExtension()
        {
            return Path.GetExtension(FileLocation.LocalPath);
        }

        private void ReadMetadada()
        {
            TagLib.File file = GetTagLibFile();

            Title = file.Tag.Title;
            Artists = file.Tag.FirstPerformer;
            Album = file.Tag.Album;
            TrackNumber = file.Tag.Track.ToString();
            Year = file.Tag.Year.ToString();
        }

        private void WriteMetadata()
        {
            ClearMetadata();
            WriteAlbumCover();

            TagLib.File file = GetTagLibFile();
            file.Tag.Title = Title;
            file.Tag.Performers = Artists.Split(',');
            file.Tag.Album = Album;
            file.Tag.Track = (uint)Convert.ToInt32(TrackNumber);
            file.Tag.Year = (uint)Convert.ToInt32(Year);
            file.Save();
            file.Dispose();        
        }

        private void ClearMetadata()
        {
            TagLib.File file = GetTagLibFile();
            file.Tag.Clear();
            file.RemoveTags(file.TagTypes & ~file.TagTypesOnDisk);
            file.RemoveTags(TagLib.TagTypes.AllTags);
            file.Save();
            file.Dispose();
        }

        private void WriteAlbumCover()
        {          
            Uri newLocation = new Uri(Settings.Default.OutputPath + GetFilename() + Enumerator.GetDescription(GetFormat()));
            string args = string.Format(FFMPEG_COVERART, FileLocation.LocalPath, CoverArt.LocalPath, newLocation.LocalPath);
            Ffmpeg.Run(args);
            if (foundCoverart)
            {
                File.Delete(CoverArt.LocalPath);
            }
            File.Delete(FileLocation.LocalPath);
            FileLocation = newLocation;
        }

        private TagLib.File GetTagLibFile()
        {
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            return TagLib.File.Create(FileLocation.LocalPath);
        }
        

        private void Rename()
        {
            if (Settings.Default.RestrictFilenames)
            {
                Uri newFileLocation = new Uri(GetPath() + Artists.ToFilePathSafeString() + " - " + Title.ToFilePathSafeString() + GetExtension());
                File.Move(FileLocation.LocalPath, newFileLocation.LocalPath);
                FileLocation = newFileLocation;

            }
            else
            {
                Uri newFileLocation = new Uri(GetPath() + Artists + " - " + Title + GetExtension());
                File.Move(FileLocation.LocalPath, newFileLocation.LocalPath);
                FileLocation = newFileLocation;
            }
        }
    }

    public static class SongExtensions
    {
        public static string ToFilePathSafeString(this string path, char replaceChar = '_')
        {
            return Path.GetInvalidFileNameChars().Aggregate(path, (current, invalidFileNameChar) => current.Replace(invalidFileNameChar, replaceChar)).TrimEnd(' ', '.');
        }
    }
}
