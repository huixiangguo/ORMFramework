using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Reflection;
using System.Collections;


namespace Entity
{
    /// <summary>
    /// ����Framework 4.0 ��ǰ��֧�ַ��͵�Э������䡣����ܾ����List<Equation></Equation> ת������ListBaseEntity;
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class EntitySet<T> where T:BaseEntity
    {
        [NonSerialized]
        private EntityInfo _EInfo;
        private EntityInfo EInfo
        {
            get
            {
                if (_EInfo == null)
                    _EInfo = DLLAnalysis.GetEntityInfoByType(typeof(T));
                return _EInfo;
            }
        }

        private List<T> _EntityList = new List<T>();
        public List<T>  EntityList
        {
            get { return this._EntityList; }
            set { this._EntityList = value; }
        }

        /// <summary>
        /// ͨ�����ַ�ʽ��ȡ�����ԣ�������д���͵�toString()��
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public T FindEntity(string value)
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (value == EntityList[i].ToString())
                {
                    return EntityList[i] as T;
                }
            }
            return null;
        }

        public T FindEntity(long id)
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (id == EntityList[i].Id)
                {
                    return EntityList[i] as T;
                }
            }
            return null;
        }

        public T FindEntity(Predicate<T> pridicate)
        {
            for (int i = 0; i < EntityList.Count; i++)
            {
                if (pridicate(EntityList[i]))
                {
                    return EntityList[i] as T;
                }
            }
            return null;
        }

        private DataTable  _EntityDataTable;
        /// <summary>
        /// δת���ɶ��󣬷���Winform ��DataGridView ����Arrylist ��DataGridView ��������
        /// </summary>
        public DataTable  EntityDataTable
        {
            get 
            { 
                return _EntityDataTable;
            }
            set { _EntityDataTable = value; }
        }


        public EntitySet():this(Fetch.Default)
        {

        }

        public EntitySet(Fetch fetchType):this(fetchType ,false )
        {
  
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fetchType"></param>
        /// <param name="stringWhere">where +ʵ������.ʲôʲô</param>
        public EntitySet(Fetch fetchType, string stringWhere)
            : this(fetchType, stringWhere, false,true)
        {

        }

        private DataTable AddIndexToTable(DataTable dt)
        {
            DataColumn Col = dt.Columns.Add("GridIndex", typeof(string));
            Col.SetOrdinal(0);
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dt.Rows[i][0] = i + 1;
            }
            return dt;
        }

        /// <summary>
        /// ���ΪTrue�������ݼ���ת���ɶ���,ֱ�ӷ���DataTable
        /// </summary>
        /// <param name="fetchType"></param>
        /// <param name="GetDataTable"></param>
        public EntitySet(Fetch fetchType,bool GetDataTable)
        {
            string sqlPattern = string.Empty;
            StringBuilder sbSelectStr = new StringBuilder();
            string strLeftJoin = string.Empty;
            string sql = string.Empty;
            DataSet ds = null;

            sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
            sqlPattern = "select {0} from {1} {2}";
            strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
            strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
            if (EInfo.REFSelf)
                strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
            sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin);
            ds = DBSQLiteHelper.ExecuteQuery(sql);
            if (GetDataTable)
            {
                EntityDataTable =this.AddIndexToTable(ds.Tables[0]);
                return;
            }
            else
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add((T)DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }
            }
        }  //order by ProjectCode

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="fetchType"></param>
        /// <param name="GetDataTable"></param>
        /// <param name="order">order by ProjectCode</param>
        public EntitySet(Fetch fetchType, bool GetDataTable,string order)
        {
            string sqlPattern = string.Empty;
            StringBuilder sbSelectStr = new StringBuilder();
            string strLeftJoin = string.Empty;
            string sql = string.Empty;
            DataSet ds = null;

            sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
            sqlPattern = "select {0} from {1} {2}";
            if (!string.IsNullOrEmpty(order))
            {
                sqlPattern += " "+order;
            }
            strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
            strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
            if (EInfo.REFSelf)
                strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
            sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin);
            ds = DBSQLiteHelper.ExecuteQuery(sql);
            if (GetDataTable)
            {
                EntityDataTable = this.AddIndexToTable(ds.Tables[0]);
                return;
            }
            else
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add((T)DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }
            }
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="fetchType"></param>
        /// <param name="GetDataTable"></param>
        /// <param name="order">order by ProjectCode</param>
        public EntitySet(Fetch fetchType, bool GetDataTable, bool loadEntity,string order)
        {
            string sqlPattern = string.Empty;
            StringBuilder sbSelectStr = new StringBuilder();
            string strLeftJoin = string.Empty;
            string sql = string.Empty;
            DataSet ds = null;

            sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
            sqlPattern = "select {0} from {1} {2}";
            if (!string.IsNullOrEmpty(order))
            {
                sqlPattern += " " + order;
            }
            strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
            strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
            if (EInfo.REFSelf)
                strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
            sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin);
            ds = DBSQLiteHelper.ExecuteQuery(sql);
            if (GetDataTable)
            {
                EntityDataTable = this.AddIndexToTable(ds.Tables[0]);
            }
            if (loadEntity)
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add((T)DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }
            }
        } 

        public EntitySet(Fetch fetchType, string whereString, bool GetDataTable,bool loadEntity)
        {
            string sqlPattern = string.Empty;
            StringBuilder sbSelectStr = new StringBuilder();
            string strLeftJoin = string.Empty;
            string sql = string.Empty;
            DataSet ds = null;

            sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
            sqlPattern = "select {0} from {1} {2} {3}";
            strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
            strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
            if (EInfo.REFSelf)
                strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
            sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin,whereString);
            ds = DBSQLiteHelper.ExecuteQuery(sql);
            if (GetDataTable)
            {
                EntityDataTable = this.AddIndexToTable(ds.Tables[0]);
            }
            if (loadEntity)
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add((T)DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }
            }
        }


        public EntitySet(Fetch fetchType, bool GetDataTable, bool loadEntity)
        {
            string sqlPattern = string.Empty;
            StringBuilder sbSelectStr = new StringBuilder();
            string strLeftJoin = string.Empty;
            string sql = string.Empty;
            DataSet ds = null;

            sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
            sqlPattern = "select {0} from {1} {2} ";
            strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
            strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
            if (EInfo.REFSelf)
                strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
            sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin);
            ds = DBSQLiteHelper.ExecuteQuery(sql);
            if (GetDataTable)
            {
                EntityDataTable = this.AddIndexToTable(ds.Tables[0]);
            }
            if (loadEntity)
            {
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add((T)DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }
            }
        }
    }

        /// <summary>
        /// �Ƿ�����
        /// </summary>
        public class EntitySet
        {
            private EntityInfo _EInfo;
            private EntityInfo EInfo
            {
                get
                {
                    return _EInfo;
                }
                set { _EInfo = value; }
            }

            private ArrayList _EntityList = new ArrayList();
            public ArrayList EntityList
            {
                get { return this._EntityList; }
                set { this._EntityList = value; }
            }

            public List<BaseEntity> GetEntityList
            {
                get
                {
                    List<BaseEntity> list = new List<BaseEntity>();
                    if (this.EntityList!=null && this.EntityList.Count>0)
                    {
                        foreach (var item in this.EntityList)
                        {
                            list.Add((BaseEntity)item);
                        }
                    }
                    return list;
                }
            }

            public EntitySet(string entityName)
                : this(entityName, Fetch.Default)
            {
            }

            public EntitySet(string entityName, Fetch fetchType)
            {
                EInfo = DLLAnalysis.GetEntityInfoByType(DLLAnalysis.GetEntityInstance(entityName).GetType());
                string sqlPattern = string.Empty;
                StringBuilder sbSelectStr = new StringBuilder();
                string strLeftJoin = string.Empty;
                string sql = string.Empty;
                DataSet ds = null;

                sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
                sqlPattern = "select {0} from {1} {2}";
                strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
                strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
                sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin);
                ds = DBSQLiteHelper.ExecuteQuery(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add(DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }

            }

            public EntitySet(string entityName, Fetch fetchType, string whereString)
            {
                EInfo = DLLAnalysis.GetEntityInfoByType(DLLAnalysis.GetEntityInstance(entityName).GetType());
                string sqlPattern = string.Empty;
                StringBuilder sbSelectStr = new StringBuilder();
                string strLeftJoin = string.Empty;
                string sql = string.Empty;
                DataSet ds = null;

                sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
                sqlPattern = "select {0} from {1} {2} {3}";
                strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
                strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
                if (EInfo.REFSelf)
                    strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
                sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin, whereString);
                ds = DBSQLiteHelper.ExecuteQuery(sql);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        long rEntityId = Convert.ToInt64(ds.Tables[0].Rows[i][EInfo.IdNameInSelect]);
                        if (i == 0 || (EntityList.Count > 0 && ((BaseEntity)EntityList[EntityList.Count - 1]).Id != rEntityId))
                        {
                            EntityList.Add(DBSQLiteHelper.FetchEntity(EInfo, fetchType, ds.Tables[0], i));
                        }
                    }
                }
                
            }
        
    }
}
    
