﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Spellbox.Model;

#nullable disable

namespace Spellbox.Migrations
{
    [DbContext(typeof(CollectionDbContext))]
    [Migration("20240718204126_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "9.0.0-preview.6.24327.4");

            modelBuilder.Entity("Spellbox.Model.Binder", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Binders");
                });

            modelBuilder.Entity("Spellbox.Model.Card", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int?>("BinderId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("CollectorNumber")
                        .HasColumnType("TEXT");

                    b.Property<string>("Finish")
                        .HasColumnType("TEXT");

                    b.Property<string>("Image")
                        .HasColumnType("TEXT");

                    b.Property<string>("Language")
                        .HasColumnType("TEXT");

                    b.Property<string>("OracleId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<string>("SetId")
                        .HasColumnType("TEXT");

                    b.Property<string>("SetName")
                        .HasColumnType("TEXT");

                    b.Property<int?>("SnapshotId")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SnapshotId1")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SnapshotId2")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SnapshotId3")
                        .HasColumnType("INTEGER");

                    b.Property<int?>("SnapshotId4")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Thumb")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("BinderId");

                    b.HasIndex("SnapshotId");

                    b.HasIndex("SnapshotId1");

                    b.HasIndex("SnapshotId2");

                    b.HasIndex("SnapshotId3");

                    b.HasIndex("SnapshotId4");

                    b.ToTable("Card");
                });

            modelBuilder.Entity("Spellbox.Model.Deck", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Created")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<string>("Updated")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Decks");
                });

            modelBuilder.Entity("Spellbox.Model.Snapshot", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Created")
                        .HasColumnType("TEXT");

                    b.Property<int?>("DeckId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("DeckId");

                    b.ToTable("Snapshot");
                });

            modelBuilder.Entity("Spellbox.Model.Card", b =>
                {
                    b.HasOne("Spellbox.Model.Binder", null)
                        .WithMany("Cards")
                        .HasForeignKey("BinderId");

                    b.HasOne("Spellbox.Model.Snapshot", null)
                        .WithMany("Commanders")
                        .HasForeignKey("SnapshotId");

                    b.HasOne("Spellbox.Model.Snapshot", null)
                        .WithMany("Companions")
                        .HasForeignKey("SnapshotId1");

                    b.HasOne("Spellbox.Model.Snapshot", null)
                        .WithMany("Mainboard")
                        .HasForeignKey("SnapshotId2");

                    b.HasOne("Spellbox.Model.Snapshot", null)
                        .WithMany("Maybeboard")
                        .HasForeignKey("SnapshotId3");

                    b.HasOne("Spellbox.Model.Snapshot", null)
                        .WithMany("Sideboard")
                        .HasForeignKey("SnapshotId4");
                });

            modelBuilder.Entity("Spellbox.Model.Snapshot", b =>
                {
                    b.HasOne("Spellbox.Model.Deck", null)
                        .WithMany("Snapshots")
                        .HasForeignKey("DeckId");
                });

            modelBuilder.Entity("Spellbox.Model.Binder", b =>
                {
                    b.Navigation("Cards");
                });

            modelBuilder.Entity("Spellbox.Model.Deck", b =>
                {
                    b.Navigation("Snapshots");
                });

            modelBuilder.Entity("Spellbox.Model.Snapshot", b =>
                {
                    b.Navigation("Commanders");

                    b.Navigation("Companions");

                    b.Navigation("Mainboard");

                    b.Navigation("Maybeboard");

                    b.Navigation("Sideboard");
                });
#pragma warning restore 612, 618
        }
    }
}