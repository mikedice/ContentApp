using System;

namespace ContentApp.Models
{
    public enum AssetType
    {
        Photo
    }

    public class Asset
    {
        /// <summary>
        /// Unique ID of this record (primary key)
        /// </summary>
        public int AssetID { get; set; }

        /// <summary>
        /// The filename prefix of the file as it is stored in the blob store
        /// </summary>
        public string FilePrefix { get; set; }

        // The type of file uploaded
        public AssetType? AssetType { get; set; }

        /// <summary>
        /// The UTC Date/Time the file was uploaded
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// The name of the file used when the file was uploaded
        /// </summary>
        public string UploadedFileName { get; set; }
    }
}
