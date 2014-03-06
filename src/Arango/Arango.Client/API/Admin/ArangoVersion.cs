using System;
using System.Text.RegularExpressions;

namespace Arango.Client
{
    /// <summary>
    /// Stores server version data.
    /// </summary>
    public class ArangoVersion
    {
        /// <summary>
        /// Major version part.
        /// </summary>
        public int Major { get; set; }
        
        /// <summary>
        /// Minor version part.
        /// </summary>
        public int Minor { get; set; }
        
        /// <summary>
        /// Patch-level version part.
        /// </summary>
        public int PatchLevel { get; set; }

        /// <summary>
        /// Full version string
        /// </summary>        
        public string Version { get; set; }
        
        /// <summary>
        /// Version constructor
        /// </summary>
        public ArangoVersion(string version)
        {
        	Version = version;
        	
        	Regex re = new Regex(@"^(\d+)\.(\d+)\.(\d+).*$");
            Match match = re.Match(version);
            
            if (re.IsMatch(version))
            {
            	Major = Convert.ToInt32(match.Groups[1].ToString());
            	Minor = Convert.ToInt32(match.Groups[2].ToString());
            	PatchLevel = Convert.ToInt32(match.Groups[3].ToString());
            }
        }
        
        public int toInt() 
        {
        	return PatchLevel + 100 * Minor + 10000 * Major;
        }
        
    }
}
