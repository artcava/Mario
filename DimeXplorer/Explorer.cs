using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace DimeXplorer
{
    /// <summary>
    /// 
    /// </summary>
    public class Explorer
    {
        private const string _endpoint = "https://prohashing.com/explorerJson/";
        /// <summary>
        /// </summary>
        public Explorer()
        {
            SetData(HttpVerb.GET, "", "ISO-8859-1");
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        public Explorer(HttpVerb method)
        {
            SetData(method, "", "ISO-8859-1");
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="postData"></param>
        public Explorer(HttpVerb method, string postData)
        {
            SetData(method, postData, "ISO-8859-1");
        }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="postData"></param>
        /// <param name="encoding"></param>
        public Explorer(HttpVerb method, string postData, string encoding)
        {
            SetData(method, postData, encoding);
        }

        public HttpVerb Method { get; set; }
        public string ContentType { get; set; }
        public string PostData { get; set; }
        public string TextEncoding { get; set; }

        /// <summary>
        /// </summary>
        /// <param name="method"></param>
        /// <param name="postData"></param>
        /// <param name="encoding"></param>
        private void SetData(HttpVerb method, string postData, string encoding)
        {
            Method = method;
            ContentType = "text/xml";
            PostData = postData;
            TextEncoding = encoding;
        }

        /// <summary>
        /// </summary>
        /// <returns></returns>
        public string MakeRequest()
        {
            return MakeRequest("");
        }

        /// <summary>
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public string MakeRequest(string parameters)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(_endpoint + parameters);

                request.Method = Method.ToString();
                request.ContentLength = 0;
                request.ContentType = ContentType;

                if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
                {
                    var bytes = Encoding.GetEncoding(TextEncoding).GetBytes(PostData);
                    request.ContentLength = bytes.Length;

                    using (Stream writeStream = request.GetRequestStream())
                    {
                        writeStream.Write(bytes, 0, bytes.Length);
                    }
                }

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    string responseValue = string.Empty;

                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        string message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                        throw new ApplicationException(message);
                    }

                    // grab the response
                    using (Stream responseStream = response.GetResponseStream())
                    {
                        if (responseStream != null)
                            using (var reader = new StreamReader(responseStream))
                            {
                                responseValue = reader.ReadToEnd();
                            }
                    }
                    return responseValue;
                }

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        #region EntryPoints
        /// <summary>
        /// EntryPoint: [https://prohashing.com/explorerJson/getTransactionsByAddress?$params={"page":1,"count":20,"sorting":{"blocktime":"asc"},"group":{},"groupBy":null}&address={address}&coin_id=101]
        /// </summary>
        /// <returns></returns>
        public DimeTransactionList getTransactionsByAddress(int page, int count, string address)
        {
            var parameters = $"getTransactionsByAddress?$params={{'page':{page},'count':{count},'filter':{{}},'sorting':{{'blocktime':'asc'}},'group':{{}},'groupBy':null}}&address={address}&coin_id=101";
            string json = MakeRequest(parameters);
            if (json == null) return null;
            return JsonConvert.DeserializeObject<DimeTransactionList>(json);
        }

        /// <summary>
        /// EntryPoint: [https://prohashing.com/explorerJson/getTransaction?coin_id=101&transaction_id={transaction}]
        /// </summary>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public Transaction getTransaction(string transaction)
        {
            var parameters = $"getTransaction?coin_id=101&transaction_id={transaction}";
            string json = MakeRequest(parameters);
            if (json == null) return null;
            return JsonConvert.DeserializeObject<Transaction>(json);
        }
        #endregion
    }

    public static class Wallet
    {
        private static string _filename = Directory.GetCurrentDirectory() + @"\leveldata.xml";
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallet"></param>
        public static void SetPrivateWallet(string wallet)
        {
            SetWallet(wallet, WalletType.Private);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallet"></param>
        public static void SetPublicWallet(string wallet)
        {
            SetWallet(wallet, WalletType.Public);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetPrivateWallet()
        {
            return GetWallet(WalletType.Private);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static string GetPublicWallet()
        {
            return GetWallet(WalletType.Public);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wallet"></param>
        /// <param name="wType"></param>
        private static void SetWallet(string wallet, WalletType wType)
        {
            string sWallet = string.Empty;
            switch (wType)
            {
                case WalletType.Private:
                    sWallet = "PrivateWallet";
                    break;
                case WalletType.Public:
                    sWallet = "PublicWallet";
                    break;
            }

            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);

            if (doc.DocumentElement.Attributes[sWallet] == null)
                doc.DocumentElement.Attributes.Append(doc.CreateAttribute(sWallet));
            doc.DocumentElement.SetAttribute(sWallet, wallet);
            doc.Save(_filename);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="wType"></param>
        /// <returns></returns>
        private static string GetWallet(WalletType wType)
        {
            string wallet = string.Empty;
            switch(wType)
            {
                case WalletType.Private:
                    wallet = "PrivateWallet";
                    break;
                case WalletType.Public:
                    wallet = "PublicWallet";
                    break;
            }
            XmlDocument doc = new XmlDocument();
            doc.Load(_filename);
            return doc.DocumentElement.Attributes[wallet]?.Value;
        }

    }

    public enum WalletType
    {
        Private,
        Public
    }
    /// <summary>
    /// </summary>
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }

    #region Json models
    [JsonObject(MemberSerialization.OptIn)]
    public class DimeTransactionList
    {
        [JsonProperty(PropertyName = "data")]
        public List<DimeTransaction> Data { get; set; }

        [JsonProperty(PropertyName = "total")]
        public int Total { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class DimeTransaction
    {
        [JsonProperty(PropertyName = "blocktime")]
        public DateTime BlockTime { get; set; }

        [JsonProperty(PropertyName = "transaction_hash")]
        public string TransactionHash { get; set; }

        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }

        [JsonProperty(PropertyName = "height")]
        public int Height { get; set; }
    }

    [JsonObject(MemberSerialization.OptIn)]
    public class Transaction
    {
        [JsonProperty(PropertyName = "address_inputs")]
        public List<Address> AddressInputs { get; set; }

        [JsonProperty(PropertyName = "address_outputs")]
        public List<Address> AddressOutputs { get; set; }

        [JsonProperty(PropertyName = "block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty(PropertyName = "block_height")]
        public int Height { get; set; }

        [JsonProperty(PropertyName = "blocktime")]
        public long BlockTime { get; set; }

        [JsonProperty(PropertyName = "confirmations")]
        public int Confirmations { get; set; }

        [JsonProperty(PropertyName = "total_inputs")]
        public decimal Inputs { get; set; }

        [JsonProperty(PropertyName = "total_outputs")]
        public decimal Outputs { get; set; }

        [JsonProperty(PropertyName = "transaction_hash")]
        public string TransactionHash { get; set; }
    }

    public class Address
    {
        [JsonProperty(PropertyName = "address")]
        public string WalletAddress { get; set; }

        [JsonProperty(PropertyName = "value")]
        public decimal Value { get; set; }
    }
    #endregion
}
