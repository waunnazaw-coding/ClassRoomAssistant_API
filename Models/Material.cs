using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class Material
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int ClassWorkId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ClassWork ClassWork { get; set; } = null!;
   

}
