using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
using System.Runtime.Serialization;
using System.IO;

namespace Fibonatix.CommDoo.ProcessingCom.Entities
{
    [DataContract]
    class ResponseJSON
    {
        [DataMember(Name = "status")]
        public StatusData status { get; set; }
        [DataMember(Name = "result")]
        public ResultData result { get; set; }

        public static ResponseJSON parseResponseJSON(string data) {
            ResponseJSON result = null;
            try {
                DataContractJsonSerializer jsonFormatter = new DataContractJsonSerializer(typeof(ResponseJSON));
                var ms = new MemoryStream(Encoding.Unicode.GetBytes(data));
                result = (ResponseJSON)jsonFormatter.ReadObject(ms);
            } catch {
            }
            return result;
        }

        [DataContract]
        public class StatusData
        {
            [DataMember(Name = "code")]
            public int code { set; get; }
            [DataMember(Name = "message")]
            public string message { set; get; }
        }

        [DataContract]
        public class ResultData
        {
            [DataMember(Name = "MD")]
            public string MD { set; get; }
            [DataMember(Name = "acs_url")]
            public string acs_url { set; get; }
            [DataMember(Name = "PaReq")]
            public string PaReq { set; get; }
            [DataMember(Name = "eci")]
            public string eci { set; get; }
            [DataMember(Name = "cavv")]
            public string cavv { set; get; }
            [DataMember(Name = "xid")]
            public string xid { set; get; }
            [DataMember(Name = "secure_hash")]
            public string secure_hash { set; get; }
        }
    }
}
