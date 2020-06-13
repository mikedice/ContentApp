using System;

namespace ContentApp.Models
{
    public enum AssetType
    {
        Photo
    }

    public class Asset
    {
        public int AssetID { get; set; }
        public string FilePrefix { get; set; }
        public AssetType? AssetType { get; set; }
        public DateTime CreatedOn { get; set; }
    }
}
