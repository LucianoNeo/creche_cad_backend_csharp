﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using creche_cad.Data.Context;

#nullable disable

namespace creche_cad.Data.Migrations
{
    [DbContext(typeof(CrecheDbContext))]
    [Migration("20240504121416_Cria tabelas iniciais")]
    partial class Criatabelasiniciais
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "6.0.29");

            modelBuilder.Entity("creche_cad.Domain.Entities.Aluno", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataAtualizacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataNascimento")
                        .HasColumnType("TEXT");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NomeMae")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("NomePai")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TurmaId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("TurmaId");

                    b.ToTable("Aluno", (string)null);
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Documento", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("AlunoId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("TEXT");

                    b.Property<byte[]>("DocumentoBytes")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<string>("NomeArquivo")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("ProfessorId")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("AlunoId");

                    b.HasIndex("ProfessorId");

                    b.ToTable("Documento", (string)null);
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Professor", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("CPF")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("CarteiraTrabalho")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataAdmissao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataAtualizacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataDemissao")
                        .HasColumnType("TEXT");

                    b.Property<string>("Endereco")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("RG")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TelefoneCelular")
                        .HasColumnType("TEXT");

                    b.Property<string>("TelefonePrincipal")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("TelefoneSecundario")
                        .HasColumnType("TEXT");

                    b.Property<string>("Titulo")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Professor", (string)null);
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Turma", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataAtualizacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("DataCriacao")
                        .HasColumnType("TEXT");

                    b.Property<DateTime?>("DataExclusao")
                        .HasColumnType("TEXT");

                    b.Property<decimal?>("Metragem")
                        .HasColumnType("TEXT");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Turma", (string)null);
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Aluno", b =>
                {
                    b.HasOne("creche_cad.Domain.Entities.Turma", "Turma")
                        .WithMany("Alunos")
                        .HasForeignKey("TurmaId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Turma");
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Documento", b =>
                {
                    b.HasOne("creche_cad.Domain.Entities.Aluno", "Aluno")
                        .WithMany("Documentos")
                        .HasForeignKey("AlunoId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("creche_cad.Domain.Entities.Professor", "Professor")
                        .WithMany("Documentos")
                        .HasForeignKey("ProfessorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Aluno");

                    b.Navigation("Professor");
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Aluno", b =>
                {
                    b.Navigation("Documentos");
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Professor", b =>
                {
                    b.Navigation("Documentos");
                });

            modelBuilder.Entity("creche_cad.Domain.Entities.Turma", b =>
                {
                    b.Navigation("Alunos");
                });
#pragma warning restore 612, 618
        }
    }
}
