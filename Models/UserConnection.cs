using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class UserConnection
{
    public int UserId { get; set; }

    public string ConnectionId { get; set; } = null!;

    public DateTime? ConnectedAt { get; set; }
}
