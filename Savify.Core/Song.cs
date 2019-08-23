using System;

namespace Savify.Core
{
    class Song
    {
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
