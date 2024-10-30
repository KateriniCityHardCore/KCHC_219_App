using System;
using System.Collections.Generic;
using KCHC.Enum;

namespace KCHC.Models
{
    public class Artist : Person
    {
        public string Description { get; set; } = string.Empty;
        public Genre Genre { get; set; } = Genre.Katerinian;
        public string ContentImage { get; set; } = string.Empty;
        public string SpotifyAccountUrl { get; set; } = string.Empty;
        public string YoutubeAccountUrl { get; set; } = string.Empty;
        public string BandCampAccountUrl { get; set; } = string.Empty;
        public string SoundcloudAccountUrl { get; set; } = string.Empty;
        public string TrovoAccountUrl { get; set; } = string.Empty;
        public string TwitchAccountUrl { get; set; } = string.Empty;
        public string ExtraDescription { get; set; } = string.Empty;
        public string SongkickUrl { get; set; } = string.Empty;
        public bool IsABand { get; set; } = false;
        public DateTime CreatedOn { get; set; } = DateTime.MinValue;
        public List<MemberRole> Members { get; set; } = new List<MemberRole>();


        /// <summary>
        /// Not full Constructor. This constructor is only for first page
        /// </summary>
        /// <param name="name"> The Name of the Artist</param>
        /// <param name="photoPath">The photopath of the artist inside this solution</param>
        /// <param name="description">General description about the artist</param>
        public Artist(string name, string photoPath, string description)
        {
            Name = name;
            PhotoPath = photoPath;
            Description = description;
        }

        /// <summary> 
        ///  Constructor with optional parameters. Only the name is necessary.
        /// </summary> 
        public Artist(string name, string photoPath = "", string description = "", Genre genre = Genre.Katerinian, string contentImage = "", string spotifyAccountUrl = "", string youtubeAccountUrl, string bandCampAccountUrl = "",
            string soundcloudAccountUrl = "", string trovoAccountUrl = "", string twitchAccountUrl = "", string extraDescription = "", string songkickUrl = "", bool isABand = true, DateTime dateTime = default, List<MemberRole> members = null)
        {
            Name = name;
            PhotoPath = photoPath;
            Description = description;
            Genre = genre;
            ContentImage = contentImage;
            SpotifyAccountUrl = spotifyAccountUrl;
            YoutubeAccountUrl = youtubeAccountUrl;
            BandCampAccountUrl = bandCampAccountUrl;
            SoundcloudAccountUrl = soundcloudAccountUrl;
            TrovoAccountUrl = trovoAccountUrl;
            TwitchAccountUrl = twitchAccountUrl;
            ExtraDescription = extraDescription;
            SongkickUrl = songkickUrl;
            IsABand = isABand;
            CreatedOn = dateTime;
            Members = members;
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public Artist()
        {
        }
    }
}
