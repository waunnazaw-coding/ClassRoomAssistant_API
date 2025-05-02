using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Message
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public int? ReceiverId { get; set; }

    public int? ClassId { get; set; }

    public string Message1 { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Class? Class { get; set; }

    public virtual User? Receiver { get; set; }

    public virtual User Sender { get; set; } = null!;
}
