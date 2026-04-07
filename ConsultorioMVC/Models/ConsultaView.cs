using System.ComponentModel.DataAnnotations;

namespace ConsultorioMVC.Models
{
    public class ConsultaView
    {
            public int Id { get; set; }
            [Required] public int PacienteId { get; set; }
            public PacienteView? Paciente { get; set; }
            [Required] public int MedicoId { get; set; }
            public MedicoView? Medico { get; set; }
            public DateTime DataHora { get; set; }
            public string Observacoes { get; set; }


}
}
