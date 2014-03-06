using System;

namespace Arango.Client
{
    /// <summary>
    /// Stores server role.
    /// </summary>
    public class ArangoServerRole
    {
    	public enum RoleType
    	{
    		COORDINATOR,
    		DBSERVER,
    		UNKNOWN
    	}
    	
        /// <summary>
        /// Server role
        /// </summary>        
        public RoleType Role { get; set; }            	

        /// <summary>
        /// Server role constructor
        /// </summary>
        public ArangoServerRole(string role)
        {
        	if (role.Equals("COORDINATOR"))
        	{
        		Role = RoleType.COORDINATOR;
        	}
        	else if (role.Equals("DBSERVER"))
        	{
        		Role = RoleType.DBSERVER;
        	}
        	else 
        	{
        		Role = RoleType.UNKNOWN;
        	}
        }
        
        /// <summary>
        /// Server role constructor
        /// </summary>
        public ArangoServerRole(RoleType role)
        {
        	Role = role;
        }
        
        public bool IsCoordinator()
        {
        	return Role == RoleType.COORDINATOR;
        }

        public bool IsCluster()
        {
        	return (Role == RoleType.COORDINATOR || Role == RoleType.DBSERVER);
        }
        
    }
}
