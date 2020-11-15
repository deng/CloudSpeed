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
                .HasAnnotation("ProductVersion", "3.1.9");

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

                    b.Property<long>("DealSize")
                        .HasColumnName("deal_size")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Error")
                        .HasColumnName("error")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2048);

                    b.Property<long>("PayloadSize")
                        .HasColumnName("payload_size")
                        .HasColumnType("INTEGER");

                    b.Property<string>("PieceCid")
                        .HasColumnName("piece_cid")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<long>("PieceSize")
                        .HasColumnName("piece_size")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Status")
                        .HasColumnName("status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("dt_file_cid");
                });

            modelBuilder.Entity("CloudSpeed.Entities.FileDeal", b =>
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

                    b.Property<string>("DealId")
                        .HasColumnName("deal_id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<string>("Error")
                        .HasColumnName("error")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2048);

                    b.Property<string>("Miner")
                        .HasColumnName("miner")
                        .HasColumnType("TEXT")
                        .HasMaxLength(18);

                    b.Property<string>("PieceCid")
                        .HasColumnName("piece_cid")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<long>("PieceSize")
                        .HasColumnName("piece_size")
                        .HasColumnType("INTEGER");

                    b.Property<byte>("Status")
                        .HasColumnName("status")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("dt_file_deal");
                });

            modelBuilder.Entity("CloudSpeed.Entities.FileGroup", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("GroupId")
                        .HasColumnName("group_id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("TEXT");

                    b.Property<string>("name")
                        .HasColumnName("name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1024);

                    b.HasKey("Id");

                    b.ToTable("dt_file_group");
                });

            modelBuilder.Entity("CloudSpeed.Entities.FileImport", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnName("id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<DateTime>("Created")
                        .HasColumnName("created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Error")
                        .HasColumnName("error")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2048);

                    b.Property<long>("Failed")
                        .HasColumnName("failed")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Path")
                        .HasColumnName("path")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2048);

                    b.Property<byte>("Status")
                        .HasColumnName("status")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Success")
                        .HasColumnName("success")
                        .HasColumnType("INTEGER");

                    b.Property<long>("Total")
                        .HasColumnName("total")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Updated")
                        .HasColumnName("updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("dt_file_import");
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

                    b.Property<string>("Error")
                        .HasColumnName("error")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2048);

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

                    b.Property<string>("Format")
                        .HasColumnName("format")
                        .HasColumnType("TEXT")
                        .HasMaxLength(128);

                    b.Property<string>("GroupId")
                        .HasColumnName("group_id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<string>("Name")
                        .HasColumnName("name")
                        .HasColumnType("TEXT")
                        .HasMaxLength(1024);

                    b.Property<long>("Size")
                        .HasColumnName("size")
                        .HasColumnType("INTEGER");

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

                    b.Property<string>("UserId")
                        .HasColumnName("user_id")
                        .HasColumnType("TEXT")
                        .HasMaxLength(36);

                    b.Property<string>("WxpayFileKey")
                        .HasColumnName("wxpay_file_key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(256);

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("dt_upload_log");
                });
#pragma warning restore 612, 618
        }
    }
}
