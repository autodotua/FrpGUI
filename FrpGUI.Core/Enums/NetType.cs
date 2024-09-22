using System.ComponentModel;

namespace FrpGUI.Enums
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