using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Collections;
using Microsoft.Data.Sqlite;
namespace Entity
{
    /// <summary>
    /// ���ڴ������ݿ����ӣ�ִ��SQL���Ĺ�����
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
        /// ִ��SQL��䣬����������¼���޸ļ�¼��ɾ����¼
        /// </summary>
        /// <param name="sql">��Ҫִ�е�SQL���</param>
        /// <returns>��Ӱ��ļ�¼����</returns>
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
        /// �����������SQLlite���ݿ�����
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
        /// ִ�����ڷ���һ������ֵ��SQL���
        /// </summary>
        /// <param name="sql">��ִ�е�SQL��䣬��ʽ��select count(*) from YourTable</param>
        /// <returns>count(*)��ֵ</returns>
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
        /// ִ��SQL��ѯ
        /// </summary>
        /// <param name="sql">��ִ�еĲ�ѯ���</param>
        /// <returns>��ѯ������ݼ�</returns>
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
        /// ��DataSet��ȡ�����ݷŵ�ʵ��ʵ����
        /// </summary>
        /// <param name="objEntity">ʵ�����</param>
        /// <param name="pinfo">��Ҫ�Ž����ݵ�����</param>
        /// <param name="table">���ݱ�</param>
        /// <param name="rowIndex">�к�</param>
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
        /// ���ַ������͵����ݾ�ת���󸶸�ָ����ʵ�����
        /// </summary>
        /// <param name="objEntity">ʵ�����</param>
        /// <param name="pinfo">��Ҫ��ֵ����������</param>
        /// <param name="table">�ַ���ֵ</param>
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
            int rindex = -1;//����objEntity��Id����,��table���ҵ���Ӧ������
            for (int i = 0; i < table.Rows.Count; i++)
            {
                if (((BaseEntity)objEntity).Id == Convert.ToInt64(table.Rows[i]["E_" + objEntity.GetType().Name + "Id"]))
                {
                    rindex = i;
                    break;
                }
            }
            //����ĸ���������Ϊ��ֵ
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
                //�����ʵ��Ĵ˼�������Ϊ��
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
        /// ��ȡʵ��ĸ�����
        /// </summary>
        /// <param name="objEntity">����ʵ��</param>
        /// <param name="table">���ݼ�</param>
        /// <param name="rIndex">ȡ���ݼ�������</param>
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
                //�����ʵ��Ĵ˼�������Ϊ��
                if (table.Rows[i]["CTId"] == DBNull.Value || table.Rows[i]["CTPId"] == DBNull.Value)
                    continue;
                if (((BaseEntity)objEntity).Id == Convert.ToInt64(table.Rows[i]["CTPId"]))
                {
                    object objElement = DLLAnalysis.GetEntityInstance(einfo.EntityName);
                    //ȡ���������Ӽ���ʱ��ÿ���Ӷ���ֻ��Id������ģ��������Զ�û��ȡ
                    ((BaseEntity)objElement).Id = Convert.ToInt64(table.Rows[i]["CTId"]);
                    //objElement.GetType().GetProperty(einfo.Properties[0].FieldName).SetValue(objElement, table.Rows[i]["CTC"], null);                    
                    list.Add(objElement);
                }
            }
            objEntity.GetType().GetProperty("Child" + einfo.EntityName + "s").SetValue(objEntity,list,null);
        }


        /// <summary>
        /// ���ݴ����ݿ�����������ݼ�������װ��ʵ�����ʵ��
        /// </summary>
        /// <param name="einfo">ʵ�����ý����</param>
        /// <param name="fetchType">����������ط�ʽ</param>
        /// <param name="table">���ݼ�</param>
        /// <param name="rowIndex">ʵ��������������ݼ�����</param>
        /// <returns>ʵ�����ʵ��</returns>
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
            //���ʵ���ж�����ʵ����������
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
        /// ��ʵ���REF���Լ�������left join���
        /// </summary>
        /// <param name="refList">REFAttribute����</param>
        /// <param name="sbSelectstr">������left join��ͬʱҲ�ı�select�б�����ַ���</param>
        /// <param name="fetchType">Fetch����</param>
        /// <returns>left join���</returns>
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
                    //����������м�����ʽ�洢���Ϲ���
                    if (!setAtt.IsMidTable)
                    {
                        sbSet.Append(string.Format(" left join {0} on {1}.id = {0}.{2}", setAtt.ElementEntityName, setAtt.EntityName, setAtt.ElementEntityId));
                        sbSelectstr.Append(string.Format(",{0}.id as E_{0}Id,{0}.*", setAtt.ElementEntityName));
                    }
                    //�����м�����ʽ�Ĳ�ѯSQL�������
                    else
                    {

                    }
                }
            }
            return sbSet.ToString();
        }

        /// <summary>
        /// ����EntityInfo��Ϣ���ɲ�ѯʵ���SQL���
        /// </summary>
        /// <param name="einfo">EntityInfo</param>
        /// <param name="sbSelectstr">����׷��select *��stringbuilder</param>
        /// <param name="fetchType">���ط�ʽ</param>
        /// <returns>sql���</returns>
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
