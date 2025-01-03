﻿// <auto-generated />
using System;
using API.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace API.Migrations
{
    [DbContext(typeof(MyContext))]
    [Migration("20241110130958_approve amount reimbursement")]
    partial class approveamountreimbursement
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.20")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("API.Models.Account", b =>
                {
                    b.Property<string>("Id_Account")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Password")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Role_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id_Account");

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("API.Models.AccountDetail", b =>
                {
                    b.Property<string>("Id_AccountDetail")
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Birth_Date")
                        .HasColumnType("datetime2");

                    b.Property<float>("Current_Limit")
                        .HasColumnType("real");

                    b.Property<string>("Gender")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Id_Title")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<DateTime>("Join_Date")
                        .HasColumnType("datetime2");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Phone")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id_AccountDetail");

                    b.HasIndex("Id_Title");

                    b.ToTable("AccountDetails");
                });

            modelBuilder.Entity("API.Models.Category", b =>
                {
                    b.Property<string>("Id_Category")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Category_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id_Category");

                    b.ToTable("Categories");
                });

            modelBuilder.Entity("API.Models.Reimbursement", b =>
                {
                    b.Property<string>("Id_Reimbursement")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("Amount")
                        .HasColumnType("real");

                    b.Property<float>("Approve_Amount")
                        .HasColumnType("real");

                    b.Property<string>("Evidence")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Id_Category")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Note")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Status")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("Submit_Date")
                        .HasColumnType("datetime2");

                    b.HasKey("Id_Reimbursement");

                    b.HasIndex("Id_Category");

                    b.ToTable("Reimbursements");
                });

            modelBuilder.Entity("API.Models.ReimbursementProfiling", b =>
                {
                    b.Property<string>("Id_Account")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(0);

                    b.Property<string>("Id_Reimbursement")
                        .HasColumnType("nvarchar(450)")
                        .HasColumnOrder(1);

                    b.HasKey("Id_Account", "Id_Reimbursement");

                    b.HasIndex("Id_Reimbursement");

                    b.ToTable("ReimbursementProfilings");
                });

            modelBuilder.Entity("API.Models.Title", b =>
                {
                    b.Property<string>("Id_Title")
                        .HasColumnType("nvarchar(450)");

                    b.Property<float>("Reimburse_Limit")
                        .HasColumnType("real");

                    b.Property<string>("Title_Name")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id_Title");

                    b.ToTable("Titles");
                });

            modelBuilder.Entity("API.Models.AccountDetail", b =>
                {
                    b.HasOne("API.Models.Account", "Account")
                        .WithOne("AccountDetails")
                        .HasForeignKey("API.Models.AccountDetail", "Id_AccountDetail")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Models.Title", "Title")
                        .WithMany("AccountDetails")
                        .HasForeignKey("Id_Title")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Title");
                });

            modelBuilder.Entity("API.Models.Reimbursement", b =>
                {
                    b.HasOne("API.Models.Category", "Category")
                        .WithMany("Reimbursements")
                        .HasForeignKey("Id_Category")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Category");
                });

            modelBuilder.Entity("API.Models.ReimbursementProfiling", b =>
                {
                    b.HasOne("API.Models.Account", "Account")
                        .WithMany("ReimbursementProfilings")
                        .HasForeignKey("Id_Account")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("API.Models.Reimbursement", "Reimbursement")
                        .WithMany("ReimbursementProfilings")
                        .HasForeignKey("Id_Reimbursement")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Account");

                    b.Navigation("Reimbursement");
                });

            modelBuilder.Entity("API.Models.Account", b =>
                {
                    b.Navigation("AccountDetails")
                        .IsRequired();

                    b.Navigation("ReimbursementProfilings");
                });

            modelBuilder.Entity("API.Models.Category", b =>
                {
                    b.Navigation("Reimbursements");
                });

            modelBuilder.Entity("API.Models.Reimbursement", b =>
                {
                    b.Navigation("ReimbursementProfilings");
                });

            modelBuilder.Entity("API.Models.Title", b =>
                {
                    b.Navigation("AccountDetails");
                });
#pragma warning restore 612, 618
        }
    }
}
