using System;
using System.IO;

namespace ContentApp.FileStorage
{
    public interface IFileStoragePiece
    {
        public string StoreImage(Stream data);
    }
}
