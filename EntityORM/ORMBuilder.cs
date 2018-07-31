using System;
using System.Collections.Generic;
using System.Text;
using Entity;
using System.IO;
using System.Windows.Forms;
using System.ComponentModel;
using System.Xml;
using Microsoft.Data.Sqlite;
using System.Configuration;


namespace EntityORM
{
    /// <summary>
    /// 用于将Entity生成数据库表和外键关联以及中间表的类
    /// </summary>
    public class ORMBuilder
    {

        private static SqliteConnection  con;

        /// <summary>
        /// 获取配置文件的值
        /// </summary>
        /// <param name="keyName"></param>
        /// <returns></returns>
        public static  string GetConfigAppSetValueByKey(string keyName)
        {
            return ConfigurationSettings.AppSettings [keyName].ToString ();
        }

        /// 设置app.config中的某个key的value.
        /// </summary>
        /// <param name="AppKey">key</param>
        /// <param name="AppValue">value</param>
        public static  void SetValue(string AppKey, string AppValue)
        {
            XmlDocument xDoc = new XmlDocument();
            xDoc.Load(Application.StartupPath + "\\EntityORM.exe.config");
            XmlNode xNode;
            XmlElement xElem1;
            XmlElement xElem2;
            xNode = xDoc.SelectSingleNode("//appSettings");
            xElem1 = (XmlElement)xNode.SelectSingleNode("//add[@key='" + AppKey + "']");
            if (xElem1 != null)
            {
                xElem1.SetAttribute("value", AppValue);
            }
            else
            {
                xElem2 = xDoc.CreateElement("add");
                xElem2.SetAttribute("key", AppKey);
                xElem2.SetAttribute("value", AppValue);
                xNode.AppendChild(xElem2);
            }
            xDoc.Save(Application.StartupPath + "\\EntityORM.exe.config");

        }



        /// <summary>
        /// 单条执行语句
        /// </summary>
        /// <param name="conn"></param>
        /// <param name="sqlText"></param>
        public  static void AddSQLiteTable(string dbName,string dbFilePath, string sqlText)
        {
            if (con ==null)
            {
                con = new SqliteConnection(string.Format("Data Source={0}{1}", dbFilePath,dbName));
            }
            SqliteCommand cmd = new SqliteCommand(sqlText, con);
            if (!(con.State == System.Data.ConnectionState.Open))
                con.Open();
            cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// 在将实体生成数据库表之前,如果表已经存在,则将表先删除
        /// </summary>
        /// <param name="einfo">EntityInfo</param>
        /// <returns>SQL语句</returns>
        public static string DropTableIfExists(EntityInfo einfo)
        {
            if (einfo == null)
            return string.Empty;
            return string.Format("Drop Table If Exists [{0}];", einfo.EntityName);
        }

        /// <summary>
        /// 生成普通属性,如(字符串,数字,日期)在创建表中的SQL语句,位于create table 中的
        /// </summary>
        /// <param name="finfo">Entity</param>
        /// <returns>FieldName nvarchar(20) NOT NULL default 'LvBin'</returns>
        private static string GenerateField(FieldAttribute finfo)
        {
            //[PCode] nvarchar(20) COLLATE NOCASE
            StringBuilder sb = new StringBuilder();
            sb.Append(string .Format ("[{0}]",finfo.FieldName));
            sb.Append(" ");
            sb.Append(ConvertCSharpToSQL(finfo.DataType));
            sb.Append(" ");
            if (finfo.AllowNull)
                sb.Append("NULL ");
            else 
                sb.Append("NOT NULL ");            
            if (!string.IsNullOrEmpty(finfo.DefaultValue))
            {
                sb.Append("default ");
                sb.Append(finfo.DefaultValue);                
            }
            if (finfo.Unique)
                sb.Append(" UNIQUE");
            return sb.ToString();
        }

        /// <summary>
        /// 生成外键引用的实体Id
        /// </summary>
        /// <param name="refinfo">以REF开头的属性</param>
        /// <returns>实体名称+Id bigint</returns>
        private static string GenerateREFEntity(REFAttribute refAtt)
        {
            return refAtt.REFFieldName + " integer";
        }       

        /// <summary>
        /// 将Entity生成create table语句
        /// </summary>
        /// <param name="einfo">EntityInfo</param>
        /// <returns>SQL :create table语句</returns>
        public static string GenerateTableByEntityInfo(EntityInfo einfo)
        {
            if (einfo == null)
                return string.Empty;
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("CREATE TABLE IF NOT EXISTS  [{0}] ( [Id] integer PRIMARY KEY AUTOINCREMENT NOT NULL", einfo.EntityName));
            for (int i = 0; i < einfo.Properties.Count; i++)
            {
                sb.Append(",");
                sb.Append(GenerateField(einfo.Properties[i]));                
            }
            for (int j = 0; j < einfo.References.Count; j++)
            {
                sb.Append(",");
                sb.Append(GenerateREFEntity(einfo.References[j]));
            }
            if (einfo.REFSelf)
            {
                sb.Append(",ParentId  integer");
            }
            sb.Append(");");
            return sb.ToString();
        }
        /// <summary>
        /// 将Entity的特性中配置的值转换为sql server中的数据类型,如将string-->varchar,long-->bigint
        /// </summary>
        /// <param name="cs">string|long</param>
        /// <returns>varchar|bigint</returns>
        private static string ConvertCSharpToSQL(string cs)
        {
            if (cs.StartsWith("string"))
            {
                string strt = cs.Substring(0, cs.Length - 1); //去掉最后一个括号
                return string.Format("nvarchar({0}) COLLATE NOCASE", strt.Substring(strt.IndexOf("(") + 1));
            }
            else if (cs.IndexOf("text") > -1)
                return "text(2147483647) COLLATE NOCASE";
            string result = string.Empty;
            switch (cs)
            {
                case "int": result = "integer"; break;
                case "long": result = "bigint"; break;
                case "float": result = "float"; break;
                case "decimal": result = "decimal(20,2)"; break; //保留两位小数
                case "Date": result = "datetime"; break;
                case "SmallDate": result = "smalldatetime"; break;
            }
            return result;
        }

        public static string LoadInitialData(Panel pnlCheck)
        {
            StringBuilder sb = new StringBuilder();
            StreamReader sreader = null;
            try
            {
                if (File.Exists(AppDomain.CurrentDomain.BaseDirectory + "Sql_Initial.txt"))
                {
                    sreader = File.OpenText(AppDomain.CurrentDomain.BaseDirectory + "Sql_Initial.txt");
                    string strRecord = sreader.ReadLine();
                    
                    while (strRecord != null)
                    {
                        if(IfShouldInitialData(strRecord,pnlCheck))
                        //if (!string.IsNullOrEmpty(strRecord.Trim()))
                        {
                            sb.Append(strRecord);
                            sb.Append("\r\n");
                        }
                        strRecord = sreader.ReadLine();
                    }
                    sreader.Close();
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return sb.ToString();
        }

        private static bool IfShouldInitialData(string sqlinsert, Panel pnl)
        {
            bool result = false;
            if (string.IsNullOrEmpty(sqlinsert))
                return false;
            foreach (Control ctrl in pnl.Controls)
            {
                if (((CheckBox)ctrl).Checked && sqlinsert.IndexOf(ctrl.Name) > 5)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}
