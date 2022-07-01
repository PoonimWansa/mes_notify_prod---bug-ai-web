using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Net;
using System.IO;
using System.Security.Cryptography;
using System.Text;


namespace mes_notify_prod.Models
{
    #region *** Parameter
    public static class MQTTMES
    {
        public static string MQServer = "10.150.224.6";
        public static string Topic = "IT:DETMES:POCDelay";
    }

    public static class MQTTDCIM
    {
        public static string MQServer = "10.150.224.6";
        public static string Topic = "IT:DETSO:DCIM";
    }

    #endregion

    #region *** Function
    public static class KeyGenerator
    {
        internal static readonly char[] chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

        public static string GetUniqueKey(int size)
        {
            byte[] data = new byte[4 * size];
            using (var crypto = RandomNumberGenerator.Create())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                var rnd = BitConverter.ToUInt32(data, i * 4);
                var idx = rnd % chars.Length;

                result.Append(chars[idx]);
            }

            return result.ToString();
        }

        public static string GetUniqueKeyOriginal_BIASED(int size)
        {
            char[] chars =
                "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[size];
            using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
            {
                crypto.GetBytes(data);
            }
            StringBuilder result = new StringBuilder(size);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }
    }

    public static class MyHttp
    {
        public static bool Get(string token_name, string token_key, string url, out string result, out string msg)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);

            if (token_name != "")
            {
                request.Headers[token_name] = token_key;
            }

            try
            {
                WebResponse response = request.GetResponse();
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.UTF8);
                    result = reader.ReadToEnd();
                }

                msg = "";
                return true;
            }
            catch (WebException ex)
            {
                WebResponse errorResponse = ex.Response;
                using (Stream responseStream = errorResponse.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, System.Text.Encoding.GetEncoding("utf-8"));
                    msg = reader.ReadToEnd();
                    result = msg;
                    return false;
                }
            }
        }
    }

    public static class MyMQTT
    {

        public static bool SentMessage(string MQServer, string Topic, string MsgSend, out string MsgReturn)
        {
            try
            {
                uPLibrary.Networking.M2Mqtt.MqttClient mqttClient = new uPLibrary.Networking.M2Mqtt.MqttClient(MQServer);
                string clientID = Guid.NewGuid().ToString();
                mqttClient.Connect(clientID);
                mqttClient.Publish(Topic, System.Text.Encoding.UTF8.GetBytes(MsgSend));
                System.Threading.Thread.Sleep(50);
                mqttClient.Disconnect();
                MsgReturn = "";
                return true;
            }
            catch (Exception ex)
            {
                MsgReturn = ex.Message;
                return false;
            }
        }

    }

    #endregion



}