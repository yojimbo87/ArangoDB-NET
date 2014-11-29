using System;
using System.Collections.Generic;

namespace Arango.Client
{
    public static class Dictator
    {
        public static DictatorSettings Settings { get; private set; }
        
        public static Dictionary<string, object> New()
        {
            return new Dictionary<string, object>();
        }
        
        static Dictator()
        {
            Settings = new DictatorSettings();
        }
    }
}

