﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MetaBrainz.MusicBrainz;
using MetaBrainz.MusicBrainz.CoverArt;

namespace Savify.Core
{
    public class Song
    {
        public readonly string YOUTUBEDL_ARGS = @"--extract-audio --format bestaudio --audio-quality {0} --audio-format {1} --prefer-ffmpeg --ffmpeg-location ""{2}"" --continue --ignore-errors --no-overwrites ""{3}"" --default-search ""{4}""";
        public readonly string SONG_ARGS = @" --output ""{5}%(title)s.%(ext)s"" --no-playlist";
        public readonly string PLAYLIST_ARGS = @" --output ""{5}%(playlist)s\%(title)s.%(ext)s"" --yes-playlist";
        public readonly string SPOTIFY_SONG_ARGS = @" --output ""{5}{6} - {7}.%(ext)s"" --no-playlist";
        public readonly string SPOTIFY_PLAYLIST_ARGS = @" --output ""{5}{6}\{7} - {8}.%(ext)s"" --no-playlist";
        public readonly string METADATA = @" --add-metadata --metadata-from-title ""(?P<artist>.+?) - (?P<title>.+)"" --xattrs";
        public readonly string WRITE_THUMBNAIL = @" --write-thumbnail --embed-thumbnail";
        public readonly string RESTRICT_FILENAMES = @" --restrict-filenames";
        public readonly string FFMPEG_COVERART = @"-i ""{0}"" -i ""{1}"" -map 0:0 -map 1:0 -codec copy -id3v2_version 3 -metadata:s:v title=""Album cover"" -metadata:s:v comment=""Cover(front)"" ""{2}""";
        public readonly string MUSICBRAINZ_HEAD = @"Savify/0.1.2 ( https://l4rry2k.github.io/savify/ )";
        public readonly string FFMPEG_GET_METADATA = @" -i ""{0}""";
        public readonly string FFMPEG_CLEAR_METADATA = @" -i ""{0}"" -map_metadata -1 ""{1}""";
        public readonly string FFMPEG_WRITE_METADATA = @" -i ""{0}"" -metadata title=""{1}"" -metadata artist=""{2}"" -metadata album=""{3}"" -id3v2_version 3 -write_id3v1 1 ""{4}""";

        public string Search { get; set; }
        public string Title { get; set; }
        public string Artists { get; set; }
        public string Album { get; set; }
        public string TrackNumber { get; set; }
        public string Year { get; set; }
        public Uri CoverArt { get; set; }
        public Uri FileLocation { get; set; }
        public Status Status { get; set; }

        public Song(string search)
        {
            Search = search;
            this.Status = Status.Waiting;
        }

