using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace FileSaverCSharp
{
    public static class FileUtils
    {
        public static byte[] ReadFileBytes(string filePath)
        {
            byte[] buffer;
            using (Stream stream = File.OpenRead(filePath))
            {
                buffer= new byte[stream.Length];
                stream.Read(buffer, 0, buffer.Length);                
            }

            return buffer;
        }

        public static void SaveFile(string filePath, string fileName, string extension, byte[] buffer)
        {
            var path = System.IO.Path.Combine(filePath, fileName + extension);
            SaveFile(path, buffer);
        }

        public static void SaveFile(string filePath, byte[] buffer)
        {
            File.WriteAllBytes(filePath, buffer);
        }

        public static string GetFileName(string filePath)
        {
            return new FileInfo(filePath).Name;
        }

        public static string GetExtension(string filePath)
        {
            return new FileInfo(filePath).Extension;
        }
    }
}
