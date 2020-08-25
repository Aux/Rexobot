using System;

namespace Rexobot
{
    public class RexoSyncedRole
    {
        public ulong Id { get; set; }
        public ulong UserId { get; set; }
        public string ProductId { get; set; }
        public string OrderId { get; set; }
        public DateTime LinkedAt { get; set; } = DateTime.UtcNow;

        public RexoUser User { get; set; }
        public RexoProduct Product { get; set; }
    }
}
