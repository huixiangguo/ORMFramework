using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    /// <summary>
    /// ������ҳ���б�ʶ��ǰҳ����������
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    [System.Serializable()]    
    public class PageAttribute:Attribute
    {
        private string _PageTitle;
        public string PageTitle
        {
            get { return this._PageTitle; }
            set { this._PageTitle = value; }
        }

        public PageAttribute(string pageTitle)
        {
            this.PageTitle = pageTitle;
        }
    }
}
