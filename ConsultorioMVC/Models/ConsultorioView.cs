using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace ConsultorioMVC.Models
{
    public class ConsultorioView
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cep { get; set; }
        [BindNever]
        public string? Logradouro { get; set; }
        [BindNever]
        public string? Bairro { get; set; }
        [BindNever]
        public string? Uf { get; set; }
        [BindNever]
        public string? Localidade { get; set; }
        public string Numero { get; set; }
    }
}
