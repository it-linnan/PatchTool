using System.Windows.Forms;
using static System.Environment;

namespace PatchTool.Common
{
    internal class FileDiaologUtil
    {
        /// <summary>
        /// 创建文件对话框
        /// </summary>
        /// <param name="initDir">默认打开路径</param>
        /// <param name="filter">文件类型</param>
        /// <returns></returns>
        public static OpenFileDialog openFileDialog(string initDir, string filter)
        {
            OpenFileDialog op = new OpenFileDialog();
            op.InitialDirectory = initDir;
            op.RestoreDirectory = true;
            op.Filter = filter;
            return op;
        }

        /// <summary>
        /// 创建选择文件夹对话框
        /// </summary>
        /// <param name="rootFolder">默认打开路径</param>
        /// <returns></returns>
        public static FolderBrowserDialog openFolderDialog(SpecialFolder specialFolder = SpecialFolder.Desktop)
        {
            FolderBrowserDialog op = new FolderBrowserDialog();
            op.ShowNewFolderButton = true;
            op.RootFolder = specialFolder;
            return op;
        }
    }
}