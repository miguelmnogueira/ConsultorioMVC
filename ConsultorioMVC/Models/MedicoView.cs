using System.ComponentModel.DataAnnotations;

namespace ConsultorioMVC.Models
{
    public class MedicoView
    {
        public int Id { get; set; }
        public String Nome { get; set; }
        public String Crm { get; set; }
        [Required] public int ConsultorioId { get; set; }
        public ConsultorioView? Consultorio { get; set; }
    }
}
