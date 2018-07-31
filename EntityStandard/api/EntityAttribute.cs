using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    [AttributeUsage(AttributeTargets.Class)]
    [Serializable()]
    public class EntityAttribute:Attribute
    {
        private string _EntityDisplayName;
        public string EntityDisplayName
        {
            get { return this._EntityDisplayName; }
            set { this._EntityDisplayName = value; }
        }

        public EntityAttribute(string displayName)
        {
            this.EntityDisplayName = displayName;
        }
    }
}
