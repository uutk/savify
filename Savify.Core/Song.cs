using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public Song(string search)
        {
            Search = search;
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
    }
}
