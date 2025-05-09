using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public class UserClassesRawDto
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string? Section { get; set; }
    public string? Subject { get; set; }
    
    public string ClassCode { get; set; } 
    public string? Room { get; set; }
    public int? CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
}