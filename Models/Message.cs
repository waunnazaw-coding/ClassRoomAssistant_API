using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Message
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public string ParentType { get; set; } = null!;

    public int ParentId { get; set; }

    public string Message1 { get; set; } = null!;

    public bool? IsPrivate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User Sender { get; set; } = null!;
}
