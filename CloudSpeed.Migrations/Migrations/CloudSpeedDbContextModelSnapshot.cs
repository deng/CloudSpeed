﻿// <auto-generated />
using System;
using CloudSpeed.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CloudSpeed.Migrations.Migrations
{
    [DbContext(typeof(CloudSpeedDbContext))]
    partial class CloudSpeedDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.1");

            modelBuilder.Entity("CloudSpeed.Entities.FileCid", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<string>("Cid")
                        .HasColumnName("name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<byte>("Status")
                        .HasColumnName("status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("dt_file_cid");
                });

            modelBuilder.Entity("CloudSpeed.Entities.FileJob", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<string>("Cid")
                        .HasColumnName("name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("JobId")
                        .HasColumnName("job_id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<byte>("Status")
                        .HasColumnName("status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("dt_file_job");
                });

            modelBuilder.Entity("CloudSpeed.Entities.FileMd5", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("DataKey")
                        .HasColumnName("data_key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("dt_file_md5");
                });

            modelBuilder.Entity("CloudSpeed.Entities.FileName", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1024);

                    b.HasKey("Id");

                    b.ToTable("dt_file_name");
                });

            modelBuilder.Entity("CloudSpeed.Entities.UploadLog", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<string>("AlipayFileKey")
                        .HasColumnName("alipay_file_key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("DataKey")
                        .HasColumnName("data_key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("Description")
                        .HasColumnName("description")
                        .HasColumnType("TEXT");

                    b.Property<string>("HashedPassword")
                        .HasColumnName("hashed_password")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.Property<string>("WxpayFileKey")
                        .HasColumnName("wxpay_file_key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.ToTable("dt_upload_log");
                });
#pragma warning restore 612, 618
        }
    }
}
