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
        /// 获得所有实体全名称,如Entity.YourEntity
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
        /// 获得实体的单独名称,如YourEntity
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
        /// 根据实体的单独名称得到全名,如Person-->Entity.Membership.Person
        /// </summary>
        /// <param name="soleName">单独名称Person</param>
        /// <returns>全名Entity.Membership.Person</returns>
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
        /// 根据单独名称创建实体实例
        /// </summary>
        /// <param name="soleName">单独名称Person</param>
        /// <returns>Person的实例</returns>
        public static object GetEntityInstance(string soleName)
        {
            return Assembly.Load("EntityStandard").CreateInstance(GetFullNameBySoleName(soleName));
                                  
        }

        /// <summary>
        /// 获得实体的包名加实体名,如Membership.UserEntity
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
        /// 根据实体的Type获得实体的类媒数据信息
        /// </summary>
        /// <param name="type">实体类的Type</param>
        /// <returns>实体的媒数据信息</returns>
        public static EntityInfo GetEntityInfoByType(Type type)
        {
            if (!type.IsSubclassOf(typeof(BaseEntity)))
                return null;
            EntityInfo ei = new EntityInfo();
            ei.EntityName = type.Name;//获得实体的名称
            object[] atts = type.GetCustomAttributes(typeof(EntityAttribute), false);
            if (atts != null && atts.Length > 0)
            {
                ei.EntityDisplayName = ((EntityAttribute)atts[0]).EntityDisplayName;//获得实体的显示名称
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
                    //如果有引用自生的属性，则将REFSelf置为true;
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
            //属性数据类型为字符串,string和text以及ntext,Date,SmallDate
            if (fieldAtt.DataType.StartsWith("string") || fieldAtt.DataType.IndexOf("text") > -1 || fieldAtt.DataType.IndexOf("Date") > -1)
            {
                if (!string.IsNullOrEmpty(propValue))
                {
                       strResult = string.Format("'{0}'", propValue.Replace("'", "''")); //防止SQL注入
                }
                else if (fieldAtt.AllowNull) //该属性允许为空值
                    strResult = "null";
                else if (!fieldAtt.AllowNull) //当不允许为空值的时候,默认插入空字符串
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
        /// 在insert()实体时获取引用实体的ID,如果引用实体没有保存,则先保存
        /// </summary>
        /// <param name="thisobj">宿主实体对象</param>
        /// <param name="refinfo">引用属性名称</param>
        /// <returns>被引用实体的ID,如果该属性为空,则返回null</returns>
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
