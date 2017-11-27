using System;
using System.Runtime.Serialization;

namespace DigDesNote.UI.WPF
{
    [DataContract]
    public class LoginSet
    {
        [DataMember]
        public bool _statusLogin;
        [DataMember]
        public Guid _userId;
    }
}
