using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using Entity;

namespace Entity
{
    public class DLLAnalysis
    {
        /// <summary>
        /// �������ʵ��ȫ����,��Entity.YourEntity
        /// </summary>
        /// <returns></returns>
        public static List<string> GetFullNameList()
        {
            Type[] types = Assembly.Load("EntityStandard").GetTypes();
            List<string> list = new List<string>();            
            foreach (Type t in types)
            {
                if(t.BaseType == typeof(BaseEntity))
                    list.Add(t.FullName);
            }
            return list;
        }

        /// <summary>
        /// ���ʵ��ĵ�������,��YourEntity
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEntitySoleNameList()
        {
            Type[] types = Assembly.Load("EntityStandard").GetTypes();
            List<string> list = new List<string>();            
            foreach (Type t in types)
            {
                if (t.BaseType == typeof(BaseEntity))
                    list.Add(t.Name);
            }
            return list;
        }

        /// <summary>
        /// ����ʵ��ĵ������Ƶõ�ȫ��,��Person-->Entity.Membership.Person
        /// </summary>
        /// <param name="soleName">��������Person</param>
        /// <returns>ȫ��Entity.Membership.Person</returns>
        public static string GetFullNameBySoleName(string soleName)
        {
            string fullName = string.Empty;
            Type[] types = Assembly.Load("EntityStandard").GetTypes();
            foreach (Type t in types)
            {
                if (t.Name.Equals(soleName))
                {
                    fullName = t.FullName;
                    break;
                }
            }
            return fullName;
        }

        /// <summary>
        /// ���ݵ������ƴ���ʵ��ʵ��
        /// </summary>
        /// <param name="soleName">��������Person</param>
        /// <returns>Person��ʵ��</returns>
        public static object GetEntityInstance(string soleName)
        {
            return Assembly.Load("EntityStandard").CreateInstance(GetFullNameBySoleName(soleName));
                                  
        }

        /// <summary>
        /// ���ʵ��İ�����ʵ����,��Membership.UserEntity
        /// </summary>
        /// <returns></returns>
        public static List<string> GetEntityPackageNameList()
        {
            List<string> list0 = GetFullNameList();
            List<string> list = new List<string>();
            foreach (string s in list0)
            {
                string str = s.Substring(s.IndexOf(".") + 1);
                list.Add(str);
            }
            return list;
        }

        /// <summary>
        /// ����ʵ���Type���ʵ�����ý������Ϣ
        /// </summary>
        /// <param name="type">ʵ�����Type</param>
        /// <returns>ʵ���ý������Ϣ</returns>
        public static EntityInfo GetEntityInfoByType(Type type)
        {
            if (!type.IsSubclassOf(typeof(BaseEntity)))
                return null;
            EntityInfo ei = new EntityInfo();
            ei.EntityName = type.Name;//���ʵ�������
            object[] atts = type.GetCustomAttributes(typeof(EntityAttribute), false);
            if (atts != null && atts.Length > 0)
            {
                ei.EntityDisplayName = ((EntityAttribute)atts[0]).EntityDisplayName;//���ʵ�����ʾ����
            }
            PropertyInfo[] properties = type.GetProperties();
            List<FieldAttribute> pros = new List<FieldAttribute>();
            List<REFAttribute> refs = new List<REFAttribute>();
            List<SETAttribute> sets = new List<SETAttribute>();
            foreach (PropertyInfo pi in properties)
            {
                object[] patts = pi.GetCustomAttributes(false);
                if (patts != null && patts.Length > 0)
                {
                    object objAtt = patts[0];
                    if (objAtt is FieldAttribute)
                    {
                        ((FieldAttribute)objAtt).FieldName = pi.Name;
                        pros.Add((FieldAttribute)objAtt);
                    }
                    else if (objAtt is REFAttribute)
                    {
                        REFAttribute refAtt = (REFAttribute)objAtt;
                        refAtt.EntityName = ei.EntityName;
                        refAtt.PropertyName = pi.Name;
                        refAtt.REFFieldName = refAtt.REFEntityName + "Id";
                        refs.Add(refAtt);
                    }
                    else if (objAtt is SETAttribute)
                    {
                        SETAttribute setAtt = (SETAttribute)objAtt;
                        setAtt.EntityName = ei.EntityName;
                        setAtt.PropertyName = pi.Name;
                        if (setAtt.IsMidTable)
                        {
                            setAtt.MidTableName = type.Name + "_" + setAtt.ElementEntityName;
                            setAtt.OwnerFieldName = type.Name + "Id";
                            setAtt.ElementEntityId = setAtt.ElementEntityName + "Id";
                        }
                        else
                        {
                            setAtt.ElementEntityId = ei.EntityName + "Id";
                        }
                        sets.Add(setAtt);
                    }
                    //������������������ԣ���REFSelf��Ϊtrue;
                    else if (objAtt is REFSelfAttribute)
                    {
                        ei.REFSelf = true;
                    }
                }
            }
            ei.Properties = pros;
            ei.References = refs;
            ei.Sets = sets;
            return ei;
        }

