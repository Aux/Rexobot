using System;
using System.Collections.Generic;

namespace Rexobot
{
    public class RexoUser
    {
        public ulong Id { get; set; }
        public string Email { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<RexoSyncedRole> Roles { get; set; }
    }
}
