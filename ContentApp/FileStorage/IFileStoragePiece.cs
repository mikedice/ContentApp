using System;
using System.Threading.Tasks;
using System.IO;

namespace ContentApp.FileStorage
{
    public interface IFileStoragePiece
    {
        public Task<string> StoreImage(Stream data);
    }
}
