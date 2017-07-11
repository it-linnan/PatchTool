using System;
using System.IO;
using System.Xml.Linq;

namespace PatchTool.Common
{
    internal class XmlUtil
    {
        #region xml文档

        /// <summary>
        /// 取得xml文档
        /// </summary>
        /// <param name="filePath">路径</param>
        /// <returns>xml文档</returns>
        public static XDocument getXDoc(string filePath)
        {
            XDocument doc = new XDocument();
            if (File.Exists(filePath))
            {
                doc = XDocument.Load(@filePath, LoadOptions.None);
            }
            else
            {
                throw new Exception("文件不存在！");
            }
            return doc;
        }

        #endregion xml文档

        #region 取得根节点的指定子节点

        /// <summary>
        /// 取得根节点下的指定子节点
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="eles">子节点序列</param>
        /// <returns>节点下的指定子节点</returns>
        public static XElement getXEle(XDocument xDoc, params string[] eles)
        {
            XElement xEle = xDoc.Root;
            XElement childEle = xDoc.Root;
            for (int i = 0; i < eles.Length; i++)
            {
                childEle = xEle.Element(eles[i]);
                if (childEle == null)
                {
                    xEle.Add(new XElement(eles[i]));
                }
                xEle = xEle.Element(eles[i]);
            }
            return xEle;
        }

        #endregion 取得根节点的指定子节点

        #region 取得节点的指定属性值

        /// <summary>
        /// 得节点的指定属性值
        /// </summary>
        /// <param name="xEle">节点</param>
        /// <param name="attrName">属性名</param>
        /// <returns></returns>
        public static string getXEleAttrVal(XElement xEle, string attrName)
        {
            string attrVal = string.Empty;
            if (xEle != null)
            {
                XAttribute xAttr = xEle.Attribute(attrName);
                if (xAttr != null)
                {
                    attrVal = xAttr.Value;
                }
            }
            return attrVal;
        }

        #endregion 取得节点的指定属性值

        #region 新增或修改某节点的属性值

        /// <summary>
        /// 新增或修改某节点的属性值
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="attrName">属性名</param>
        /// <param name="attrValue">属性值</param>
        /// <param name="eles">子节点序列</param>
        public static void saveEleAttr(string filePath, string attrName, string attrValue, params string[] eles)
        {
            XDocument xDoc = getXDoc(filePath);
            XElement xEle = getXEle(xDoc, eles);
            xEle.SetAttributeValue(attrName, attrValue);
            xDoc.Save(filePath);
        }

        #endregion 新增或修改某节点的属性值

        #region 新建xml

        /// <summary>
        /// 新建xml文件
        /// </summary>
        /// <param name="filePath">文件路径</param>
        /// <param name="rootNodeName">根节点名</param>
        /// <returns>新建标记</returns>
        public static bool createXml(string filePath, string rootNodeName)
        {
            bool saveFlg = false;
            try
            {
                XDocument xDoc = new XDocument();
                xDoc.Add(new XElement(rootNodeName));
                xDoc.Save(filePath);
                saveFlg = true;
            }
            catch (Exception e)
            {
                throw e;
            }
            return saveFlg;
        }

        #endregion 新建xml
    }
}