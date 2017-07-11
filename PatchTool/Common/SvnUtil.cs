using SharpSvn;
using SharpSvn.Remote;
using SharpSvn.Security;
using System;
using System.Collections.ObjectModel;
using System.IO;

namespace PatchTool.Common
{
    internal class SvnUtil
    {
        #region 鉴权

        /// <summary>
        /// 鉴权
        /// </summary>
        /// <param name="client">svn客户端</param>
        /// <param name="userName">svn用户名</param>
        /// <param name="password">svn密码</param>
        public static void authenticate(SvnClient client, string userName, string password)
        {
            client.Authentication.UserNamePasswordHandlers +=
                new EventHandler<SvnUserNamePasswordEventArgs>(
                    delegate (object s, SvnUserNamePasswordEventArgs e)
                    {
                        e.UserName = userName;
                        e.Password = password;
                    });
        }

        #endregion 鉴权

        #region 取得版本记录

        /// <summary>
        /// 取得版本记录
        /// </summary>
        /// <param name="client">svn客户端</param>
        /// <param name="url">svn路径</param>
        /// <param name="start">开始版本号</param>
        /// <param name="end">结束版本号</param>
        /// <returns>更新记录</returns>
        public static Collection<SvnLogEventArgs> getLogs(SvnClient client, string url, long start, long end)
        {
            Uri fileUri = new Uri(@url);
            SvnRemoteSession remoteSession = new SvnRemoteSession(fileUri);
            string fileRelPath = remoteSession.MakeRepositoryRootRelativePath(fileUri);
            SvnRevisionRange revisionRange = new SvnRevisionRange(start, end);
            SvnLogArgs svnLogArgs = new SvnLogArgs(revisionRange);
            Collection<SvnLogEventArgs> logItems = new Collection<SvnLogEventArgs>();
            client.GetLog(fileUri, svnLogArgs, out logItems);
            return logItems;
        }

        /// <summary>
        /// 取得版本记录
        /// </summary>
        /// <param name="client">svn客户端</param>
        /// <param name="url">svn路径</param>
        /// <param name="dtStart">开始时间</param>
        /// <param name="dtEnd">结束时间</param>
        /// <returns>更新记录</returns>
        public static Collection<SvnLogEventArgs> getLogs(SvnClient client, string url, DateTime dtStart, DateTime dtEnd)
        {
            Uri fileUri = new Uri(@url);
            SvnRemoteSession remoteSession = new SvnRemoteSession(fileUri);
            string fileRelPath = remoteSession.MakeRepositoryRootRelativePath(fileUri);
            SvnRevisionRange revisionRange = new SvnRevisionRange(dtStart, dtEnd);
            SvnLogArgs svnLogArgs = new SvnLogArgs(revisionRange);
            Collection<SvnLogEventArgs> logItems = new Collection<SvnLogEventArgs>();
            client.GetLog(fileUri, svnLogArgs, out logItems);
            return logItems;
        }

        #endregion 取得版本记录

        #region 获取diff结果

        /// <summary>
        /// 获取svn的diff文件
        /// </summary>
        /// <param name="client">svn客户端</param>
        /// <param name="url">文件svn路径</param>
        /// <param name="start">开始版本</param>
        /// <param name="end">结束版本</param>
        /// <param name="stream">文件流</param>
        public static void getDiffResults(SvnClient client, string url, long start, long end, FileStream stream)
        {
            SvnTarget target = SvnTarget.FromString(url);
            SvnRevisionRange revisionRange = new SvnRevisionRange(start, end);
            client.Diff(target, revisionRange, stream);
        }

        #endregion 获取diff结果
    }
}