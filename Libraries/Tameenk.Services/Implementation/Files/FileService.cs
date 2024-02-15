using System;
using System.IO;
using System.Reflection;
using Tameenk.Services.Core.Files;

namespace Tameenk.Services.Implementation.Files
{
    public class FileService :IFileService
    {
        #region Methods
        
        /// <summary>
        /// check if file exist or not
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public bool FileExist(string fileName)
        {
            string currentPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            var path = currentPath + "bin\\" + fileName + ".dll";
            if (File.Exists(path))
            {
                return true;
            }
            return false;
        }
        /// <summary>
        /// validate if dll file is vaild dll file
        /// </summary>
        /// <param name="nameSpace"></param>
        /// <param name="nameOfClass"></param>
        /// <param name="file">binary data to file</param>
        /// <returns></returns>
        public bool ValidateDllFile(string nameSpace , string nameOfClass,byte[] file)
        {
            string currentPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            string folder = "App_Data\\uploads\\";
            var path = currentPath + folder + nameSpace + ".dll";
            
            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
                fs.Close();
            }
            AppDomain dom = AppDomain.CreateDomain("TestCompanyDll");
            AssemblyName assemblyName = new AssemblyName
            {
                CodeBase = path
            };
            Assembly testAssembly = dom.Load(assemblyName);
            Type calc = testAssembly.GetType(nameOfClass);

            if (calc != null)
            {
                AppDomain.Unload(dom);
                DeleteFile(nameSpace,folder);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// Save byte data in dll file ( Bin folder ) 
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public void SaveFileInBin(string fileName, Byte[] file)
        {
            // File.WriteAllBytes(fileName, file);
            string currentPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            var path = currentPath + "bin\\" + fileName + ".dll";

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                fs.Write(file, 0, file.Length);
            }
        }

        /// <summary>
        /// Delete dll file from bin folder
        /// </summary>
        /// <param name="fileName"></param>
        public void DeleteFile(string fileName,string folder = "bin\\")
        {
            string currentPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();
            
            var currentFolder = currentPath + folder;
            var path = currentFolder + fileName + ".dll";

            //Check if the program is in the same folder as the update
            
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        #endregion
    }
}
