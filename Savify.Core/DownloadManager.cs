using System;
using System.Collections.Generic;
using System.IO;

namespace Savify.Core
{
    public static class DownloadManager
    {
        private readonly static string YOUTUBEDL_ARGS = @"--extract-audio --format bestaudio --audio-quality {0} --audio-format {1} --prefer-ffmpeg --ffmpeg-location ""{2}"" --continue --ignore-errors --no-overwrites ""{3}"" --default-search ""{4}""";
        private readonly static string SONG_ARGS = @" --output ""{5}%(title)s.%(ext)s"" --no-playlist";
        private readonly static string PLAYLIST_ARGS = @"""{0}"" --get-id --ignore-errors";
        private readonly static string SPOTIFY_SONG_ARGS = @" --output ""{5}{6} - {7}.%(ext)s"" --no-playlist";
        private readonly static string SPOTIFY_PLAYLIST_ARGS = @" --output ""{5}{6}\{7} - {8}.%(ext)s"" --no-playlist";
        private readonly static string METADATA = @" --add-metadata --metadata-from-title ""(?P<artist>.+?) - (?P<title>.+)"" --xattrs";
        private readonly static string RESTRICT_FILENAMES = @" --restrict-filenames";

        public static void DownloadQuery(string query)
        {
            Uri link = GetLink(query);

            if (link == null)
            {
                //No link search
                DownloadSong(new Song(query));
            }
            else
            {
                Host host = GetHost(link);
                LinkType linkType = GetLinkType(link, host);
                //YouTube link
                if (host == Host.YouTube)
                {
                    //YouTube playlist
                    if (linkType == LinkType.Playlist)
                    {
                        string args = string.Format(PLAYLIST_ARGS, link.OriginalString);
                        string output = Youtubedl.Run(args);

                        List<Song> songs = new List<Song>();
                        string[] stringSeparators = new string[] { "\n" };
                        foreach (string id in output.Split(stringSeparators, StringSplitOptions.None))
                        {
                            if (id.Length == 11)
                            {
                                songs.Add(new Song(id));
                            }
                        }

                        DownloadList(songs);
                    }
                    //YouTube video
                    else
                    {
                        DownloadSong(new Song(link.OriginalString));
                    }
                }
            }
        }

        private static void DownloadSong(Song song)
        {
            string args = string.Format(YOUTUBEDL_ARGS + METADATA + SONG_ARGS + (Settings.Default.RestrictFilenames ? RESTRICT_FILENAMES : ""), Settings.Default.Quality, Enumerator.GetValueFromDescription<Format>(Settings.Default.Format), Settings.Default.FFmpeg, song.Search, Settings.Default.Search, Path.GetTempPath());            
            song.Download(args);
        }

        private static void DownloadList(List<Song> songs)
        {
            foreach (Song song in songs){
                DownloadSong(song);
            }
        }

        private static LinkType GetLinkType(Uri link, Host host)
        {


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

        private static Uri GetLink(string query)
        {
            _ = Uri.TryCreate(query, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return uriResult;
        }

        private static Host GetHost(Uri link)
        {
            string host = link.Host;

            if (host.Contains(Enumerator.GetDescription(Host.Spotify)))
                return Host.Spotify;
            else if (host.Contains(Enumerator.GetDescription(Host.YouTube)))
                return Host.YouTube;
            else if (host.Contains(Enumerator.GetDescription(Host.Soundcloud)))
                return Host.Soundcloud;
            else
                throw new Exception("Link from that website are not supported.");
        }


        public static void Log(string output)
        {
            string path = Path.Combine(Environment.CurrentDirectory, "Logs");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            else
            {
                System.IO.File.WriteAllText(path + @"\Log.txt", output);
            }
        }
    }
}
