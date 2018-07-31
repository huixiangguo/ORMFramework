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
        /// ������ʾ����
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
        ///  �Ƚϵ�ʱ����Ҫ�õ�����ʵ������
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
        /// ����ʵ�岢�һ��ش��ڵ�ʵ��
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
        /// �ڱȽϵ�ʵ�����õ��������Եĵط��ͱ����������ء�
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
        /// ���´�����ʵ��������ݿ�,�����ʵ�岢�����½���,����׳��쳣
        /// </summary>        
        public virtual void Insert()
        {
            if (this.Id > 0)
                throw new OOEngineException("�����Ѿ����־û�,�����ڴ˶�����ִ����������!");
            //try
            //{
            string sqlPattern = "insert into {0}({1}) values({2});select last_insert_rowid() newid;";
                StringBuilder sbFieldNameList = new StringBuilder();
                StringBuilder sbFieldValueList = new StringBuilder();
                //��������
                foreach (FieldAttribute pinfo in EInfo.Properties)
                {
                    if (sbFieldNameList.Length > 0)
                        sbFieldNameList.Append(",");
                    sbFieldNameList.Append(pinfo.FieldName);
                    if (sbFieldValueList.Length > 0)
                        sbFieldValueList.Append(",");
                    sbFieldValueList.Append(DLLAnalysis.GetPropertyValue(this, pinfo));
                }
                //��������
                foreach (REFAttribute refobj in EInfo.References)
                {
                    if (sbFieldNameList.Length > 0)
                        sbFieldNameList.Append(",");
                    //��ʵ���е�REFEntityNameת��Ϊ���ݿ����ֶ�EntityNameId
                    sbFieldNameList.Append(refobj.REFFieldName);
                    if (sbFieldValueList.Length > 0)
                        sbFieldValueList.Append(",");
                    sbFieldValueList.Append(DLLAnalysis.GetReferenceEntityId(this, refobj.PropertyName));
                }
                if (EInfo.REFSelf)
                {
                    if (sbFieldNameList.Length > 0)
                        sbFieldNameList.Append(",");
                    //��ʵ���е�REFEntityNameת��Ϊ���ݿ����ֶ�EntityNameId
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
            //    throw new OOEngineException("�ڳ־û���ʵ�����ʱ�����쳣,��ַ:BaseEntity.Insert(),������Ϣ:"+e.Message);
            //}
        }

        /// <summary>
        /// ִ���������ʱ�򷵻ؽ�Ҫִ�е�SQL���
        /// </summary>        
        public void Insert(List <string> listsql)
        {
            if (this.Id > 0)
                throw new OOEngineException("�����Ѿ����־û�,�����ڴ˶�����ִ����������!");
            //try
            //{
            string sqlPattern = "insert into {0}({1}) values({2});select last_insert_rowid() newid;";
            StringBuilder sbFieldNameList = new StringBuilder();
            StringBuilder sbFieldValueList = new StringBuilder();
            //��������
            foreach (FieldAttribute pinfo in EInfo.Properties)
            {
                if (sbFieldNameList.Length > 0)
                    sbFieldNameList.Append(",");
                sbFieldNameList.Append(pinfo.FieldName);
                if (sbFieldValueList.Length > 0)
                    sbFieldValueList.Append(",");
                sbFieldValueList.Append(DLLAnalysis.GetPropertyValue(this, pinfo));
            }
            //��������
            foreach (REFAttribute refobj in EInfo.References)
            {
                if (sbFieldNameList.Length > 0)
                    sbFieldNameList.Append(",");
                //��ʵ���е�REFEntityNameת��Ϊ���ݿ����ֶ�EntityNameId
                sbFieldNameList.Append(refobj.REFFieldName);
                if (sbFieldValueList.Length > 0)
                    sbFieldValueList.Append(",");
                sbFieldValueList.Append(DLLAnalysis.GetReferenceEntityId(this, refobj.PropertyName));
            }
            if (EInfo.REFSelf)
            {
                if (sbFieldNameList.Length > 0)
                    sbFieldNameList.Append(",");
                //��ʵ���е�REFEntityNameת��Ϊ���ݿ����ֶ�EntityNameId
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
        /// ɾ���ѳ־û�ʵ�����,�����һ����ʱ�������ɾ������,���׳��쳣
        /// </summary>
        public virtual void Delete()
        {
            if (this.Id == 0)
                throw new OOEngineException("�˶�����δ���־û�,�޷�ִ��ɾ������!");
            string sqlPattern = "delete from {0} where Id={1};";
            string sql = string.Format(sqlPattern,EInfo.EntityName,this.Id);
            try
            {
                DBSQLiteHelper.ExecuteUpdate(sql);
            }
            catch (Exception e)
            {
                throw new OOEngineException("��ɾ���־û�����ʱ�����쳣!"+e.Message);
            }
        }

        /// <summary>
        /// ִ���������ʱ�򷵻ؽ�Ҫִ�е�SQL���
        /// </summary>
        public virtual void Delete(List <string> listsql)
        {
            if (this.Id == 0)
                throw new OOEngineException("�˶�����δ���־û�,�޷�ִ��ɾ������!");
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
                throw new OOEngineException("�˶�����δ���־û�,�޷�ִ��ɾ������!");
            string sqlPattern = "delete from {0};";
            string sql = string.Format(sqlPattern, EInfo.EntityName);
            try
            {
                DBSQLiteHelper.ExecuteUpdate(sql);
            }
            catch (Exception e)
            {
                throw new OOEngineException("��ɾ���־û�����ʱ�����쳣!" + e.Message);
            }
        }


        /// <summary>
        /// ���³־û����������
        /// </summary>
        public void Update()
        {
            if (this.Id == 0)
                throw new OOEngineException("�˶�����δ���־û�,����ִ��������������ִ�д˷���!");
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
        /// ���ؼ���Ҫִ�е�SQL���
        /// </summary>
        public void Update(List <string > listsql)
        {
            if (this.Id == 0)
                throw new OOEngineException("�˶�����δ���־û�,����ִ��������������ִ�д˷���!");
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
        /// �Զ��ж�ʵ������״̬,����ʱ״̬�����ѳ־û�״̬,��ִ�в�����߸��²���
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
        /// ����ʵ��ID����Ĭ�Ϸ�ʽ����ʵ������Fetch.Deault
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
        /// ����ID����ʵ��
        /// </summary>
        /// <param name="id">ʵ������Id</param>
        /// <param name="fetchType">���ض���ķ�ʽ,�����Ƿ�������ö���ͼ�������</param>
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
                //���ʵ���ж�����ʵ����������
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
        /// ʵ������
        /// </summary>
        public string EntityName
        {
            get { return this._EntityName; }
            set { this._EntityName = value; }
        }

        /// <summary>
        /// �ڲ�ѯʱʵ���������,��Id-->E_PersonId
        /// </summary>
        public string IdNameInSelect
        {
            get { return "E_"+EntityName+"Id"; }
        }

        private string _EntityDisplayName;
        /// <summary>
        /// ʵ�����ʾ����
        /// </summary>
        public string EntityDisplayName
        {
            get { return this._EntityDisplayName; }
            set { this._EntityDisplayName = value; }
        }

        private List<FieldAttribute> _Properties;
        /// <summary>
        /// ʵ��ĳ�������,ֻ��������,�ַ���,����
        /// </summary>
        public List<FieldAttribute> Properties
        {
            get { return this._Properties; }
            set { this._Properties = value; }
        }

        private List<REFAttribute> _References;
        /// <summary>
        /// ʵ�����õĶ�������,��REF��ͷ
        /// </summary>
        public List<REFAttribute> References
        {
            get { return this._References; }
            set { this._References = value; }
        }

        private List<SETAttribute> _Sets;
        /// <summary>
        /// ʵ��ļ�������,��SET��ͷ
        /// </summary>
        public List<SETAttribute> Sets
        {
            get { return this._Sets; }
            set { this._Sets = value; }
        }

        private bool _REFSelf;
        /// <summary>
        /// �Ƿ�����������������
        /// </summary>
        public bool REFSelf
        {
            get { return _REFSelf; }
            set { this._REFSelf = value; }
        }

        //private REFSelfAttribute _REFSelf;
        ///// <summary>
        ///// �����Լ�������
        ///// </summary>
        //public REFSelfAttribute REFSelf
        //{
        //    get { return this._REFSelf; }
        //    set { this._REFSelf = value; }
        //}

        //private SETSelfAttribute _SETSelf;
        ///// <summary>
        ///// ��������Լ�ʵ�����͵ļ�������
        ///// </summary>
        //public SETSelfAttribute SETSelf
        //{
        //    get { return this._SETSelf; }
        //    set { this._SETSelf = value; }
        //}
    }
}
