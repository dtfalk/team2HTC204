using System.Runtime.Serialization;

namespace Contoso.Api.Data
{
    public enum OrderStatus
    {
        [EnumMember(Value = "Pending")]
        Pending,
        
        [EnumMember(Value = "Completed")]
        Completed
       
    }
}