﻿namespace creche_cad.Domain.Entities
{
    public class Professor
    {
        public Guid Id { get; set; }
        public string Nome { get; set; }
        public string RG { get; set; }
        public string CPF { get; set; }
        public string Endereco { get; set; }
        public string TelefonePrincipal { get; set; }
        public string? TelefoneCelular { get; set; }
        public string? TelefoneSecundario { get; set; }
        public string? Titulo { get; set; }
        public string? CarteiraTrabalho { get; set; }
        public DateTime DataAdmissao { get; set; }
        public DateTime? DataDemissao { get; set; }
        public DateTime DataCriacao { get; set; }
        public DateTime? DataAtualizacao { get; set; }
        public virtual ICollection<Documento>? Documentos { get; set; }
    }
}