using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class ClassWork
{
    public int Id { get; set; }

    public int ClassId { get; set; }

    public int? TopicId { get; set; }

    public string Type { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Assignment> Assignments { get; set; } = new List<Assignment>();

    public virtual Class Class { get; set; } = null!;

    public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();

    public virtual Topic? Topic { get; set; }
    
    public virtual ICollection<Attachment> Attachments { get; set; } = new List<Attachment>();
}
