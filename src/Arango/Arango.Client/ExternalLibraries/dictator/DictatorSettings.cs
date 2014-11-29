using System;

namespace Arango.Client
{
    public class DictatorSettings
    {
        public DateTimeFormat DateTimeFormat { get; set; }
        public EnumFormat EnumFormat { get; set; }
        public string DateTimeStringFormat { get; set; }
        public DateTime UnixEpoch { get; private set; }
        
        public DictatorSettings()
        {
            UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            ResetToDefaults();
        }
        
        public void ResetToDefaults()
        {
            DateTimeFormat = DateTimeFormat.Object;
            EnumFormat = EnumFormat.Object;
            DateTimeStringFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        }
    }
}
