using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    [AttributeUsage(AttributeTargets.Property)]
    [System.Serializable()]
    public class FieldAttribute:Attribute
    {
        private string _DisplayName;
        /// <summary>
        /// 字段显示出来的名称,如Name-->名称
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        private string _FieldName;
        /// <summary>
        /// 属性名称
        /// </summary>
        public string FieldName
        {
            get { return this._FieldName; }
            set { this._FieldName = value; }
        }

        private string _DataType;
        /// <summary>
        /// 字段的数据类型,可以为:string(1-255)|text|int|long|float|decimal|Date|SmallDate
        /// </summary>
        public string DataType
        {
            get { return this._DataType; }
            set { this._DataType = value; }
        }

        private bool _AllowNull;
        /// <summary>
        /// 字段是否可以为空
        /// </summary>
        public bool AllowNull
        {
            get { return this._AllowNull; }
            set { this._AllowNull = value; }
        }

        private string _DefaultValue;
        /// <summary>
        /// 字段默认值,可以为:"value"|string.EmptyString
        /// 如果该值设置为空字符串,则不会指定默认值
        /// </summary>
        public string DefaultValue
        {
            get { return this._DefaultValue; }
            set { this._DefaultValue = value; }
        }

        private bool _Unique;
        /// <summary>
        /// 该属性值是否是唯一
        /// </summary>
        public bool Unique
        {
            get { return this._Unique; }
            set { this._Unique = value; }
        }

        public FieldAttribute(string displayName,string dataType,bool allowNull,string defaultValue,bool unique)
        {
            this.DisplayName = displayName;
            this.DataType = dataType;
            this.AllowNull = allowNull;
            this.DefaultValue = defaultValue;
            this.Unique = unique;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    [System.Serializable()]
    public class REFAttribute : Attribute
    {
        private string _EntityName;
        /// <summary>
        /// 此对象引用特性所在的实体名称
        /// </summary>
        public string EntityName
        {
            get { return this._EntityName; }
            set { this._EntityName = value; }
        }

        private string _DisplayName;
        /// <summary>
        /// 字段显示出来的名称,如Name-->名称
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        private string _PropertyName;
        /// <summary>
        /// 实体属性的名称
        /// </summary>
        public string PropertyName
        {
            get { return this._PropertyName; }
            set { this._PropertyName = value; }
        }

        private string _REFEntityName;
        /// <summary>
        /// 该属性引用对象的名称
        /// </summary>
        public string REFEntityName
        {
            get { return this._REFEntityName; }
            set { this._REFEntityName = value; }
        }

        private string _REFFieldName;
        /// <summary>
        /// 该属性对应的数据库表的外键引用字段名称,如DepartmentId
        /// </summary>
        public string REFFieldName
        {
            get { return this._REFFieldName; }
            set { this._REFFieldName = value; }
        }

        private bool _LazyLoad;
        /// <summary>
        /// 该引用属性是否为懒载入,如果为false,而且Fetch为Default时,将在载入entity时自动加载此引用属性
        /// </summary>
        public bool LazyLoad
        {
            get { return this._LazyLoad; }
            set { this._LazyLoad = value; }
        }

        private bool _AllowNull;
        /// <summary>
        /// 字段是否可以为空
        /// </summary>
        public bool AllowNull
        {
            get { return this._AllowNull; }
            set { this._AllowNull = value; }
        }

        public REFAttribute(string displayName, string refEntityName, bool lazyLoad,bool allowNull)
        {
            this.DisplayName = displayName;
            this.REFEntityName = refEntityName;
            this.LazyLoad = lazyLoad;
            this.AllowNull = allowNull;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    [System.Serializable()]
    public class SETAttribute : Attribute
    {
        private string _EntityName;
        /// <summary>
        /// 此集合引用特性所在的实体名称
        /// </summary>
        public string EntityName
        {
            get { return this._EntityName; }
            set { this._EntityName = value; }
        }

        private string _DisplayName;
        /// <summary>
        /// 字段显示出来的名称,如Name-->名称
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        private string _PropertyName;
        /// <summary>
        /// 实体属性的名称
        /// </summary>
        public string PropertyName
        {
            get { return this._PropertyName; }
            set { this._PropertyName = value; }
        }

        private string _ElementEntityName;
        /// <summary>
        /// SET集合中存放元素实体名称
        /// </summary>
        public string ElementEntityName
        {
            get { return this._ElementEntityName; }
            set { this._ElementEntityName = value; }
        }

        private string _MidTableName;
        /// <summary>
        /// 自动生成的中间表的名称
        /// </summary>
        public string MidTableName
        {
            get { return this._MidTableName; }
            set { this._MidTableName = value; }
        }

        private string _OwnerFieldName;
        /// <summary>
        /// SET集合所处实体方的字段名称,例如Person有个SETCourse的集合,这个字段就为PersonId
        /// </summary>
        public string OwnerFieldName
        {
            get { return this._OwnerFieldName; }
            set { this._OwnerFieldName = value; }
        }

        private string _ElementEntityId;
        /// <summary>
        /// SET集合中存放元素的实体ID,如果是中间表,则为CourseId,如果不是中间表,则为Id
        /// </summary>
        public string ElementEntityId
        {
            get { return this._ElementEntityId; }
            set { this._ElementEntityId = value; }
        }

        private bool _LazyLoad;
        /// <summary>
        /// 该引用属性是否为懒载入,如果为false,而且Fetch为Default时,将在载入entity时自动加载此引用属性
        /// </summary>
        public bool LazyLoad
        {
            get { return this._LazyLoad; }
            set { this._LazyLoad = value; }
        }

        private bool _IsMidTable;
        /// <summary>
        /// 标识该集合引用是否通过中间表
        /// </summary>
        public bool IsMidTable
        {
            get { return this._IsMidTable; }
            set { this._IsMidTable = value; }
        }

        public SETAttribute(string displayName,string elementEntityName, bool lazyLoad, bool isMidTable)
        {
            this.DisplayName = displayName;
            this.ElementEntityName = elementEntityName;
            this.LazyLoad = lazyLoad;
            this.IsMidTable = isMidTable;
        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    [System.Serializable()]
    public class REFSelfAttribute : Attribute
    {
        public REFSelfAttribute()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Property)]
    [System.Serializable()]
    public class SETSelfAttribute : Attribute
    {
        public SETSelfAttribute()
        {

        }
    }
}
