using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading;

namespace FrpGUI.Config
{
    public enum NetType
    {
        TCP,
        UDP,
        HTTP,
        HTTPS,
        STCP,

        [Description("STCP访问者")]
        STCP_Visitor,

        //XTCP
    }
}