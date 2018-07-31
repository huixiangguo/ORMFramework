using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using Entity;
using System.Threading;


namespace EntityORM
{
    public partial class StartForm : Form
    {
        public StartForm()
        {
            InitializeComponent();
        }

        private string DbFilePath = string.Empty;

        private void StartForm_Load(object sender, EventArgs e)
        {
            TextBox.CheckForIllegalCrossThreadCalls = false;
            EntityListBind();
            this.txtDataName.Text = ORMBuilder.GetConfigAppSetValueByKey("connectionString");
            //DbFilePath = ORMBuilder.GetConfigAppSetValueByKey("dbFilePath");
            DbFilePath = AppDomain.CurrentDomain.BaseDirectory;
        }

        private void EntityListBind()
        {
            List<EntityInfo> list = DLLAnalysis.GetEntityInfoList();
            foreach (EntityInfo einfo in list)
            {
                CheckBox cbox = new CheckBox();
                cbox.Text = einfo.EntityDisplayName;
                cbox.Name = einfo.EntityName;
                pnlChk.Controls.Add(cbox);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtDataName.Text))
            {
                MessageBox.Show("���ݿ����Ʋ���Ϊ��");
                return;
            }
            if (DbFilePath.Equals("."))
            {
                DbFilePath = AppDomain.CurrentDomain.BaseDirectory;
            }
            BuildTable(txtDataName.Text, DbFilePath);
            ORMBuilder.SetValue("connectionString", txtDataName.Text);
        }

        private void BuildTable(string dataName, string DbfilePath)
        {
            //List<EntityInfo> list = DLLAnalysis.GetEntityInfoList();
            StringBuilder sb = null;
            foreach (Control ctrl in pnlChk.Controls)
            {
                if (!((CheckBox)ctrl).Checked)
                    continue;
                EntityInfo einfo = DLLAnalysis.GetEntityInfoByType(DLLAnalysis.GetEntityInstance(((CheckBox)ctrl).Name).GetType());
                sb = new StringBuilder();
                sb.Append("Begin Transaction;");
                sb.Append(ORMBuilder.DropTableIfExists(einfo));
                sb.Append("\r\n");
                sb.Append(ORMBuilder.GenerateTableByEntityInfo(einfo));
                sb.Append("\r\n");
                sb.Append("Commit Transaction;");
                ORMBuilder.AddSQLiteTable(dataName, DbfilePath, sb.ToString());
                sb.Append(string.Format("---------------------->>����ʵ��{0}���ݱ�ɹ�!", einfo.EntityName));
                sb.Append("\r\n");
                sb.Append("\r\n");
                textBox1.Text += sb.ToString();
            }
            string initData = ORMBuilder.LoadInitialData(pnlChk);
            if (string.IsNullOrEmpty(initData))
                return;
            textBox1.Text += "/**********************************************************************************************************************************************************************************/\r\n";
            DBSQLiteHelper.ExecuteUpdate(initData);
            textBox1.Text += "\r\n";
            textBox1.Text += initData;
            textBox1.Text += "\r\n";
            textBox1.Text += "************************>>ʵ�����ݳ�ʼ���������!";
        }

        private void btnRight_Click(object sender, EventArgs e)
        {
            textBox1.Width -= 400;
            textBox1.Left += 400;
            btnRight.Visible = false;
            btnLeft.Left += 400;
            btnLeft.Visible = true;
            pnlChk.Width += 400;
        }

        private void btnLeft_Click(object sender, EventArgs e)
        {
            pnlChk.Width -= 400;
            btnLeft.Visible = false;
            btnLeft.Left -= 400;
            btnRight.Visible = true;
            textBox1.Left -= 400;
            textBox1.Width += 400;
        }

        private void btnChooseAll_Click(object sender, EventArgs e)
        {
            bool chk = !((CheckBox)pnlChk.Controls[0]).Checked;
            foreach (Control ctrl in pnlChk.Controls)
            {
                ((CheckBox)ctrl).Checked = chk;
            }

        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }


        private void btnChoose_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            fbd.ShowNewFolderButton = true;
            fbd.RootFolder = Environment.SpecialFolder.MyComputer;
            fbd.Description = "��ѡ����Ҫ�������ݿ��·����";
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                DbFilePath = fbd.SelectedPath;
            }
        }

    }
}
