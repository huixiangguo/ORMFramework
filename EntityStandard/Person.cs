using System;
using System.Collections.Generic;
using System.Text;

namespace Entity
{
    [Entity("用户")]
    [Serializable]
    public class Person:BaseEntity
    {
        private string _account;
        [Field("登录名", "string(50)", true, "", false)]
        public string Account
        {
            get { return _account; }
            set { _account = value; }
        }

        private string _name;
        [Field("姓名", "string(50)", true, "", false)]
        public string DisplayName
        {
            get { return _name; }
            set { _name = value; }
        }

        private string _password;
        [Field("密码", "string(50)", true, "", false)]
        public string Password
        {
            get { return _password; }
            set { _password = value; }
        }

        //certificate
        private string certificate;
        [Field("证书", "string(50)", true, "", false)]
        public string Certificate
        {
            get { return certificate; }
            set { certificate = value; }
        }

        /// <summary>
        /// 管理员 设计人员 空为客户
        /// </summary>
        private string _role;
        [Field("角色", "string(50)", true, "", false)]
        public string Role
        {
            get { return _role; }
            set { _role = value; }
        }

        public override string ToString()
        {
            return this.Account;
        }

    }
}
