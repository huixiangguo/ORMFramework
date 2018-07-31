using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using Microsoft.Data.Sqlite;
namespace Entity
{
    /// <summary>
    /// 用于处理数据库连接，执行SQL语句的公用类
    /// </summary>
    public class DBSQLiteHelper
    {
        private static SqliteConnection con;
        public static SqliteConnection GetSQLiteConnection()
        {
            if (con ==null )
            {
                var sqliteFilePath = string.Format("{0}{1}", AppDomain.CurrentDomain.BaseDirectory, "Bin\\Topbaike.db");
                con = new SqliteConnection(string .Format("Data Source={0}",sqliteFilePath));
            }
            if (con.State== ConnectionState.Closed)
            {
                con.Open();
            }
            return con;
        }


        /// <summary>
        /// 执行SQL语句，包括新增记录，修改记录，删除记录
        /// </summary>
        /// <param name="sql">需要执行的SQL语句</param>
        /// <returns>受影响的记录条数</returns>
        public static int ExecuteUpdate(string sql)
        {
            SqliteConnection connection = GetSQLiteConnection();
            using (SqliteCommand command = new SqliteCommand(sql,connection))
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
                return command.ExecuteNonQuery();
            }

        }

        /// <summary>
        /// 启动事务插入SQLlite数据库数据
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public static bool  TransactionSQLlist(List <string > list)
        {
            SqliteConnection conn = GetSQLiteConnection();
            using (SqliteCommand cmd = new SqliteCommand())
            {
                cmd.Connection = conn;
                if (!(conn.State == ConnectionState.Open))
                {
                    conn.Open();
                }
                SqliteTransaction tx = conn.BeginTransaction();
                cmd.Transaction = tx;
                try
                {
                    for (int n = 0; n < list.Count; n++)
                    {
                        string strsql = list[n].ToString();
                        if (strsql.Trim().Length > 1)
                        {
                            cmd.CommandText = strsql;
                            cmd.ExecuteNonQuery();
                        }
                    }
                    tx.Commit();
                    return true;
                }
                catch (SqliteException E)
                {
                    tx.Rollback();
                    return false;
                }
            }
        }

