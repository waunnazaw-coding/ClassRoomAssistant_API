using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ClassRoomClone_App.Server.Models;

public partial class DbContextClassName : DbContext
{
    public DbContextClassName()
    {
    }

    public DbContextClassName(DbContextOptions<DbContextClassName> options)
        : base(options)
    {
    }

    public virtual DbSet<Announcement> Announcements { get; set; }

    public virtual DbSet<Assignment> Assignments { get; set; }

    public virtual DbSet<AssignmentProgress> AssignmentProgresses { get; set; }

    public virtual DbSet<AssignmentSubmission> AssignmentSubmissions { get; set; }

    public virtual DbSet<Attachment> Attachments { get; set; }

    public virtual DbSet<Class> Classes { get; set; }

    public virtual DbSet<ClassParticipant> ClassParticipants { get; set; }

    public virtual DbSet<ClassWork> ClassWorks { get; set; }

    public virtual DbSet<Grade> Grades { get; set; }

    public virtual DbSet<Material> Materials { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<SubmissionResponse> SubmissionResponses { get; set; }

    public virtual DbSet<Todo> Todos { get; set; }

    public virtual DbSet<Topic> Topics { get; set; }

    public virtual DbSet<User> Users { get; set; }
    
    public virtual DbSet<UserNotificationRawDto> UserNotificationRawDtos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=.;Database=ClassRoomDb;User Id=sa;Password=waunnazaw;TrustServerCertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserNotificationRawDto>(entity =>
        {
            entity.HasNoKey();
            entity.ToView(null);
        });
        
        modelBuilder.Entity<Announcement>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Announce__3214EC07E2CAF7DF");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Class).WithMany(p => p.Announcements)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Announcem__Class__6C190EBB");
        });

        modelBuilder.Entity<Assignment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assignme__3214EC07476AA540");

            entity.Property(e => e.AllowLateSubmission).HasDefaultValue(false);
            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.ClassWork).WithMany(p => p.Assignments)
                .HasForeignKey(d => d.ClassWorkId)
                .HasConstraintName("FK__Assignmen__Class__534D60F1");
        });

        modelBuilder.Entity<AssignmentProgress>(entity =>
        {
            entity.HasKey(e => e.AssignmentId).HasName("PK__Assignme__32499E77426D5D92");

            entity.ToTable("AssignmentProgress");

            entity.Property(e => e.AssignmentId).ValueGeneratedNever();
            entity.Property(e => e.GradedCount).HasDefaultValue(0);
            entity.Property(e => e.LastGradedAt).HasColumnType("datetime");
            entity.Property(e => e.LastReviewedAt).HasColumnType("datetime");
            entity.Property(e => e.TurnedInCount).HasDefaultValue(0);

            entity.HasOne(d => d.Assignment).WithOne(p => p.AssignmentProgress)
                .HasForeignKey<AssignmentProgress>(d => d.AssignmentId)
                .HasConstraintName("FK__Assignmen__Assig__5CD6CB2B");

            entity.HasOne(d => d.GradedByNavigation).WithMany(p => p.AssignmentProgressGradedByNavigations)
                .HasForeignKey(d => d.GradedBy)
                .HasConstraintName("FK__Assignmen__Grade__5EBF139D");

            entity.HasOne(d => d.ReviewedByNavigation).WithMany(p => p.AssignmentProgressReviewedByNavigations)
                .HasForeignKey(d => d.ReviewedBy)
                .HasConstraintName("FK__Assignmen__Revie__5DCAEF64");
        });

        modelBuilder.Entity<AssignmentSubmission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Assignme__3214EC078498402E");

            entity.HasIndex(e => new { e.AssignmentId, e.StudentId }, "UQ_Assignment_Student").IsUnique();

            entity.Property(e => e.SubmittedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Assignment).WithMany(p => p.AssignmentSubmissions)
                .HasForeignKey(d => d.AssignmentId)
                .HasConstraintName("FK__Assignmen__Assig__1EA48E88");

            entity.HasOne(d => d.Student).WithMany(p => p.AssignmentSubmissions)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Assignmen__Stude__1F98B2C1");
        });

        modelBuilder.Entity<Attachment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Attachme__3214EC0761B3A42D");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.FileType)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.FileUrl).HasMaxLength(500);
            entity.Property(e => e.ReferenceType)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Attachments)
                .HasForeignKey(d => d.CreatedBy)
                .HasConstraintName("FK__Attachmen__Creat__68487DD7");
        });

        modelBuilder.Entity<Class>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Classes__3214EC0790070208");

            entity.Property(e => e.ClassCode)
                .HasMaxLength(10)
                .IsFixedLength();
            entity.Property(e => e.CreatedDate)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Room).HasMaxLength(50);
            entity.Property(e => e.Section).HasMaxLength(100);
            entity.Property(e => e.Subject).HasMaxLength(255);

            entity.HasOne(d => d.CreatedByNavigation).WithMany(p => p.Classes)
                .HasForeignKey(d => d.CreatedBy)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Classes__Created__3C69FB99");
        });

        modelBuilder.Entity<ClassParticipant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassPar__3214EC072C62302F");

            entity.HasIndex(e => new { e.ClassId, e.UserId }, "UQ__ClassPar__1A61AB05DA87CB8D").IsUnique();

            entity.Property(e => e.AddedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsOwner).HasDefaultValue(false);
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.AddedByNavigation).WithMany(p => p.ClassParticipantAddedByNavigations)
                .HasForeignKey(d => d.AddedBy)
                .HasConstraintName("FK__ClassPart__Added__44FF419A");

            entity.HasOne(d => d.Class).WithMany(p => p.ClassParticipants)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ClassPart__Class__4316F928");

            entity.HasOne(d => d.User).WithMany(p => p.ClassParticipantUsers)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__ClassPart__UserI__440B1D61");
        });

        modelBuilder.Entity<ClassWork>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__ClassWor__3214EC071D16154D");

            entity.ToTable("ClassWork");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Class).WithMany(p => p.ClassWorks)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__ClassWork__Class__4E88ABD4");

            entity.HasOne(d => d.Topic).WithMany(p => p.ClassWorks)
                .HasForeignKey(d => d.TopicId)
                .HasConstraintName("FK__ClassWork__Topic__4F7CD00D");
        });

        modelBuilder.Entity<Grade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Grades__3214EC07CE0B97BA");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.MaxScore).HasColumnType("decimal(5, 2)");
            entity.Property(e => e.Score).HasColumnType("decimal(5, 2)");

            entity.HasOne(d => d.ClassWork).WithMany(p => p.Grades)
                .HasForeignKey(d => d.ClassWorkId)
                .HasConstraintName("FK__Grades__ClassWor__7C4F7684");

            entity.HasOne(d => d.GradedByNavigation).WithMany(p => p.GradeGradedByNavigations)
                .HasForeignKey(d => d.GradedBy)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__GradedBy__7D439ABD");

            entity.HasOne(d => d.Student).WithMany(p => p.GradeStudents)
                .HasForeignKey(d => d.StudentId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Grades__StudentI__7B5B524B");
        });

        modelBuilder.Entity<Material>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Material__3214EC078500C52D");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.ClassWork).WithMany(p => p.Materials)
                .HasForeignKey(d => d.ClassWorkId)
                .HasConstraintName("FK__Materials__Class__628FA481");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Messages__3214EC0798229969");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsPrivate).HasDefaultValue(false);
            entity.Property(e => e.Message1).HasColumnName("Message");
            entity.Property(e => e.ParentType)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers)
                .HasForeignKey(d => d.ReceiverId)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("FK__Messages__Receiv__18EBB532");

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders)
                .HasForeignKey(d => d.SenderId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Messages__Sender__17F790F9");
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Notifica__3214EC0715DA8A38");

            entity.HasIndex(e => e.CreatedAt, "idx_notifications_created").IsDescending();

            entity.HasIndex(e => e.UserId, "idx_notifications_unread").HasFilter("([IsRead]=(0))");

            entity.HasIndex(e => e.UserId, "idx_notifications_user");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.Type)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.User).WithMany(p => p.Notifications)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Notificat__UserI__123EB7A3");
        });

        modelBuilder.Entity<SubmissionResponse>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Submissi__3214EC0757E37220");

            entity.Property(e => e.FileName).HasMaxLength(255);
            entity.Property(e => e.FilePath).HasMaxLength(500);
            entity.Property(e => e.Link).HasMaxLength(1000);
            entity.Property(e => e.MimeType).HasMaxLength(100);
            entity.Property(e => e.ResponseType).HasMaxLength(20);
            entity.Property(e => e.UploadedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.Submission).WithMany(p => p.SubmissionResponses)
                .HasForeignKey(d => d.SubmissionId)
                .HasConstraintName("FK__Submissio__Submi__236943A5");
        });

        modelBuilder.Entity<Todo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Todos__3214EC07678C3F2A");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.IsMissing).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasDefaultValue("Pending");

            entity.HasOne(d => d.ClassWork).WithMany(p => p.Todos)
                .HasForeignKey(d => d.ClassWorkId)
                .HasConstraintName("FK__Todos__ClassWork__03F0984C");

            entity.HasOne(d => d.User).WithMany(p => p.Todos)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Todos__UserId__02FC7413");
        });

        modelBuilder.Entity<Topic>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Topics__3214EC07BF7EE5E0");

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasOne(d => d.Class).WithMany(p => p.Topics)
                .HasForeignKey(d => d.ClassId)
                .HasConstraintName("FK__Topics__ClassId__49C3F6B7");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Users__3214EC0797B86277");

            entity.HasIndex(e => e.Email, "UQ__Users__A9D1053437385872").IsUnique();

            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
