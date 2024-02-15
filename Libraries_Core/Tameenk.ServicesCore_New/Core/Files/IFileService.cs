using System;

namespace Tameenk.Services.Core.Files
{
    public interface IFileService
    {
        /// <summary>
        /// validate if dll file is vaild dll file
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="nameOfClass"></param>
        /// <param name="file">binary data to file</param>
        /// <returns></returns>
       bool ValidateDllFile(string nameSpace, string nameOfClass,byte[] file);

        /// <summary>
        /// check if file exist or not
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        bool FileExist(string fileName);

        /// <summary>
        /// Save byte data in dll file ( Bin folder ) 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        void SaveFileInBin(string fileName, Byte[] file);

        /// <summary>
        /// Delete dll file from bin folder
        /// </summary>
        /// <param name="fileName"></param>
        void DeleteFile(string fileName, string folder = "bin\\");
    }
}
