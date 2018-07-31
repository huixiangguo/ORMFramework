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
        /// �ֶ���ʾ����������,��Name-->����
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        private string _FieldName;
        /// <summary>
        /// ��������
        /// </summary>
        public string FieldName
        {
            get { return this._FieldName; }
            set { this._FieldName = value; }
        }

        private string _DataType;
        /// <summary>
        /// �ֶε���������,����Ϊ:string(1-255)|text|int|long|float|decimal|Date|SmallDate
        /// </summary>
        public string DataType
        {
            get { return this._DataType; }
            set { this._DataType = value; }
        }

        private bool _AllowNull;
        /// <summary>
        /// �ֶ��Ƿ����Ϊ��
        /// </summary>
        public bool AllowNull
        {
            get { return this._AllowNull; }
            set { this._AllowNull = value; }
        }

        private string _DefaultValue;
        /// <summary>
        /// �ֶ�Ĭ��ֵ,����Ϊ:"value"|string.EmptyString
        /// �����ֵ����Ϊ���ַ���,�򲻻�ָ��Ĭ��ֵ
        /// </summary>
        public string DefaultValue
        {
            get { return this._DefaultValue; }
            set { this._DefaultValue = value; }
        }

        private bool _Unique;
        /// <summary>
        /// ������ֵ�Ƿ���Ψһ
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
        /// �˶��������������ڵ�ʵ������
        /// </summary>
        public string EntityName
        {
            get { return this._EntityName; }
            set { this._EntityName = value; }
        }

        private string _DisplayName;
        /// <summary>
        /// �ֶ���ʾ����������,��Name-->����
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        private string _PropertyName;
        /// <summary>
        /// ʵ�����Ե�����
        /// </summary>
        public string PropertyName
        {
            get { return this._PropertyName; }
            set { this._PropertyName = value; }
        }

        private string _REFEntityName;
        /// <summary>
        /// ���������ö��������
        /// </summary>
        public string REFEntityName
        {
            get { return this._REFEntityName; }
            set { this._REFEntityName = value; }
        }

        private string _REFFieldName;
        /// <summary>
        /// �����Զ�Ӧ�����ݿ�����������ֶ�����,��DepartmentId
        /// </summary>
        public string REFFieldName
        {
            get { return this._REFFieldName; }
            set { this._REFFieldName = value; }
        }

        private bool _LazyLoad;
        /// <summary>
        /// �����������Ƿ�Ϊ������,���Ϊfalse,����FetchΪDefaultʱ,��������entityʱ�Զ����ش���������
        /// </summary>
        public bool LazyLoad
        {
            get { return this._LazyLoad; }
            set { this._LazyLoad = value; }
        }

        private bool _AllowNull;
        /// <summary>
        /// �ֶ��Ƿ����Ϊ��
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
        /// �˼��������������ڵ�ʵ������
        /// </summary>
        public string EntityName
        {
            get { return this._EntityName; }
            set { this._EntityName = value; }
        }

        private string _DisplayName;
        /// <summary>
        /// �ֶ���ʾ����������,��Name-->����
        /// </summary>
        public string DisplayName
        {
            get { return this._DisplayName; }
            set { this._DisplayName = value; }
        }

        private string _PropertyName;
        /// <summary>
        /// ʵ�����Ե�����
        /// </summary>
        public string PropertyName
        {
            get { return this._PropertyName; }
            set { this._PropertyName = value; }
        }

        private string _ElementEntityName;
        /// <summary>
        /// SET�����д��Ԫ��ʵ������
        /// </summary>
        public string ElementEntityName
        {
            get { return this._ElementEntityName; }
            set { this._ElementEntityName = value; }
        }

        private string _MidTableName;
        /// <summary>
        /// �Զ����ɵ��м�������
        /// </summary>
        public string MidTableName
        {
            get { return this._MidTableName; }
            set { this._MidTableName = value; }
        }

        private string _OwnerFieldName;
        /// <summary>
        /// SET��������ʵ�巽���ֶ�����,����Person�и�SETCourse�ļ���,����ֶξ�ΪPersonId
        /// </summary>
        public string OwnerFieldName
        {
            get { return this._OwnerFieldName; }
            set { this._OwnerFieldName = value; }
        }

        private string _ElementEntityId;
        /// <summary>
        /// SET�����д��Ԫ�ص�ʵ��ID,������м��,��ΪCourseId,��������м��,��ΪId
        /// </summary>
        public string ElementEntityId
        {
            get { return this._ElementEntityId; }
            set { this._ElementEntityId = value; }
        }

        private bool _LazyLoad;
        /// <summary>
        /// �����������Ƿ�Ϊ������,���Ϊfalse,����FetchΪDefaultʱ,��������entityʱ�Զ����ش���������
        /// </summary>
        public bool LazyLoad
        {
            get { return this._LazyLoad; }
            set { this._LazyLoad = value; }
        }

        private bool _IsMidTable;
        /// <summary>
        /// ��ʶ�ü��������Ƿ�ͨ���м��
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
