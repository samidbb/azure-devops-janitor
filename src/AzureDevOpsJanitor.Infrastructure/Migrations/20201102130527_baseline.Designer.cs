﻿// <auto-generated />
using AzureDevOpsJanitor.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AzureDevOpsJanitor.Infrastructure.Migrations
{
    [DbContext(typeof(DomainContext))]
    [Migration("20201102130527_baseline")]
    partial class baseline
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.3");

            modelBuilder.Entity("AzureDevOpsJanitor.Domain.Aggregates.Build.BuildRoot", b =>
                {
                    b.Property<ulong>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("StatusId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("StatusId");

                    b.ToTable("Build");
                });

            modelBuilder.Entity("AzureDevOpsJanitor.Domain.Aggregates.Build.BuildStatus", b =>
                {
                    b.Property<int>("Id")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Status");
                });

            modelBuilder.Entity("AzureDevOpsJanitor.Domain.Aggregates.Build.BuildRoot", b =>
                {
                    b.HasOne("AzureDevOpsJanitor.Domain.Aggregates.Build.BuildStatus", "Status")
                        .WithMany()
                        .HasForeignKey("StatusId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
