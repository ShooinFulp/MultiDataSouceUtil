using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace EFMultiDataSouceUtil
{
    class LoopDBMember
    {
        public Hashtable LoopMemberDB()
        {
            // key, value =》workflowDB,BusinessDB
            Hashtable memberList = new Hashtable();
            try
            {
                // 获取加盟商API的服务URL
                string _apiAddress = "http://intpassport.einwin.com";
                System.Net.HttpWebRequest httpWebRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(_apiAddress + "/ContractService/GetValidServiceMemberInfo");
                httpWebRequest.ContentType = "application/json";
                httpWebRequest.Method = "POST";
                using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
                {
                    streamWriter.Write("");
                    streamWriter.Flush();
                    streamWriter.Close();
                    var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                    {
                        var result = streamReader.ReadToEnd();
                        JavaScriptSerializer jss = new JavaScriptSerializer();
                        var jsonvalue = jss.Deserialize<ReturnResult<OutValidServiceMemberInfo<OutDatabaseInfo>>>(result);
                        if (jsonvalue.Data != null)
                        {
                            string strWFDB = string.Empty;
                            string strBSDB = string.Empty;
                            for (var i = 0; i < jsonvalue.Data.Count; i++)
                            {
                                strWFDB = string.Empty;
                                strBSDB = string.Empty;
                                var Data = jsonvalue.Data[i];
                                var DbInfo = Data.DbInfo;
                                var memberCode = string.Empty;
                                memberCode = Data.MemberCode;
                                foreach (var item in DbInfo)
                                {
                                    if (item.DbType.Equals("BusinessSystem"))
                                    {
                                        strBSDB = string.Format("Server={0};Database={1};Uid={2};Pwd={3};MultipleActiveResultSets=true",
                                            item.Server, item.Database, item.Username, item.Password);

                                    }
                                    else if (item.DbType.Equals("Workflow"))
                                    {
                                        strWFDB = string.Format("Server={0};Database={1};Uid={2};Pwd={3};MultipleActiveResultSets=true",
                                                                                  item.Server, item.Database, item.Username, item.Password);
                                    }
                                }
                                if (!string.IsNullOrEmpty(strBSDB))
                                {
                                    memberList.Add(memberCode, strBSDB);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // logger.Error(ex.Message + ex.StackTrace);
            }
            return memberList;
        }
    }


    public class ReturnResult<T>
    {
        public IList<T> Data { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public bool Result { get; set; }
    }

    /// <summary>
    /// 请求数据库连接接口输出类
    /// </summary>
    public class OutDatabaseInfo
    {
        /// <summary>
        /// 服务器地址
        /// </summary>
        public string Server { get; set; }
        /// <summary>
        /// 数据库名称
        /// </summary>
        public string Database { get; set; }
        /// <summary>
        /// 用户名
        /// </summary>
        public string Username { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Password { get; set; }
        /// <summary>
        /// 数据库配置类型(数据字典项Code)
        /// </summary>
        public string DbType { get; set; }
    }

    /// <summary>
    /// 获取所有有效服务的盟友信息接口输出类
    /// </summary>
    public class OutValidServiceMemberInfo<T>
    {
        /// <summary>
        /// 盟友编号
        /// </summary>
        public string MemberCode { get; set; }
        /// <summary>
        /// 系统类型
        /// </summary>
        public List<EnumSystemType> SystemType { get; set; }
        /// <summary>
        /// 数据库连接
        /// </summary>
        public List<T> DbInfo { get; set; }
    }

    /// <summary>
    /// 系统类型枚举
    /// </summary>
    public enum EnumSystemType
    {
        PMS = 1,
        CRM = 2,
        UMall = 3
    }
}
