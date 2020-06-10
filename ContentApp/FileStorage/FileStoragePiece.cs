using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using ContentApp.KeyVault;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;

namespace ContentApp.FileStorage
{
    public class FileStoragePiece : IFileStoragePiece
    {
        private static string connection = "TODO";
        private const string ImageKeyTiny = "tiny";
        private const string ImageKeyMedium = "medium";
        private const string ImageKeyOriginal = "normal";
        private const int OrientationKey = 0x0112;
        private const int NotSpecified = 0;
        private const int NormalOrientation = 1;
        private const int MirrorHorizontal = 2;
        private const int UpsideDown = 3;
        private const int MirrorVertical = 4;
        private const int MirrorHorizontalAndRotateRight = 5;
        private const int RotateLeft = 6;
        private const int MirorHorizontalAndRotateLeft = 7;
        private const int RotateRight = 8;
        private readonly IKeyVaultPiece keyVaultPiece;

        public FileStoragePiece(IKeyVaultPiece keyVaultPiece)
        {
            this.keyVaultPiece = keyVaultPiece;
        }

        public string StoreImage(Stream data)
        {
            var imageDictionary = PrepareImages(data);
            var fileBase = Guid.NewGuid().ToString();
            if (CloudStorageAccount.TryParse(connection, out var storageAccount))
            {
                CloudBlobClient client = storageAccount.CreateCloudBlobClient();

                var container = client.GetContainerReference("images");

                foreach (var kvp in imageDictionary)
                {
                    var fileName = $"{fileBase}.{kvp.Key}.jpg";
                    var blockBlob = container.GetBlockBlobReference(fileName);
                    blockBlob.Properties.ContentType = "image/jpeg";
                    using (var memStream = new MemoryStream())
                    {
                        kvp.Value.Save(memStream, ImageFormat.Jpeg);
                        memStream.Flush();
                        memStream.Seek(0, SeekOrigin.Begin);
                        blockBlob.UploadFromStream(memStream);
                    }
                }
                return fileBase;
            }
            return null;
        }

        private static Dictionary<string, Image> PrepareImages(Stream data)
        {
            Dictionary<string, Image> result = new Dictionary<string, Image>();

            var original = Image.FromStream(data, false, false);
            result.Add(ImageKeyTiny, ScaleImage(original, 0.1));
            result.Add(ImageKeyMedium, ScaleImage(original, 0.3));
            result.Add(ImageKeyOriginal, original);
            return result;
        }

        private static Image ScaleImage(Image original, double scale)
        {
            var newWidth = Math.Floor(original.Width * scale);
            var newHeight = Math.Floor(original.Height * scale);
            Bitmap newImage = new Bitmap((int)newWidth, (int)newHeight, PixelFormat.Format32bppArgb);
            using (var graphics = Graphics.FromImage(newImage)) 
            {
                //graphics.Clear(Color.Transparent);
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;

                graphics.DrawImage(original,
                    new Rectangle(0, 0, (int)newWidth, (int)newHeight),
                    new Rectangle(0, 0, original.Width, original.Height),
                    GraphicsUnit.Pixel);

                if (original.PropertyIdList.Contains(OrientationKey))
                {
                    var orientation = (int)original.GetPropertyItem(OrientationKey).Value[0];
                    switch (orientation)
                    {
                        case NotSpecified: // Assume it is good.
                        case NormalOrientation:
                            // No rotation required.
                            break;
                        case MirrorHorizontal:
                            newImage.RotateFlip(RotateFlipType.RotateNoneFlipX);
                            break;
                        case UpsideDown:
                            newImage.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            break;
                        case MirrorVertical:
                            newImage.RotateFlip(RotateFlipType.Rotate180FlipX);
                            break;
                        case MirrorHorizontalAndRotateRight:
                            newImage.RotateFlip(RotateFlipType.Rotate90FlipX);
                            break;
                        case RotateLeft:
                            newImage.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            break;
                        case MirorHorizontalAndRotateLeft:
                            newImage.RotateFlip(RotateFlipType.Rotate270FlipX);
                            break;
                        case RotateRight:
                            newImage.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            break;
                        //default:
                            //throw new NotImplementedException("An orientation of " + orientation + " isn't implemented.");
                    }
                }
            }
            return newImage;        
        }
    }
}
