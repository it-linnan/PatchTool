namespace PatchTool.Common
{
    internal class Constants
    {
        public const string LOG_VERSION = "版本";
        public const string LOG_AUTHOR = "作者";
        public const string LOG_TIME = "时间";
        public const string LOG_UPDATE = "修改内容";
        public const string COLON = ":";
        public const string HALF_SPACE = " ";

        public const string SVN_CHANGE_ACTION_NONE = "    ";
        public const string SVN_CHANGE_ACTION_ADD = "新增";
        public const string SVN_CHANGE_ACTION_DELETE = "删除";
        public const string SVN_CHANGE_ACTION_MODIFY = "修改";
        public const string SVN_CHANGE_ACTION_REPLACE = "替换";

        public const string COPY_SUCCESS = "抽取成功";
        public const string COPY_FAIL = "抽取失败";

        public const string DIFF_ADD = "+";
    }
}