        /// <summary>
        /// 执行用于返回一个整数值的SQL语句
        /// </summary>
        /// <param name="sql">待执行的SQL语句，格式如select count(*) from YourTable</param>
        /// <returns>count(*)的值</returns>
        public static int ExecuteScalar(string sql)
        {
            SqliteConnection connection = GetSQLiteConnection();
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
                return Convert.ToInt32(command.ExecuteScalar());
            }

        }

        /// <summary>
        /// 执行SQL查询
        /// </summary>
        /// <param name="sql">待执行的查询语句</param>
        /// <returns>查询结果数据集</returns>
        public static DataSet ExecuteQuery(string sql)
        {
            DataSet ds = new DataSet();
            SqliteConnection connection = GetSQLiteConnection();
            using (SqliteCommand command = new SqliteCommand(sql, connection))
            {
                var reader = command.ExecuteReader();
                if (!(connection.State == ConnectionState.Open))
                {
                    connection.Open();
                }
                DataTable table = new DataTable();
                int fieldCount = reader.FieldCount;
                for (int i = 0; i < fieldCount; i++)
                {
                    table.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
                }
                table.BeginLoadData();
                object[] values = new object[fieldCount];
                while (reader.Read())
                {
                    reader.GetValues(values);
                    table.LoadDataRow(values, true);
                }
                table.EndLoadData();
                ds.Tables.Add(table);
            }
            return ds;
        }

        /// <summary>
        /// 从DataSet中取出数据放到实体实例中
        /// </summary>
        /// <param name="objEntity">实体对象</param>
        /// <param name="pinfo">需要放进数据的属性</param>
        /// <param name="table">数据表</param>
        /// <param name="rowIndex">行号</param>
        public static void FetchPropertyValue(object objEntity,FieldAttribute fieldAtt,DataTable table,int rowIndex)
        {
            if (table.Rows[rowIndex][fieldAtt.FieldName] == DBNull.Value)
                return;
            string strValue = table.Rows[rowIndex][fieldAtt.FieldName].ToString();
            if (fieldAtt.DataType.StartsWith("string") || fieldAtt.DataType.IndexOf("text") > -1)
                objEntity.GetType().GetProperty(fieldAtt.FieldName).SetValue(objEntity, strValue, null);
            else
            {
                switch (fieldAtt.DataType)
                {
                    case "int": if (!string.IsNullOrEmpty(strValue)) objEntity.GetType().GetProperty(fieldAtt.FieldName).SetValue(objEntity, Convert.ToInt32(strValue), null); break;
                    case "long": if (!string.IsNullOrEmpty(strValue)) objEntity.GetType().GetProperty(fieldAtt.FieldName).SetValue(objEntity, Convert.ToInt64(strValue), null); break;
                    case "float": if (!string.IsNullOrEmpty(strValue)) objEntity.GetType().GetProperty(fieldAtt.FieldName).SetValue(objEntity, Convert.ToSingle(strValue), null); break;
                    case "decimal": if (!string.IsNullOrEmpty(strValue)) objEntity.GetType().GetProperty(fieldAtt.FieldName).SetValue(objEntity, Convert.ToDecimal(strValue), null); break;
                    case "Date":
                    case "SmallDate": if (!string.IsNullOrEmpty(strValue)) objEntity.GetType().GetProperty(fieldAtt.FieldName).SetValue(objEntity, Convert.ToDateTime(strValue), null); break;
                }
            }
        }

        /// <summary>
        /// 将字符串类型的数据经转换后付给指定的实体对象
        /// </summary>
        /// <param name="objEntity">实体对象</param>
        /// <param name="pinfo">需要赋值的属性名称</param>
        /// <param name="table">字符串值</param>
        public static void FetchPropertyValue(object objEntity,string fieldName,string propValue)
        {
            if (string.IsNullOrEmpty(propValue))
                return;
            EntityInfo einfo = DLLAnalysis.GetEntityInfoByType(objEntity.GetType());
            FieldAttribute fieldAtt = null;
            foreach (FieldAttribute fatt in einfo.Properties)
            {
                if (fatt.FieldName.Equals(fieldName))
                {
                    fieldAtt = fatt;
                    break;
                }
            }
            if (fieldAtt.DataType.StartsWith("string") || fieldAtt.DataType.IndexOf("text") > -1)
                objEntity.GetType().GetProperty(fieldName).SetValue(objEntity, propValue, null);
            else
            {
                switch (fieldAtt.DataType)
                {
                    case "int": objEntity.GetType().GetProperty(fieldName).SetValue(objEntity, Convert.ToInt32(propValue), null); break;
                    case "long": objEntity.GetType().GetProperty(fieldName).SetValue(objEntity, Convert.ToInt64(propValue), null); break;
                    case "float": objEntity.GetType().GetProperty(fieldName).SetValue(objEntity, Convert.ToSingle(propValue), null); break;
                    case "decimal": objEntity.GetType().GetProperty(fieldName).SetValue(objEntity, Convert.ToDecimal(propValue), null); break;
                    case "Date":
                    case "SmallDate": objEntity.GetType().GetProperty(fieldName).SetValue(objEntity, Convert.ToDateTime(propValue), null); break;
                }
            }
        }

        public static void FetchREFEntity(object objEntity, REFAttribute refAtt, DataTable table)
        {
            int rindex = -1;//根据objEntity的Id属性,在table中找到对应的行数
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (((BaseEntity)objEntity).Id == Convert.ToInt64(table.Rows[i]["E_" + objEntity.GetType().Name + "Id"]))
                {
                    rindex = i;
                    break;
                }
            }
            //对象的该引用属性为空值
            if (table.Rows[rindex]["E_" + refAtt.REFEntityName + "Id"] == DBNull.Value)
                return;

            object objRef = DLLAnalysis.GetEntityInstance(refAtt.REFEntityName);
            ((BaseEntity)objRef).Id = Convert.ToInt64(table.Rows[rindex]["E_"+refAtt.REFEntityName+"Id"]);
            EntityInfo refeinfo = DLLAnalysis.GetEntityInfoByType(objRef.GetType());
            foreach (FieldAttribute fieldAtt in refeinfo.Properties)
            {
                FetchPropertyValue(objRef, fieldAtt, table, rindex);
            }
            objEntity.GetType().GetProperty(refAtt.PropertyName).SetValue(objEntity, objRef,null);
        }

        public static void FetchSet(object objEntity, SETAttribute setAtt, DataTable table)
        {
            ArrayList list = new ArrayList();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //如果该实体的此集合属性为空
                if (table.Rows[i][setAtt.ElementEntityId] == DBNull.Value)
                    continue;
                if (((BaseEntity)objEntity).Id == Convert.ToInt64(table.Rows[i][setAtt.ElementEntityId]))
                {
                    object objElement = DLLAnalysis.GetEntityInstance(setAtt.ElementEntityName);
                    if (Convert.IsDBNull(table.Rows[i]["E_" + setAtt.ElementEntityName + "Id"]))
                        continue;
                    ((BaseEntity)objElement).Id = Convert.ToInt64(table.Rows[i]["E_" + setAtt.ElementEntityName + "Id"]);
                    EntityInfo elementEinfo = DLLAnalysis.GetEntityInfoByType(objElement.GetType());
                    foreach (FieldAttribute fieldAtt in elementEinfo.Properties)
                    {
                        FetchPropertyValue(objElement, fieldAtt, table, i);
                    }
                    foreach (REFAttribute refAtt in elementEinfo.References)
                    {
                        if (refAtt.REFEntityName.Equals(objEntity.GetType().Name))
                            objElement.GetType().GetProperty(refAtt.PropertyName).SetValue(objElement, objEntity, null);
                    }
                    list.Add(objElement);
                }
            }
            objEntity.GetType().GetProperty(setAtt.PropertyName).SetValue(objEntity, list, null);
        }

        /// <summary>
        /// 获取实体的父对象
        /// </summary>
        /// <param name="objEntity">宿主实体</param>
        /// <param name="table">数据集</param>
        /// <param name="rIndex">取数据集的行数</param>
        public static void FetchParent(object objEntity, DataTable table, int rIndex)
        {
            EntityInfo einfo = DLLAnalysis.GetEntityInfoByType(objEntity.GetType());
            object pid = table.Rows[rIndex]["PTId"];
            if (pid == DBNull.Value || string.IsNullOrEmpty(pid.ToString()))
                return;
            BaseEntity pEntity = (BaseEntity)DLLAnalysis.GetEntityInstance(einfo.EntityName);
            pEntity.Id = Convert.ToInt64(pid);
            try
            {
                pEntity.GetType().GetProperty(einfo.Properties[0].FieldName).SetValue(pEntity, table.Rows[rIndex]["PTC"], null);
                objEntity.GetType().GetProperty("Parent" + einfo.EntityName).SetValue(objEntity, pEntity, null);
            }
            catch (Exception ex) { }
            return;
        }

        public static void FetchChildren(object objEntity, DataTable table)
        {
            EntityInfo einfo = DLLAnalysis.GetEntityInfoByType(objEntity.GetType());
            ArrayList list = new ArrayList();
            for (int i = 0; i < table.Rows.Count; i++)
            {
                //如果该实体的此集合属性为空
                if (table.Rows[i]["CTId"] == DBNull.Value || table.Rows[i]["CTPId"] == DBNull.Value)
                    continue;
                if (((BaseEntity)objEntity).Id == Convert.ToInt64(table.Rows[i]["CTPId"]))
                {
                    object objElement = DLLAnalysis.GetEntityInstance(einfo.EntityName);
                    //取自身引用子集合时，每个子对象只有Id是载入的，其他属性都没获取
                    ((BaseEntity)objElement).Id = Convert.ToInt64(table.Rows[i]["CTId"]);
                    //objElement.GetType().GetProperty(einfo.Properties[0].FieldName).SetValue(objElement, table.Rows[i]["CTC"], null);                    
                    list.Add(objElement);
                }
            }
            objEntity.GetType().GetProperty("Child" + einfo.EntityName + "s").SetValue(objEntity,list,null);
        }


        /// <summary>
        /// 根据从数据库检索出的数据集从中组装出实体对象实例
        /// </summary>
        /// <param name="einfo">实体对象媒数据</param>
        /// <param name="fetchType">关联对象加载方式</param>
        /// <param name="table">数据集</param>
        /// <param name="rowIndex">实体对象所处的数据集行数</param>
        /// <returns>实体对象实例</returns>
        public static BaseEntity FetchEntity(EntityInfo einfo,Fetch fetchType,DataTable table, int rowIndex)
        {
            BaseEntity entity = (BaseEntity)DLLAnalysis.GetEntityInstance(einfo.EntityName);
            entity.Id = Convert.ToInt64(table.Rows[rowIndex][einfo.IdNameInSelect]);
            foreach (FieldAttribute pinfo in einfo.Properties)
            {
                DBSQLiteHelper.FetchPropertyValue(entity, pinfo, table, rowIndex);
            }
            foreach (REFAttribute refAtt in einfo.References)
            {
                if ((fetchType == Fetch.Default && !refAtt.LazyLoad) || fetchType == Fetch.REFS || fetchType == Fetch.REFSandSets)
                    DBSQLiteHelper.FetchREFEntity(entity, refAtt, table);
            }
            foreach (SETAttribute setAtt in einfo.Sets)
            {
                if ((fetchType == Fetch.Default && !setAtt.LazyLoad) || fetchType == Fetch.SETS || fetchType == Fetch.REFSandSets)
                    DBSQLiteHelper.FetchSet(entity, setAtt, table);
            }
            //如果实体有对自身实体对象的引用
            if (einfo.REFSelf)
            {
                if (fetchType == Fetch.REFS)
                    FetchParent(entity, table, rowIndex);
                else if (fetchType == Fetch.SETS)
                { 
                    FetchChildren(entity, table);
                }
                else if (fetchType == Fetch.REFSandSets)
                {
                    FetchParent(entity, table, rowIndex);
                    FetchChildren(entity, table);
                }
            }
            return entity;
        }
       
        /// <summary>
        /// 将实体的REF特性集合生成left join语句
        /// </summary>
        /// <param name="refList">REFAttribute集合</param>
        /// <param name="sbSelectstr">在生成left join的同时也改变select列表里的字符串</param>
        /// <param name="fetchType">Fetch类型</param>
        /// <returns>left join语句</returns>
        public static string GenerateLeftJoinSQL(List<REFAttribute> refList, ref StringBuilder sbSelectstr,Fetch fetchType)
        {
            StringBuilder sbLeftJoin = new StringBuilder();
            foreach (REFAttribute refAtt in refList)
            {
                if ((fetchType == Fetch.Default && !refAtt.LazyLoad) || fetchType == Fetch.REFS || fetchType == Fetch.REFSandSets)
                {
                    sbLeftJoin.Append(string.Format(" left join {0} on {1}.{2}={0}.Id",
                       refAtt.REFEntityName, refAtt.EntityName, refAtt.REFFieldName));
                    sbSelectstr.Append(string.Format(",{0}.id as E_{0}Id,{0}.*",refAtt.REFEntityName));
                }
            }
            return sbLeftJoin.ToString();
        }

        public static string GenerateSetSQL(List<SETAttribute> setList, ref StringBuilder sbSelectstr,Fetch fetchType)
        {
            StringBuilder sbSet = new StringBuilder();
            foreach (SETAttribute setAtt in setList)
            {
                if ((fetchType == Fetch.Default && !setAtt.LazyLoad) || fetchType == Fetch.SETS || fetchType == Fetch.REFSandSets)
                {
                    //如果不采用中间表的形式存储集合关联
                    if (!setAtt.IsMidTable)
                    {
                        sbSet.Append(string.Format(" left join {0} on {1}.id = {0}.{2}", setAtt.ElementEntityName, setAtt.EntityName, setAtt.ElementEntityId));
                        sbSelectstr.Append(string.Format(",{0}.id as E_{0}Id,{0}.*", setAtt.ElementEntityName));
                    }
                    //采用中间表的形式的查询SQL语句生成
                    else
                    {

                    }
                }
            }
            return sbSet.ToString();
        }

        /// <summary>
        /// 根据EntityInfo信息生成查询实体的SQL语句
        /// </summary>
        /// <param name="einfo">EntityInfo</param>
        /// <param name="sbSelectstr">用于追加select *的stringbuilder</param>
        /// <param name="fetchType">加载方式</param>
        /// <returns>sql语句</returns>
        public static string GenerateREFSelfSQL(EntityInfo einfo,ref StringBuilder sbSelectstr,Fetch fetchType)
        {
            StringBuilder sb = new StringBuilder();
            if (fetchType == Fetch.REFS)
            {
                sb.Append(string.Format(" left join {0} as PT on {0}.ParentId=PT.Id",einfo.EntityName));
                sbSelectstr.Append(string.Format(",PT.Id as PTId,PT.{0} as PTC",einfo.Properties[0].FieldName));
            }
            else if (fetchType == Fetch.SETS)
            {
                sb.Append(string.Format(" left join {0} as CT on {0}.Id=CT.ParentId", einfo.EntityName));
                sbSelectstr.Append(string.Format(",CT.Id as CTId,CT.ParentId as CTPId,CT.{0} as CTC", einfo.Properties[0].FieldName));
            }
            else if (fetchType == Fetch.REFSandSets)
            {
                sb.Append(string.Format(" left join {0} as PT on {0}.ParentId=PT.Id", einfo.EntityName));
                sbSelectstr.Append(string.Format(",PT.Id as PTId,PT.{0} as PTC", einfo.Properties[0].FieldName));
                sb.Append(string.Format(" left join {0} as CT on {0}.Id=CT.ParentId", einfo.EntityName));
                sbSelectstr.Append(string.Format(",CT.Id as CTId,CT.ParentId as CTPId,CT.{0} as CTC", einfo.Properties[0].FieldName));
            }
            return sb.ToString();
        }

        public static ArrayList GetEntityTreeList(ArrayList listRoot)
        {
            if(listRoot==null || listRoot.Count<1)
                return null;            
            ArrayList list = new ArrayList();
            foreach (BaseEntity entity in listRoot)
                PutNodeAndChildNodes(ref list,entity);
            return list;
        }
        private static void PutNodeAndChildNodes(ref ArrayList list, BaseEntity entity)
        {
            list.Add(entity);
            entity.Load(entity.Id, Fetch.REFSandSets);
            ArrayList clist = (ArrayList)entity.GetType().GetProperty("Child" + entity.GetType().Name + "s").GetValue(entity, null);
            if (clist != null && clist.Count > 0)
            {
                foreach (BaseEntity centity in clist)
                    PutNodeAndChildNodes(ref list, centity);
            }
        }
    }
}
