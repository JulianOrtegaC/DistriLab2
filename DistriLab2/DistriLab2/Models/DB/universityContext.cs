using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DistriLab2.Models.DB
{
    public partial class universityContext : DbContext
    {
        public universityContext()
        {
        }

        public universityContext(DbContextOptions<universityContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Credential> Credentials { get; set; } = null!;
        public virtual DbSet<Inscription> Inscriptions { get; set; } = null!;
        public virtual DbSet<Student> Students { get; set; } = null!;
        public virtual DbSet<Subject> Subjects { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("server=tcp:uniserver1.database.windows.net;database=university;uid=Miguelz;pwd=Admin123");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("Modern_Spanish_CI_AS");

            modelBuilder.Entity<Credential>(entity =>
            {
                entity.HasKey(e => e.CodCredential)
                    .HasName("credential_pk_codcreden");

                entity.Property(e => e.CodCredential)
                    .HasMaxLength(50)
                    .HasColumnName("cod_credential");

                entity.Property(e => e.CodUser)
                    .HasMaxLength(50)
                    .HasColumnName("cod_user");

                entity.Property(e => e.HashUser)
                    .HasMaxLength(900)
                    .HasColumnName("hash_user");

                entity.HasOne(d => d.CodUserNavigation)
                    .WithMany(p => p.Credentials)
                    .HasForeignKey(d => d.CodUser)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("credential_fk_codcreden");
            });

            modelBuilder.Entity<Inscription>(entity =>
            {
                entity.HasKey(e => e.IdInscription)
                    .HasName("inscriptions_pk_idinscription");

                entity.Property(e => e.IdInscription)
                    .ValueGeneratedNever()
                    .HasColumnName("id_inscription");

                entity.Property(e => e.CodStudent).HasColumnName("cod_student");

                entity.Property(e => e.CodSubject).HasColumnName("cod_subject");

                entity.Property(e => e.DateRegistration)
                    .HasColumnType("date")
                    .HasColumnName("date_registration");

                entity.HasOne(d => d.CodStudentNavigation)
                    .WithMany(p => p.Inscriptions)
                    .HasForeignKey(d => d.CodStudent)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("inscriptions_fk_codstudent");

                entity.HasOne(d => d.CodSubjectNavigation)
                    .WithMany(p => p.Inscriptions)
                    .HasForeignKey(d => d.CodSubject)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("inscriptions_fk_codsubject");
            });

            modelBuilder.Entity<Student>(entity =>
            {
                entity.HasKey(e => e.CodStudent)
                    .HasName("students_pk_codstudent");

                entity.Property(e => e.CodStudent)
                    .ValueGeneratedNever()
                    .HasColumnName("cod_student");

                entity.Property(e => e.FirstNameStudent)
                    .HasMaxLength(50)
                    .HasColumnName("first_name_student");

                entity.Property(e => e.GenderStudent)
                    .HasMaxLength(1)
                    .HasColumnName("gender_student")
                    .IsFixedLength();

                entity.Property(e => e.LastNameStudent)
                    .HasMaxLength(50)
                    .HasColumnName("last_name_student");

                entity.Property(e => e.NumDocument)
                    .HasMaxLength(50)
                    .HasColumnName("num_document");

                entity.Property(e => e.PathStudent)
                    .HasMaxLength(600)
                    .IsUnicode(false)
                    .HasColumnName("path_student");

                entity.Property(e => e.StatusStudent)
                    .HasMaxLength(1)
                    .HasColumnName("status_student")
                    .IsFixedLength();

                entity.Property(e => e.TypeDocument)
                    .HasMaxLength(50)
                    .HasColumnName("type_document");
            });

            modelBuilder.Entity<Subject>(entity =>
            {
                entity.HasKey(e => e.CodSubject)
                    .HasName("subjects_pk_codsubject");

                entity.Property(e => e.CodSubject)
                    .ValueGeneratedNever()
                    .HasColumnName("cod_subject");

                entity.Property(e => e.NameSubject)
                    .HasMaxLength(50)
                    .HasColumnName("name_subject");

                entity.Property(e => e.Quotas).HasColumnName("quotas");

                entity.Property(e => e.StatusSubject)
                    .HasMaxLength(1)
                    .HasColumnName("status_subject")
                    .IsFixedLength();
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.CodUser)
                    .HasName("user_pk_coduser");

                entity.Property(e => e.CodUser)
                    .HasMaxLength(50)
                    .HasColumnName("cod_user");

                entity.Property(e => e.EmailUser)
                    .HasMaxLength(50)
                    .HasColumnName("email_user");

                entity.Property(e => e.NameUser)
                    .HasMaxLength(50)
                    .HasColumnName("name_user");

                entity.Property(e => e.StatusUser)
                    .HasMaxLength(1)
                    .HasColumnName("status_user")
                    .IsFixedLength();
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
