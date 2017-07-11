using System.IO;
using System.Text;

namespace PatchTool.Common
{
    internal class FileUtil
    {
        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="fileContent">文件内容</param>
        public static void createFileFromString(string filePath, string fileContent)
        {
            byte[] datas = Encoding.UTF8.GetBytes(fileContent);
            createFileFromBytes(filePath, datas);
        }

        /// <summary>
        /// 创建文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="datas">比特数组</param>
        public static void createFileFromBytes(string filePath, byte[] datas)
        {
            int lastChar = filePath.LastIndexOf(@"\");
            string folderPath = filePath.Substring(0, lastChar);
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            FileStream fs = File.Create(filePath);
            fs.Write(datas, 0, datas.Length);
            fs.Flush();
            fs.Close();
        }

        /// <summary>
        /// 删除文件夹
        /// </summary>
        /// <param name="folderPath">文件夹路径</param>
        public static void deleteFolder(string folderPath)
        {
            if (Directory.Exists(folderPath))
            {
                foreach (string dir in Directory.GetFileSystemEntries(folderPath))
                {
                    if (File.Exists(dir))
                    {
                        File.Delete(dir);
                    }
                    else
                    {
                        deleteFolder(dir);
                    }
                }
                Directory.Delete(folderPath, true);
            }
        }
    }
}