using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Entity
{
    [Serializable]
    public abstract class BaseEntity
    {
        private long _Id;
        public long Id
        {
            get { return this._Id; }
            set { this._Id = value; }
        }

        /// <summary>
        /// 用来显示别名
        /// </summary>
        public virtual string DisplayName
        {
            get;
            set;
        }

        private EntityInfo _EInfo;
        private EntityInfo EInfo
        {
            get
            {
                if (_EInfo == null)
                    this._EInfo=DLLAnalysis.GetEntityInfoByType(this.GetType());
                return this._EInfo;
            }
        }

        public static bool Exist<T>(string namevalue) where T : BaseEntity
        {
            EntitySet<T> es = new EntitySet<T>(Fetch.Default);
            if (es.EntityList.Count>0)
            {
               T entity = es.FindEntity(namevalue);
               if (entity!=null)
               {
                   return true;
               }
            }
            return false;
        }
        
        /// <summary>
        ///  比较的时候需要用到引用实体的情况
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="namevalue"></param>
        /// <param name="fetch"></param>
        /// <returns></returns>
        public static bool Exist<T>(string namevalue,Fetch fetch) where T : BaseEntity
        {
            EntitySet<T> es = new EntitySet<T>(fetch);
            if (es.EntityList.Count > 0)
            {
                T entity = es.FindEntity(namevalue);
                if (entity != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 存在实体并且换回存在的实体
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="namevalue"></param>
        /// <param name="existEntity"></param>
        /// <returns></returns>
        public static bool Exist<T>(string namevalue,out T existEntity ) where T : BaseEntity
        {
            EntitySet<T> es = new EntitySet<T>(Fetch.Default);
            if (es.EntityList.Count > 0)
            {
                T entity = es.FindEntity(namevalue);
                if (entity != null)
                {
                    existEntity= entity;
                    return true;
                }
            }
            existEntity = null;
            return false;
        }

        /// <summary>
        /// 在比较的实体有用到引用属性的地方就必须这样加载。
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="namevalue"></param>
        /// <param name="existEntity"></param>
        /// <param name="fetch"></param>
        /// <returns></returns>
        public static bool Exist<T>(string namevalue, out T existEntity,Fetch fetch) where T : BaseEntity
        {
            EntitySet<T> es = new EntitySet<T>(fetch);
            if (es.EntityList.Count > 0)
            {
                T entity = es.FindEntity(namevalue);
                if (entity != null)
                {
                    existEntity = entity;
                    return true;
                }
            }
            existEntity = null;
            return false;
        }

        /// <summary>
        /// 将新创建的实体插入数据库,如果该实体并不是新建的,则会抛出异常
        /// </summary>        
        public virtual void Insert()
        {
            if (this.Id > 0)
                throw new OOEngineException("对象已经被持久化,不能在此对象上执行新增操作!");
            //try
            //{
            string sqlPattern = "insert into {0}({1}) values({2});select last_insert_rowid() newid;";
                StringBuilder sbFieldNameList = new StringBuilder();
                StringBuilder sbFieldValueList = new StringBuilder();
                //常规属性
                foreach (FieldAttribute pinfo in EInfo.Properties)
                {
                    if (sbFieldNameList.Length > 0)
                        sbFieldNameList.Append(",");
                    sbFieldNameList.Append(pinfo.FieldName);
                    if (sbFieldValueList.Length > 0)
                        sbFieldValueList.Append(",");
                    sbFieldValueList.Append(DLLAnalysis.GetPropertyValue(this, pinfo));
                }
                //引用属性
                foreach (REFAttribute refobj in EInfo.References)
                {
                    if (sbFieldNameList.Length > 0)
                        sbFieldNameList.Append(",");
                    //将实体中的REFEntityName转变为数据库表的字段EntityNameId
                    sbFieldNameList.Append(refobj.REFFieldName);
                    if (sbFieldValueList.Length > 0)
                        sbFieldValueList.Append(",");
                    sbFieldValueList.Append(DLLAnalysis.GetReferenceEntityId(this, refobj.PropertyName));
                }
                if (EInfo.REFSelf)
                {
                    if (sbFieldNameList.Length > 0)
                        sbFieldNameList.Append(",");
                    //将实体中的REFEntityName转变为数据库表的字段EntityNameId
                    sbFieldNameList.Append("ParentId");
                    if (sbFieldValueList.Length > 0)
                        sbFieldValueList.Append(",");
                    sbFieldValueList.Append(DLLAnalysis.GetReferenceEntityId(this,"Parent"+EInfo.EntityName));
                }
                string sql = string.Format(sqlPattern, EInfo.EntityName, sbFieldNameList.ToString(), sbFieldValueList.ToString());
                int newid = DBSQLiteHelper.ExecuteScalar(sql);
                this.Id = Convert.ToInt64(newid);
            //}
            //catch (Exception e)
            //{
            //    throw new OOEngineException("在持久化新实体对象时发生异常,地址:BaseEntity.Insert(),具体信息:"+e.Message);
            //}
        }

        /// <summary>
        /// 执行事务操作时候返回将要执行的SQL语句
        /// </summary>        
        public void Insert(List <string> listsql)
        {
            if (this.Id > 0)
                throw new OOEngineException("对象已经被持久化,不能在此对象上执行新增操作!");
            //try
            //{
            string sqlPattern = "insert into {0}({1}) values({2});select last_insert_rowid() newid;";
            StringBuilder sbFieldNameList = new StringBuilder();
            StringBuilder sbFieldValueList = new StringBuilder();
            //常规属性
            foreach (FieldAttribute pinfo in EInfo.Properties)
            {
                if (sbFieldNameList.Length > 0)
                    sbFieldNameList.Append(",");
                sbFieldNameList.Append(pinfo.FieldName);
                if (sbFieldValueList.Length > 0)
                    sbFieldValueList.Append(",");
                sbFieldValueList.Append(DLLAnalysis.GetPropertyValue(this, pinfo));
            }
            //引用属性
            foreach (REFAttribute refobj in EInfo.References)
            {
                if (sbFieldNameList.Length > 0)
                    sbFieldNameList.Append(",");
                //将实体中的REFEntityName转变为数据库表的字段EntityNameId
                sbFieldNameList.Append(refobj.REFFieldName);
                if (sbFieldValueList.Length > 0)
                    sbFieldValueList.Append(",");
                sbFieldValueList.Append(DLLAnalysis.GetReferenceEntityId(this, refobj.PropertyName));
            }
            if (EInfo.REFSelf)
            {
                if (sbFieldNameList.Length > 0)
                    sbFieldNameList.Append(",");
                //将实体中的REFEntityName转变为数据库表的字段EntityNameId
                sbFieldNameList.Append("ParentId");
                if (sbFieldValueList.Length > 0)
                    sbFieldValueList.Append(",");
                sbFieldValueList.Append(DLLAnalysis.GetReferenceEntityId(this, "Parent" + EInfo.EntityName));
            }
            string sql = string.Format(sqlPattern, EInfo.EntityName, sbFieldNameList.ToString(), sbFieldValueList.ToString());
            if (listsql!=null)
            {
                listsql.Add(sql);
            }
        }

        /// <summary>
        /// 删除已持久化实体对象,如果对一个临时对象进行删除操作,将抛出异常
        /// </summary>
        public virtual void Delete()
        {
            if (this.Id == 0)
                throw new OOEngineException("此对象尚未被持久化,无法执行删除操作!");
            string sqlPattern = "delete from {0} where Id={1};";
            string sql = string.Format(sqlPattern,EInfo.EntityName,this.Id);
            try
            {
                DBSQLiteHelper.ExecuteUpdate(sql);
            }
            catch (Exception e)
            {
                throw new OOEngineException("在删除持久化对象时发生异常!"+e.Message);
            }
        }

        /// <summary>
        /// 执行事务操作时候返回将要执行的SQL语句
        /// </summary>
        public virtual void Delete(List <string> listsql)
        {
            if (this.Id == 0)
                throw new OOEngineException("此对象尚未被持久化,无法执行删除操作!");
            string sqlPattern = "delete from {0} where Id={1};";
            string sql = string.Format(sqlPattern, EInfo.EntityName, this.Id);
            if (listsql !=null )
            {
                listsql.Add(sql);
            }
        }


        public virtual void DeleteAll()
        {
            if (this.Id == 0)
                throw new OOEngineException("此对象尚未被持久化,无法执行删除操作!");
            string sqlPattern = "delete from {0};";
            string sql = string.Format(sqlPattern, EInfo.EntityName);
            try
            {
                DBSQLiteHelper.ExecuteUpdate(sql);
            }
            catch (Exception e)
            {
                throw new OOEngineException("在删除持久化对象时发生异常!" + e.Message);
            }
        }


        /// <summary>
        /// 更新持久化对象的属性
        /// </summary>
        public void Update()
        {
            if (this.Id == 0)
                throw new OOEngineException("此对象尚未被持久化,请先执行新增操作后再执行此方法!");
            string sqlPattern = "update {0} set {1} where Id={2};";
            StringBuilder sbSetValue = new StringBuilder();
            foreach (FieldAttribute pinfo in EInfo.Properties)
            {
                if (sbSetValue.Length > 0)
                    sbSetValue.Append(",");
                sbSetValue.Append(string.Format("{0}={1}",pinfo.FieldName,DLLAnalysis.GetPropertyValue(this,pinfo)));
            }
            foreach (REFAttribute refobj in EInfo.References)
            {
                if (sbSetValue.Length > 0)
                    sbSetValue.Append(",");
                sbSetValue.Append(string.Format("{0}={1}",refobj.REFFieldName,DLLAnalysis.GetReferenceEntityId(this,refobj.PropertyName)));
            }
            if (EInfo.REFSelf)
            {
                if (sbSetValue.Length > 0)
                    sbSetValue.Append(",");
                sbSetValue.Append(string.Format("ParentId={0}", DLLAnalysis.GetReferenceEntityId(this,"Parent"+EInfo.EntityName)));
            }
            string sql = string.Format(sqlPattern,EInfo.EntityName,sbSetValue.ToString(),this.Id);
            DBSQLiteHelper.ExecuteUpdate(sql);
        }

        /// <summary>
        /// 返回即将要执行的SQL语句
        /// </summary>
        public void Update(List <string > listsql)
        {
            if (this.Id == 0)
                throw new OOEngineException("此对象尚未被持久化,请先执行新增操作后再执行此方法!");
            string sqlPattern = "update {0} set {1} where Id={2};";
            StringBuilder sbSetValue = new StringBuilder();
            foreach (FieldAttribute pinfo in EInfo.Properties)
            {
                if (sbSetValue.Length > 0)
                    sbSetValue.Append(",");
                sbSetValue.Append(string.Format("{0}={1}", pinfo.FieldName, DLLAnalysis.GetPropertyValue(this, pinfo)));
            }
            foreach (REFAttribute refobj in EInfo.References)
            {
                if (sbSetValue.Length > 0)
                    sbSetValue.Append(",");
                sbSetValue.Append(string.Format("{0}={1}", refobj.REFFieldName, DLLAnalysis.GetReferenceEntityId(this, refobj.PropertyName)));
            }
            if (EInfo.REFSelf)
            {
                if (sbSetValue.Length > 0)
                    sbSetValue.Append(",");
                sbSetValue.Append(string.Format("ParentId={0}", DLLAnalysis.GetReferenceEntityId(this, "Parent" + EInfo.EntityName)));
            }
            string sql = string.Format(sqlPattern, EInfo.EntityName, sbSetValue.ToString(), this.Id);
            if (listsql !=null )
            {
                listsql.Add(sql);
            }
        }

        /// <summary>
        /// 自动判断实体对象的状态,是临时状态还是已持久化状态,并执行插入或者更新操作
        /// </summary>
        public void InsertOrUpdate()
        {            
            if (this.Id == 0)
                Insert();
            else
                Update();            
        }

        public void InsertOrUpdate(List<string> sql)
        {
            if (this.Id == 0)
                Insert(sql);
            else
                Update(sql);  
        }

        /// <summary>
        /// 根据实体ID采用默认方式载入实体属性Fetch.Deault
        /// </summary>
        /// <param name="id"></param>
        public void Load(long id)
        {
            Load(id,Fetch.Default);
        }

        public void Load(Fetch fetchType)
        {
            if (this.Id>0)
            {
                Load(this.Id, fetchType);
            }
        }

        /// <summary>
        /// 根据ID载入实体
        /// </summary>
        /// <param name="id">实体对象的Id</param>
        /// <param name="fetchType">加载对象的方式,例如是否加载引用对象和集合属性</param>
        public void Load(long id,Fetch fetchType)
        {
            this.Id = id;
            string sqlPattern = string.Empty;
            string sql = string.Empty;
            StringBuilder sbSelectStr = new StringBuilder();
            string strLeftJoin = string.Empty;
            DataSet ds = null;

            sbSelectStr.Append(string.Format("{0}.id as E_{0}Id,{0}.*", EInfo.EntityName));
            sqlPattern = "select {0} from {1} {2} where {1}.id={3}";
            strLeftJoin = DBSQLiteHelper.GenerateLeftJoinSQL(EInfo.References, ref sbSelectStr, fetchType);
            strLeftJoin += DBSQLiteHelper.GenerateSetSQL(EInfo.Sets, ref sbSelectStr, fetchType);
            if (EInfo.REFSelf)
                strLeftJoin += DBSQLiteHelper.GenerateREFSelfSQL(EInfo, ref sbSelectStr, fetchType);
            sql = string.Format(sqlPattern, sbSelectStr.ToString(), EInfo.EntityName, strLeftJoin, id);
            ds = DBSQLiteHelper.ExecuteQuery(sql);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (FieldAttribute pinfo in EInfo.Properties)
                {
                    DBSQLiteHelper.FetchPropertyValue(this, pinfo, ds.Tables[0], 0);
                }
                foreach (REFAttribute refAtt in EInfo.References)
                {
                    if((fetchType== Fetch.Default && !refAtt.LazyLoad) || fetchType== Fetch.REFS || fetchType== Fetch.REFSandSets)
                        DBSQLiteHelper.FetchREFEntity(this, refAtt, ds.Tables[0]);
                }
                foreach (SETAttribute setAtt in EInfo.Sets)
                {
                    if ((fetchType== Fetch.Default && !setAtt.LazyLoad) || fetchType== Fetch.SETS || fetchType== Fetch.REFSandSets)
                        DBSQLiteHelper.FetchSet(this, setAtt, ds.Tables[0]);
                }
                //如果实体有对自身实体对象的引用
                if (EInfo.REFSelf)
                {
                    if (fetchType == Fetch.REFS)
                        DBSQLiteHelper.FetchParent(this, ds.Tables[0], 0);
                    else if (fetchType == Fetch.SETS)
                        DBSQLiteHelper.FetchChildren(this, ds.Tables[0]);
                    else if (fetchType == Fetch.REFSandSets)
                    {
                        DBSQLiteHelper.FetchParent(this, ds.Tables[0], 0);
                        DBSQLiteHelper.FetchChildren(this, ds.Tables[0]);
                    }
                }
            }
        }
    }

    [Serializable]
    public sealed class EntityInfo
    {
        private string _EntityName;
        /// <summary>
        /// 实体名称
        /// </summary>
        public string EntityName
        {
            get { return this._EntityName; }
            set { this._EntityName = value; }
        }

        /// <summary>
        /// 在查询时实体的列名称,如Id-->E_PersonId
        /// </summary>
        public string IdNameInSelect
        {
            get { return "E_"+EntityName+"Id"; }
        }

        private string _EntityDisplayName;
        /// <summary>
        /// 实体的显示名称
        /// </summary>
        public string EntityDisplayName
        {
            get { return this._EntityDisplayName; }
            set { this._EntityDisplayName = value; }
        }

        private List<FieldAttribute> _Properties;
        /// <summary>
        /// 实体的常规属性,只包括数字,字符串,日期
        /// </summary>
        public List<FieldAttribute> Properties
        {
            get { return this._Properties; }
            set { this._Properties = value; }
        }

        private List<REFAttribute> _References;
        /// <summary>
        /// 实体引用的对象属性,以REF开头
        /// </summary>
        public List<REFAttribute> References
        {
            get { return this._References; }
            set { this._References = value; }
        }

        private List<SETAttribute> _Sets;
        /// <summary>
        /// 实体的集合属性,以SET开头
        /// </summary>
        public List<SETAttribute> Sets
        {
            get { return this._Sets; }
            set { this._Sets = value; }
        }

        private bool _REFSelf;
        /// <summary>
        /// 是否有引用自生的属性
        /// </summary>
        public bool REFSelf
        {
            get { return _REFSelf; }
            set { this._REFSelf = value; }
        }

        //private REFSelfAttribute _REFSelf;
        ///// <summary>
        ///// 引用自己的属性
        ///// </summary>
        //public REFSelfAttribute REFSelf
        //{
        //    get { return this._REFSelf; }
        //    set { this._REFSelf = value; }
        //}

        //private SETSelfAttribute _SETSelf;
        ///// <summary>
        ///// 针对引用自己实体类型的集合引用
        ///// </summary>
        //public SETSelfAttribute SETSelf
        //{
        //    get { return this._SETSelf; }
        //    set { this._SETSelf = value; }
        //}
    }
}
