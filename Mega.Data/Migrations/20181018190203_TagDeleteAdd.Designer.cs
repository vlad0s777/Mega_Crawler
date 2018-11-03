﻿// <auto-generated />
using System;
using Mega.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Mega.Data.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20181018190203_TagDeleteAdd")]
    partial class TagDeleteAdd
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.SerialColumn)
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            modelBuilder.Entity("Mega.Domain.Article", b =>
                {
                    b.Property<int>("ArticleId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateCreate");

                    b.Property<string>("Head");

                    b.Property<int>("OuterArticleId");

                    b.Property<string>("Text");

                    b.HasKey("ArticleId");

                    b.HasIndex("OuterArticleId")
                        .IsUnique();

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Mega.Domain.ArticleTag", b =>
                {
                    b.Property<int>("ArticleId");

                    b.Property<int>("TagId");

                    b.HasKey("ArticleId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("ArticleTag");
                });

            modelBuilder.Entity("Mega.Domain.Tag", b =>
                {
                    b.Property<int>("TagId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Name");

                    b.Property<string>("TagKey");

                    b.HasKey("TagId");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("Mega.Domain.TagDelete", b =>
                {
                    b.Property<int>("TagDeleteId")
                        .ValueGeneratedOnAdd();

                    b.Property<DateTime>("DateDelete");

                    b.Property<int>("TagId");

                    b.HasKey("TagDeleteId");

                    b.HasIndex("TagId")
                        .IsUnique();

                    b.ToTable("TagsDelete");
                });

            modelBuilder.Entity("Mega.Domain.ArticleTag", b =>
                {
                    b.HasOne("Mega.Domain.Article", "Article")
                        .WithMany("ArticleTag")
                        .HasForeignKey("ArticleId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("Mega.Domain.Tag", "Tag")
                        .WithMany("ArticleTag")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Mega.Domain.TagDelete", b =>
                {
                    b.HasOne("Mega.Domain.Tag", "Tag")
                        .WithOne("TagDelete")
                        .HasForeignKey("Mega.Domain.TagDelete", "TagId")
                        .OnDelete(DeleteBehavior.Restrict);
                });
#pragma warning restore 612, 618
        }
    }
}
