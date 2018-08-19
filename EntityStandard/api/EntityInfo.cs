using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
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
            get { return "E_" + EntityName + "Id"; }
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
