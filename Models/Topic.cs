using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Topic
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public int ClassId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<ClassWork> ClassWorks { get; set; } = new List<ClassWork>();
}
