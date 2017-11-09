using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
