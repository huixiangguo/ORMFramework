using System;
using System.Collections.Generic;
using System.Text;

namespace Entity.API
{
    /// <summary>
    /// 修改部分字段提交, ORM 性能优化 部分字段更新，部分字段查询（暂未实现）
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class QueryDB<T> where T:BaseEntity
    {
        private string sqlPattern = "update {0} set {1} where {2}={3};";
        private string whereColumn;
        private string whereValue;
        private StringBuilder sbSetValue = new StringBuilder();
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

        public QueryDB<T> Update()
        {
            return this;
        }

        public QueryDB<T> Column(string column,string value)
        {
            if (sbSetValue.Length > 0)
                sbSetValue.Append(",");
            sbSetValue.Append(string.Format("{0}='{1}'", column,value));
            return this;
        }

        public QueryDB<T> Where(string column, string value)
        {
            this.whereColumn = column;
            this.whereValue = value;
            return this;
        }

        public int Execute()
        {
            string sql = string.Format(sqlPattern, EInfo.EntityName, sbSetValue.ToString(), this.whereColumn,this.whereValue);
            return DBSQLiteHelper.ExecuteUpdate(sql);
        }
    }
}
