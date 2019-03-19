using System;
using System.Runtime.Serialization;

namespace WCFChatBase
{
    [DataContract]
    public class Client
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public int AvatarID { get; set; }

        [DataMember]
        public DateTime Time { get; set; }
    }
}
