﻿using System;
using System.Collections.Generic;

namespace ClassRoomClone_App.Server.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public string? Profile { get; set; }

    public string? RefreshToken { get; set; }

    public DateTime? RefreshTokenExpiryTime { get; set; }

    public virtual ICollection<AssignmentProgress> AssignmentProgressGradedByNavigations { get; set; } = new List<AssignmentProgress>();

    public virtual ICollection<AssignmentProgress> AssignmentProgressReviewedByNavigations { get; set; } = new List<AssignmentProgress>();

    public virtual ICollection<AssignmentSubmission> AssignmentSubmissions { get; set; } = new List<AssignmentSubmission>();

    public virtual ICollection<ClassParticipant> ClassParticipantAddedByNavigations { get; set; } = new List<ClassParticipant>();

    public virtual ICollection<ClassParticipant> ClassParticipantUsers { get; set; } = new List<ClassParticipant>();

    public virtual ICollection<Grade> GradeGradedByNavigations { get; set; } = new List<Grade>();

    public virtual ICollection<Grade> GradeStudents { get; set; } = new List<Grade>();

    public virtual ICollection<Message> MessageReceivers { get; set; } = new List<Message>();

    public virtual ICollection<Message> MessageSenders { get; set; } = new List<Message>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Todo> Todos { get; set; } = new List<Todo>();
}
