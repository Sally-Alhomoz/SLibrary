using System;

namespace Shared
{
    public class Userdto
    {
        public Guid Id { get; set; }     
        public string Username { get; set; } 
        public Role Role { get; set; }
    }
}
