using PatchTool.Common;
using SharpSvn;
using SharpSvn.Implementation;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Forms;
using System.Xml.Linq;

namespace PatchTool
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        #region 初始化

        #region 变量
        /// <summary>
        /// 文件路径缓存
        /// </summary>
        private Hashtable pathCache;

        /// <summary>
        /// tomcat路径
        /// </summary>
        private string tomcatPath = string.Empty;

        /// <summary>
        /// 补丁路径
        /// </summary>
        private string patchPath = string.Empty;

        /// <summary>
        /// svn开始版本号
        /// </summary>
        private long revisionStart = 1L;

        /// <summary>
        /// svn结束版本号
        /// </summary>
        private long revisionEnd = 1L;
        #endregion 变量

        #region 窗体初始化

        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 窗体初始化
        /// </summary>
        /// <param name="sender">事件数据</param>
        /// <param name="e">事件监视对象</param>
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 从配置文件中读取配置
            XDocument xDoc = XmlUtil.getXDoc("init.xml");
            XElement xEleTomcat = XmlUtil.getXEle(xDoc, "tomcat");
            XElement xEleSvn = XmlUtil.getXEle(xDoc, "svn");
            XElement xElePatch = XmlUtil.getXEle(xDoc, "patch");
            // 回显
            tomcatPathTxt.Text = XmlUtil.getXEleAttrVal(xEleTomcat, "path");
            svnUrlTxt.Text = XmlUtil.getXEleAttrVal(xEleSvn, "url");
            revisionStartTxt.Text = XmlUtil.getXEleAttrVal(xEleSvn, "revisionStart");
            revisionEndTxt.Text = XmlUtil.getXEleAttrVal(xEleSvn, "revisionEnd");
            patchPathTxt.Text = XmlUtil.getXEleAttrVal(xElePatch, "path");
        }

        #endregion 窗体初始化

        #endregion 初始化

        #region 事件

        #region 打开文件夹对话框
        /// <summary>
        /// 打开文件夹对话框
        /// </summary>
        /// <param name="sender">事件数据</param>
        /// <param name="e">事件监视对象</param>
        private void openFolder_Click(object sender, RoutedEventArgs e)
        {
            // 获取触发事件的文本框
            System.Windows.Controls.TextBox txt = (System.Windows.Controls.TextBox)e.Source;
            // 打开文件夹对话框 根节点是我的电脑
            FolderBrowserDialog dialog = FileDiaologUtil.openFolderDialog(Environment.SpecialFolder.MyComputer);

            // 初始化选中文件夹
            if (!string.IsNullOrWhiteSpace(txt.Text))
            {
                dialog.SelectedPath = txt.Text;
            }
            dialog.ShowDialog();

            // 用户选择完成回显
            if (!string.IsNullOrWhiteSpace(dialog.SelectedPath))
            {
                txt.Text = dialog.SelectedPath;
            }
        }
        #endregion 打开文件夹对话框

        #region 生成补丁
        /// <summary>
        /// 生成补丁
        /// </summary>
        /// <param name="sender">事件数据</param>
        /// <param name="e">事件监视对象</param>
        private void createPatchBtn_Click(object sender, RoutedEventArgs e)
        {
            #region 初始化变量
            StringBuilder sbLogs = new StringBuilder();
            Collection<SvnLogEventArgs> logItems = new Collection<SvnLogEventArgs>();
            pathCache = new Hashtable();

            string svnUrl = svnUrlTxt.Text;
            tomcatPath = tomcatPathTxt.Text;
            patchPath = patchPathTxt.Text;
            revisionStart = Convert.ToInt64(revisionStartTxt.Text.ToString());
            revisionEnd = Convert.ToInt64(revisionEndTxt.Text.ToString());

            if (tomcatPath.LastIndexOf(@"\") != tomcatPath.Length - 1)
            {
                tomcatPath = tomcatPathTxt.Text + @"\";
            }
            if (patchPath.LastIndexOf(@"\") != patchPath.Length - 1)
            {
                patchPath = patchPathTxt.Text + @"\";
            }
            #endregion 初始化变量

            #region 生成补丁处理
            // 删除补丁包
            FileUtil.deleteFolder(patchPath);
            // 获取用户指定版本号范围内的修改记录
            using (SvnClient client = new SvnClient())
            {
                logItems = SvnUtil.getLogs(client, svnUrl, revisionStart, revisionEnd);
            }
            try
            {
                foreach (var logItem in logItems)
                {
                    // 创建补丁文件及日志字符串缓冲
                    createPatchAndLog(logItem, sbLogs);
                }

                // 输出log到指定文件
                FileUtil.createFileFromString(patchPath + @"\logs.txt", sbLogs.ToString());

                // 确认提示
                if (MessageBoxResult.Yes.Equals(System.Windows.MessageBox.Show("是否打开补丁包？", "确认", MessageBoxButton.YesNo)))
                {
                    // 调用计算机的浏览器 打开补丁包
                    System.Diagnostics.Process.Start("explorer.exe", System.IO.Path.GetDirectoryName(patchPath));
                }
            }
            catch (Exception ex)
            {
                // 异常提示
                System.Windows.MessageBox.Show(ex.Message, "警告", MessageBoxButton.OK);
            }
            #endregion 生成补丁处理
        }
        #endregion 生成补丁

        #region 保存配置

        /// <summary>
        /// 保存配置
        /// </summary>
        /// <param name="sender">事件数据</param>
        /// <param name="e">事件监视对象</param>
        private void saveConfigBtn_Click(object sender, RoutedEventArgs e)
        {
            // 配置信息保存到配置文件中
            string initConfigFilePath = @"init.xml";
            if (!File.Exists(initConfigFilePath))
            {
                XmlUtil.createXml(initConfigFilePath, "config");
            }
            XmlUtil.saveEleAttr(initConfigFilePath, "path", tomcatPathTxt.Text, "tomcat");
            XmlUtil.saveEleAttr(initConfigFilePath, "path", patchPathTxt.Text, "patch");
            XmlUtil.saveEleAttr(initConfigFilePath, "url", svnUrlTxt.Text, "svn");
            XmlUtil.saveEleAttr(initConfigFilePath, "revisionStart", revisionStartTxt.Text, "svn");
            XmlUtil.saveEleAttr(initConfigFilePath, "revisionEnd", revisionEndTxt.Text, "svn");

            System.Windows.MessageBox.Show("配置信息保存成功！", "警告", MessageBoxButton.OK);
        }

        #endregion 保存配置

        #endregion 事件

        #region 方法

        #region 创建补丁
        /// <summary>
        /// 创建补丁
        /// </summary>
        /// <param name="logItem">修改项</param>
        /// <param name="sbLogs">日志字符串缓存</param>
        private void createPatchAndLog(SvnLogEventArgs logItem, StringBuilder sbLogs)
        {
            // 取得一次svn提交 修改的文件
            SvnChangeItemCollection changeItems = logItem.ChangedPaths;
            Hashtable changeItemCopyFlgs = new Hashtable();
            bool copyFlg = false;
            foreach (SvnChangeItem changeItem in changeItems)
            {
                switch (changeItem.Action)
                {
                    case SvnChangeAction.Add:
                    case SvnChangeAction.Modify:
                    case SvnChangeAction.Replace:
                        // 将文件从tomcat下 复制到补丁包中 获取复制成功标记
                        copyFlg = dispathcerHandleFile(changeItem.Path);
                        changeItemCopyFlgs.Add(changeItem.Path, copyFlg);
                        break;

                    case SvnChangeAction.Delete:
                    case SvnChangeAction.None:
                    default:
                        break;
                }
            }
            // 拼接日志
            createLogs(sbLogs, logItem, changeItemCopyFlgs);
        }
        #endregion 创建补丁

        #region 分发处理文件
        /// <summary>
        /// 分发处理文件
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>抽取成功标记</returns>
        private bool dispathcerHandleFile(string path)
        {
            bool copyFlg = false;
            if (!pathCache.Contains(path))
            {
                if (path.Contains("/WebRoot/"))
                {
                    String purePath = path.Substring(path.IndexOf("/WebRoot/") + 9);
                    String srcPath = tomcatPath + purePath.Replace("/", "\\");
                    String targetPath = patchPath + purePath.Replace("/", "\\");
                    copyFlg = copyFile(srcPath, targetPath);
                }
                else if (path.ToLower().Contains(".sql"))
                {
                    String purePath = path.Substring(path.LastIndexOf(@"/") + 1);
                    String targetPath = patchPath + @"\sql\" + purePath;
                    copyFlg = copySql(path, targetPath);
                }
                else if (path.IndexOf("/src/") > -1)
                {
                    if (!path.Contains("."))
                    {
                        copyFlg = true;
                    }
                    else if (path.ToLower().EndsWith("java"))
                    {
                        String purePath = path.Substring(path.IndexOf("/com/"));
                        String className = purePath.Substring(purePath.LastIndexOf("/") + 1).Replace(".java", "");
                        String classPath = purePath.Substring(0, purePath.LastIndexOf("/") + 1).Replace("/", "\\");
                        String srcPath = tomcatPath + "WEB-INF\\classes" + classPath;
                        String targetPath = patchPath + "WEB-INF\\classes" + classPath;
                        copyFlg = copyJava(srcPath, targetPath, className);
                    }
                    else if (path.ToLower().Contains("map") && path.ToLower().EndsWith("xml"))
                    {
                        String purePath = path.Substring(path.IndexOf("/com/"));
                        String classPath = purePath.Replace("/", "\\");
                        String srcPath = tomcatPath + "WEB-INF\\classes" + classPath;
                        String targetPath = patchPath + "WEB-INF\\classes" + classPath;
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else if (path.ToLower().EndsWith("jsp"))
                    {
                        String purePath = path.Substring(path.IndexOf("/web/"));
                        String jspPath = purePath.Substring(purePath.LastIndexOf("/WEB-INF") + 1, purePath.Length)
                                .Replace("/", "\\");
                        String srcPath = tomcatPath + "WEB-INF\\" + jspPath;
                        String targetPath = patchPath + "WEB-INF\\" + jspPath;
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else if (path.ToLower().EndsWith("js"))
                    {
                        String purePath = path.Substring(path.IndexOf("/js/"));
                        String jsPath = purePath.Replace("/", "\\");
                        String srcPath = tomcatPath + jsPath;
                        String targetPath = patchPath + jsPath;
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else if (path.ToLower().EndsWith("css"))
                    {
                        String purePath = path.Substring(path.IndexOf("/css/"));
                        String jsPath = purePath.Replace("/", "\\");
                        String srcPath = tomcatPath + jsPath;
                        String targetPath = patchPath + jsPath;
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else if (path.ToLower().Contains("/src/main/resources/"))
                    {
                        String purePath = path.Substring(path.IndexOf("/resources/") + 11);
                        String srcPath = tomcatPath + "WEB-INF\\classes\\" + purePath.Replace("/", "\\");
                        String targetPath = patchPath + "WEB-INF\\classes\\" + purePath.Replace("/", "\\");
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else if (path.ToLower().Contains("/src/main/webapp/images/"))
                    {
                        String purePath = path.Substring(path.IndexOf("/images/"));
                        String srcPath = tomcatPath + purePath.Replace("/", "\\");
                        String targetPath = patchPath + purePath.Replace("/", "\\");
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else if (path.ToLower().Contains("/src/main/webapp/"))
                    {
                        String purePath = path.Substring(path.IndexOf("/webapp/") + 8);
                        String srcPath = tomcatPath + purePath.Replace("/", "\\");
                        String targetPath = patchPath + purePath.Replace("/", "\\");
                        copyFlg = copyFile(srcPath, targetPath);
                    }
                    else
                    {
                        String purePath = path.Substring(path.IndexOf("/src/") + 5);
                        String srcPath = tomcatPath + "WEB-INF\\classes\\" + purePath.Replace("/", "\\");
                        String desPath = patchPath + "WEB-INF\\classes\\" + purePath.Replace("/", "\\");
                        copyFlg = copyFile(srcPath, desPath);
                    }
                }
                pathCache.Add(path, "");
            }
            else
            {
                //处理过了 跳过
                copyFlg = true;
            }
            return copyFlg;
        }
        #endregion 分发处理文件

        #region 复制sql文件
        /// <summary>
        /// 复制sql文件
        /// </summary>
        /// <param name="path"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        private bool copySql(string src, string target)
        {
            try
            {
                // 拼接sql文件在svn上的完整路径
                int secondChar = src.Substring(1).IndexOf(@"/");
                string temp = src.Substring(1, secondChar);
                int index = svnUrlTxt.Text.IndexOf(temp);
                src = svnUrlTxt.Text.Substring(0, index - 1) + src;

                // 获取diff结果 写入文件
                FileUtil.createFileFromString(target, string.Empty);
                FileStream diffResults = File.Create(target);
                using (SvnClient client = new SvnClient())
                {
                    SvnUtil.getDiffResults(client, src, revisionStart, revisionEnd, diffResults);
                }
                diffResults.Flush();
                diffResults.Close();

                // 读取diff结果
                FileStream fs = new FileStream(target, FileMode.Open, FileAccess.Read);
                StreamReader read = new StreamReader(fs, Encoding.UTF8);
                string strReadline = string.Empty;
                string modifyFlg = string.Empty;
                StringBuilder sbModifyContent = new StringBuilder();
                int i = 0;

                while ((strReadline = read.ReadLine()) != null)
                {
                    // 前4行diff基本信息 不进行处理 从第5行开始处理
                    if (i >= 4)
                    {
                        // 取出标记为add的行 写入字符串缓冲
                        modifyFlg = strReadline.Substring(0, 1);
                        if (Constants.DIFF_ADD.Equals(modifyFlg))
                        {
                            sbModifyContent.AppendLine(strReadline.Substring(1));
                        }
                    }
                    i++;
                }
                read.Close();
                // 创建更新脚本
                FileUtil.createFileFromString(target, sbModifyContent.ToString());
            }
            catch (Exception e)
            {
                throw e;
            }
            return true;
        }
        #endregion 复制sql文件

        #region 复制class文件
        /// <summary>
        /// 复制class文件
        /// </summary>
        /// <param name="src">源文件</param>
        /// <param name="target">目标文件</param>
        /// <param name="className">类名</param>
        /// <returns>复制成功标记</returns>
        private bool copyJava(string src, string target, string className)
        {
            if (!Directory.GetParent(target).Exists)
            {
                Directory.CreateDirectory((Directory.GetParent(target).FullName));
            }
            string[] files = Directory.GetFiles(src);
            for (int i = 0; i < files.Length; i++)
            {
                // 抽取本类及内部类
                if (files[i].Contains(@"\" + className + ".class") || files[i].Contains(@"\" + className + "$"))
                {
                    String targetTemp = target + files[i].Substring(files[i].LastIndexOf(@"\") + 1, files[i].Length - files[i].LastIndexOf(@"\") - 1);
                    try
                    {
                        File.Copy(files[i], targetTemp);
                    }
                    catch (IOException e)
                    {
                        throw e;
                    }
                }
            }
            return true;
        }
        #endregion 复制class文件

        #region 复制文件
        /// <summary>
        /// 复制文件
        /// </summary>
        /// <param name="src">源文件</param>
        /// <param name="target">目标文件</param>
        /// <returns>复制成功标记</returns>
        private bool copyFile(string src, string target)
        {
            if (!Directory.GetParent(target).Exists)
            {
                Directory.GetParent(target).Create();
            }
            try
            {
                if (Directory.Exists(src))
                {
                    if (!Directory.Exists(target))
                    {
                        Directory.CreateDirectory(target);
                    }
                }
                else
                {
                    File.Copy(src, target);
                }
            }
            catch (IOException e)
            {
                throw (e);
            }
            return true;
        }
        #endregion 复制文件

        #region 生成更新日志
        /// <summary>
        /// 生成更新日志
        /// </summary>
        /// <param name="sbLogs">日志字符串缓冲区</param>
        /// <param name="logItem">更新记录</param>
        /// <param name="changeItemCopyFlgs">文件抽取成功标识</param>
        private void createLogs(StringBuilder sbLogs, SvnLogEventArgs logItem, Hashtable changeItemCopyFlgs)
        {
            // 版本:999
            sbLogs.Append(Constants.LOG_VERSION).Append(Constants.COLON).Append(logItem.Revision).Append(Constants.HALF_SPACE);
            // 作者:XXX
            sbLogs.Append(Constants.LOG_AUTHOR).Append(Constants.COLON).Append(logItem.Author).Append(Constants.HALF_SPACE);
            // 时间:yyyy-MM-dd HH:mm:ss
            sbLogs.Append(Constants.LOG_TIME).Append(Constants.COLON).Append(logItem.Time.ToString("yyyy-MM-dd HH:mm:ss")).AppendLine(Constants.HALF_SPACE);
            // 修改内容:XXXX
            sbLogs.Append(Constants.LOG_UPDATE).Append(Constants.COLON).AppendLine(logItem.LogMessage);
            SvnChangeItemCollection changeItems = logItem.ChangedPaths;
            // svn操作类型:
            foreach (var changeItem in changeItems)
            {
                switch (changeItem.Action)
                {
                    case SvnChangeAction.Modify:
                        sbLogs.Append(Constants.SVN_CHANGE_ACTION_MODIFY).Append(Constants.COLON);
                        break;

                    case SvnChangeAction.Delete:
                        sbLogs.Append(Constants.SVN_CHANGE_ACTION_DELETE).Append(Constants.COLON);
                        break;

                    case SvnChangeAction.Add:
                        sbLogs.Append(Constants.SVN_CHANGE_ACTION_ADD).Append(Constants.COLON);
                        break;

                    case SvnChangeAction.Replace:
                        sbLogs.Append(Constants.SVN_CHANGE_ACTION_REPLACE).Append(Constants.COLON);
                        break;

                    case SvnChangeAction.None:
                    default:
                        sbLogs.Append(Constants.SVN_CHANGE_ACTION_NONE).Append(Constants.COLON);
                        break;
                }
                // 是否抽取成功
                if (Convert.ToBoolean(changeItemCopyFlgs[changeItem.Path]))
                {
                    sbLogs.Append(Constants.COPY_SUCCESS);
                }
                else
                {
                    sbLogs.Append(Constants.COPY_FAIL);
                }
                // svn路径
                sbLogs.AppendLine(changeItem.Path);
            }
            sbLogs.AppendLine();
        }
        #endregion 生成更新日志

        #endregion 方法
    }
}