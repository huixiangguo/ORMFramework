using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    public class OOEngineException : Exception
    {
        public OOEngineException():base()
        {           
        }
        public OOEngineException(string message):base(message)
        {
        }
    }
}
