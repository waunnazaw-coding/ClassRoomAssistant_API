using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Topic
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public int? ClassId { get; set; }

    public virtual ICollection<ClassWork> ClassWorks { get; set; } = new List<ClassWork>();
}
