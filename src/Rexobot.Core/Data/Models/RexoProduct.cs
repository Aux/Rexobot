using System;
using System.Collections.Generic;

namespace Rexobot
{
    public class RexoProduct
    {
        public string Id { get; set; }
        public ulong GuildId { get; set; }
        public string Name { get; set; }
        public string PreviewImageUrl { get; set; }
        public ulong RoleId { get; set; }
        public ulong? WatchMessageId { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        public List<RexoSyncedRole> Roles { get; set; }
    }
}