        private bool IsLink()
        {
            bool result = Uri.TryCreate(Search, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        private Uri GetLink()
        {
            bool result = Uri.TryCreate(Search, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return uriResult;
        }

        private Host GetHost()
        {
            string host = GetLink().Host;

            if (host.Contains(Enumerator.GetDescription(Host.Spotify)))
                return Host.Spotify;
            else if (host.Contains(Enumerator.GetDescription(Host.YouTube)))
                return Host.YouTube;
            else if (host.Contains(Enumerator.GetDescription(Host.Soundcloud)))
                return Host.Soundcloud;
            else
                throw new Exception("Link from that website are not supported.");
        }

        private LinkType GetLinkType()
        {
            Host host = GetHost();
            Uri link = GetLink();

            if (host == Host.Spotify)
            {
                if (link.OriginalString.Contains("/album/"))
                    return LinkType.Album;
                else if (link.OriginalString.Contains("/playlist/"))
                    return LinkType.Playlist;
                else if (link.OriginalString.Contains("/track/"))
                    return LinkType.Song;
                else
                    throw new Exception("Not a valid spotify link.");
            }
            else if (host == Host.YouTube)
            {
                if (link.OriginalString.Contains("/watch?v="))
                    return LinkType.Song;
                else if (link.OriginalString.Contains("/playlist?list="))
                    return LinkType.Playlist;
                else
                    throw new Exception("Not a valid youtube link.");
            }
            else if (host == Host.Soundcloud)
            {
                //Treat album like playlist
                if (link.OriginalString.Contains("/sets/"))
                    return LinkType.Playlist;
                else
                    return LinkType.Song;
            }
            else
            {
                throw new Exception("Not a valid link for that website.");
            }
        }

        private Format GetFormat()
        {
            return Enumerator.GetValueFromDescription<Format>(Path.GetExtension(FileLocation.OriginalString));
        }

        public void Download()
        {
            Status = Status.Downloading;
            string args;
            if (!IsLink())
            {
                args = string.Format(YOUTUBEDL_ARGS + METADATA + SONG_ARGS + (Settings.Default.RestrictFilenames ? RESTRICT_FILENAMES : ""), Settings.Default.Quality, Enumerator.GetValueFromDescription<Format>(Settings.Default.Format), Settings.Default.FFmpeg, Search, Settings.Default.Search, Path.GetTempPath());
                string output = Youtubedl.Run(args);

                Regex regex = new Regex(@"(?<=\[ffmpeg\] Destination: )(.*?)(?=\n)");
                Match match = regex.Match(output);
                if (match.Success)
                {
                    Status = Status.Downloaded;
                    FileLocation = new Uri(match.Value);
                    ReadMetadada();
                    DownloadAlbumCover();
                    WriteMetadata();
                    Console.WriteLine("\nDownloaded: " + match.Value + " [RETURN]");
                }
            }
        }

        private void DownloadAlbumCover()
        {
            Query query = new Query(MUSICBRAINZ_HEAD);
            Guid artistMbid = query.FindArtists("artist:" + Artists).Results[0].MbId;
            Guid releaseMbid = query.FindRecordings("recording:" + Title + " AND arid:" + artistMbid).Results[0].Releases[0].MbId;
            Album = query.FindReleases("reid:" + releaseMbid).Results[0].Title;

            if (query.LookupRelease(releaseMbid).CoverArtArchive.Front)
            {
                CoverArt ca = new CoverArt(MUSICBRAINZ_HEAD);
                CoverArt = new Uri(Path.GetTempPath() + releaseMbid + ".jpg");
                ca.FetchFront(releaseMbid).Decode().Save(CoverArt.OriginalString);
            }          
        }

        private string GetFilename()
        {
            return Path.GetFileNameWithoutExtension(FileLocation.OriginalString);
        }

        private string GetPath()
        {
            return Path.GetDirectoryName(FileLocation.LocalPath);
        }

        private void ReadMetadada()
        {
            if (GetFormat() == Format.mp3)
            {
                TagLib.File file = GetTagLibFile();

                Title = file.Tag.Title;
                Artists = file.Tag.FirstPerformer;
                Album = file.Tag.Album;
                TrackNumber = file.Tag.Track.ToString();
                Year = file.Tag.Year.ToString();
            }
        }

        private void WriteMetadata()
        {
            TagLib.File file = GetTagLibFile();

            ClearMetadata();
            
            file.Tag.Title = Title;
            file.Tag.Performers = Artists.Split(',');
            file.Tag.Album = Album;
            file.Tag.Track = (uint)Convert.ToInt32(TrackNumber);
            file.Tag.Year = (uint)Convert.ToInt32(Year);
            file.RemoveTags(file.TagTypes & ~file.TagTypesOnDisk);
            file.Save();
            file.Dispose();

            WriteAlbumCover();
        }

        private void ClearMetadata()
        {
            if (GetFormat() == Format.mp3)
            {
                TagLib.File file = GetTagLibFile();

                file.Tag.Clear();
                file.RemoveTags(TagLib.TagTypes.AllTags);
                file.Save();
                file.Dispose();
            }
        }

        private void WriteAlbumCover()
        {          
            Uri newLocation = new Uri(Settings.Default.OutputPath + GetFilename() + Enumerator.GetDescription(GetFormat()));
            string args = string.Format(FFMPEG_COVERART, FileLocation.LocalPath, CoverArt.LocalPath, newLocation.LocalPath);
            Ffmpeg.Run(args);
            File.Delete(CoverArt.LocalPath);
            File.Delete(FileLocation.LocalPath);
            FileLocation = newLocation;
        }

        private TagLib.File GetTagLibFile()
        {
            TagLib.Id3v2.Tag.DefaultVersion = 3;
            TagLib.Id3v2.Tag.ForceDefaultVersion = true;
            return TagLib.File.Create(FileLocation.LocalPath);
        }
    }
}
