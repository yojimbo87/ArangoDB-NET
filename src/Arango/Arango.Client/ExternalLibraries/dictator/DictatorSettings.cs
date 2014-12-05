using System;

namespace Arango.Client
{
    public class DictatorSettings
    {
        public EnumFormat EnumFormat { get; set; }
        public MergeBehavior MergeBehavior { get; set; }
        public DateTimeFormat DateTimeFormat { get; set; }
        public string DateTimeStringFormat { get; set; }
        public DateTime UnixEpoch { get; private set; }
        
        internal DictatorSettings()
        {
            UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            
            ResetToDefaults();
        }
        
        public void ResetToDefaults()
        {
            EnumFormat = EnumFormat.Object;
            MergeBehavior = MergeBehavior.OverwriteFields;
            DateTimeFormat = DateTimeFormat.Object;
            DateTimeStringFormat = "yyyy-MM-ddTHH:mm:ss.fffZ";
        }
    }
}
