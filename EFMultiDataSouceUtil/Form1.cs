using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EFMultiDataSouceUtil
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

        }

        /// <summary>
        /// 执行
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExecute_Click(object sender, EventArgs e)
        {
            txtState.Text = "";
            LoopDBMember loop = new LoopDBMember();
            var dbStrList = loop.LoopMemberDB();
            PrintState("获取加盟商成功，数量：\n\r" + dbStrList.Count);

            int count = 0;
            foreach (DictionaryEntry de in dbStrList)
            {
                // 调用执行SQL
                RunSqlCmd(de.Value as string, de.Key as string);
                count++;
            }
            PrintState("执行成功" + count + "个加盟商\n\r");
        }
        /// <summary>
        /// 执行SQL
        /// </summary>
        /// <param name="strBSConnection">加盟商连接字符串</param>
        /// <param name="memberCode">加盟商代码</param>
        private void RunSqlCmd(string strBSConnection, string memberCode)
        {
            SqlConnection connection = new SqlConnection(strBSConnection);
            connection.Open();
            PrintState("数据库打开连接成功！\n\r");
            SqlCommand cmd = new SqlCommand(txtSql.Text, connection);
            int ffectRows = cmd.ExecuteNonQuery();
            PrintState("SQL执行成功！\n\r");

            string outPrintStr = string.Format("执行{0}加盟商-连接字符串{1}-影响行数{2}\n\r", memberCode, strBSConnection, ffectRows);
            PrintState(outPrintStr);

            PrintState(memberCode + "加盟商执行完成！\n\r");
        }

        /// <summary>
        /// 输出状态
        /// </summary>
        /// <param name="message">状态</param>
        private void PrintState(string message)
        {
            txtState.Text = message + txtState.Text;
        }
    }


}
