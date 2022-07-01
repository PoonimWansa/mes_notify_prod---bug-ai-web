using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;

namespace mes_notify_prod.Models
{

    public static class TdcOnlineAction
    {
        private static string token_name = "tokenID";
        private static string token = "894A0F0DF8494799E0530CCA940AC604";
        private static string token_sign = "894A0F0DF84A4799E0530CCA940AC604";
        private static string apiurl = "http://10.150.192.16:10101";

        public static bool GetMolist(string Factory, string LineName, out string msg, out string result)
        {
            string EMP_NO = "86057312";
            string FACTORY = Factory;
            string GETDATA_TYPE = "0";
            string LINE_NAME = LineName;
            string MO_TYPE = "0";

            string Raw = $"{token_sign}EMP_NO{EMP_NO}FACTORY{FACTORY}GETDATA_TYPE{GETDATA_TYPE}LINE_NAME{LINE_NAME}MO_TYPE{MO_TYPE}";
            string Sign = GetMD5(Raw);
            bool ret = MyHttp.Get(token_name, token
                 , $"{apiurl}/QueryData/MOList?" +
                    $"EMP_NO={EMP_NO}" +
                    $"&FACTORY={FACTORY}" +
                    $"&GETDATA_TYPE={GETDATA_TYPE}" +
                    $"&LINE_NAME={LINE_NAME}" +
                    $"&MO_TYPE={MO_TYPE}" +
                    $"&sign={Sign}"
                 , out result, out msg);

            return ret;
        }

        public static string GetMD5(string str)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("X2"));
            }
            return sBuilder.ToString();
        }
    }


}