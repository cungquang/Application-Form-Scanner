using System.ComponentModel;
using System.Runtime.InteropServices;

namespace BCHousing.AfsWebAppMvc.Servives.UtilityService
{
    public class UtilityService
    {
        public UtilityService() { }

        public static Guid GenerateCustomGuid()
        {
            return Guid.NewGuid();
        }
    }
}