        public static List<EntityInfo> GetEntityInfoList()
        {
            List<EntityInfo> list = new List<EntityInfo>();
            Type[] types = Assembly.Load("EntityStandard").GetTypes();
            foreach (Type t in types)
            {               
                EntityInfo einfo = GetEntityInfoByType(t);
                if (einfo != null)
                    list.Add(einfo);   
            }
            return list;
        }



        public static string GetPropertyValue(object thisobj, FieldAttribute fieldAtt)
        {
            string strResult = string.Empty;
            object obj = thisobj.GetType().GetProperty(fieldAtt.FieldName).GetValue(thisobj, null);
            string propValue = null;
            if (fieldAtt.DataType.IndexOf("Date") > -1)
            {
                 propValue = obj != null ? ((DateTime)obj).ToString("s") : string.Empty;
            }
            else
            {
                 propValue = obj != null ? obj.ToString() : string.Empty;
            }
            //������������Ϊ�ַ���,string��text�Լ�ntext,Date,SmallDate
            if (fieldAtt.DataType.StartsWith("string") || fieldAtt.DataType.IndexOf("text") > -1 || fieldAtt.DataType.IndexOf("Date") > -1)
            {
                if (!string.IsNullOrEmpty(propValue))
                {
                       strResult = string.Format("'{0}'", propValue.Replace("'", "''")); //��ֹSQLע��
                }
                else if (fieldAtt.AllowNull) //����������Ϊ��ֵ
                    strResult = "null";
                else if (!fieldAtt.AllowNull) //��������Ϊ��ֵ��ʱ��,Ĭ�ϲ�����ַ���
                    strResult = "''";
            }
            else
            {               
                if (!string.IsNullOrEmpty(propValue))
                    strResult = propValue;
                else if (fieldAtt.AllowNull)
                    strResult = "null";
                else if(!fieldAtt.AllowNull)
                    strResult = "0";
            }
            return strResult;
        }

        /// <summary>
        /// ��insert()ʵ��ʱ��ȡ����ʵ���ID,�������ʵ��û�б���,���ȱ���
        /// </summary>
        /// <param name="thisobj">����ʵ�����</param>
        /// <param name="refinfo">������������</param>
        /// <returns>������ʵ���ID,���������Ϊ��,�򷵻�null</returns>
        public static string GetReferenceEntityId(object thisobj, string refinfo)
        {
            string result = string.Empty;
            BaseEntity refEntity = (BaseEntity)thisobj.GetType().GetProperty(refinfo).GetValue(thisobj, null);
            if (refEntity == null)
                result = "null";
            else
            {
                if (refEntity.Id == 0)
                    refEntity.Insert();
                result = refEntity.Id.ToString();
            }
            return result;
        }
    }   
}
