using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Class
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string ClassCode { get; set; } = null!;

    public string? Section { get; set; }

    public string? Subject { get; set; }

    public string? Room { get; set; }

    public int? CreatedBy { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool? IsDeleted { get; set; }

    public virtual ICollection<Announcement> Announcements { get; set; } = new List<Announcement>();

    public virtual ICollection<ClassParticipant> ClassParticipants { get; set; } = new List<ClassParticipant>();

    public virtual ICollection<ClassWork> ClassWorks { get; set; } = new List<ClassWork>();

    public virtual User? CreatedByNavigation { get; set; }

    public virtual ICollection<Topic> Topics { get; set; } = new List<Topic>();
}
