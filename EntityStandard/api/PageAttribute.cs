using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    /// <summary>
    /// 用来在页面中标识当前页面标题的特性
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
