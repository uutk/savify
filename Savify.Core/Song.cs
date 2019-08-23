using System;

namespace Savify.Core
{
    public class Song
    {
        public readonly string YOUTUBEDL_ARGS = @"--extract-audio --format bestaudio --audio-quality {0} --audio-format {1} --prefer-ffmpeg --ffmpeg-location ""{2}"" --continue --ignore-errors --no-overwrites ""{3}"" --default-search ""{4}""";
        public readonly string SONG_ARGS = @" --output ""{5}%(title)s.%(ext)s"" --no-playlist";
        public readonly string PLAYLIST_ARGS = @" --output ""{5}%(playlist)s\%(title)s.%(ext)s"" --yes-playlist";
        public readonly string SPOTIFY_SONG_ARGS = @" --output ""{5}{6} - {7}.%(ext)s"" --no-playlist";
        public readonly string SPOTIFY_PLAYLIST_ARGS = @" --output ""{5}{6}\{7} - {8}.%(ext)s"" --no-playlist";
        public readonly string METADATA = @" --add-metadata --metadata-from-title ""(?P<artist>.+?) - (?P<title>.+)"" --xattrs --embed-thumbnail";

        private string search;
        private string title;
        private string artists;
        private string album;
        private string trackNumber;
        private string year;
        private Uri coverArt;
        private Uri fileLocation;
        private Status status;
        private Format format;

        public Song(string search)
        {
            Search = search;
            this.Status = Status.Waiting;
            this.Format = Enumerator.GetValueFromDescription<Format>(Settings.Default.Format);
        }

        public bool IsLink()
        {
            bool result = Uri.TryCreate(Search, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return result;
        }

        public Uri GetLink()
        {
            bool result = Uri.TryCreate(Search, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);

            return uriResult;
        }

        public Host GetHost()
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

        public LinkType GetLinkType()
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

        public void Download()
        {
            string args;
            if (!IsLink())
            {
                args = string.Format(YOUTUBEDL_ARGS + METADATA + SONG_ARGS, Settings.Default.Quality, Enumerator.GetValueFromDescription<Format>(Settings.Default.Format), Settings.Default.FFmpeg, Search, Settings.Default.Search, Settings.Default.OutputPath);
                Youtubedl.Run(args);
            }
        }

        public string Search { get => search; set => search = value; }
        public string Title { get => title; set => title = value; }
        public string Artists { get => artists; set => artists = value; }
        public string Album { get => album; set => album = value; }
        public string TrackNumber { get => trackNumber; set => trackNumber = value; }
        public string Year { get => year; set => year = value; }
        public Uri CoverArt { get => coverArt; set => coverArt = value; }
        public Uri FileLocation { get => fileLocation; set => fileLocation = value; }
        public Status Status { get => status; set => status = value; }
        public Format Format { get => format; set => format = value; }
    }
}
