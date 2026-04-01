using ConsultorioMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace ConsultorioMVC.Controllers
{
    public class PacientesController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public PacientesController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("ConsultorioApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("api/Pacientes");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var pacientes = JsonSerializer.Deserialize<List<PacienteView>>(json, _jsonOptions);
                return View(pacientes);
            }
            return View(new List<PacienteView>());
        }

        public async Task<IActionResult> CriarPaciente(PacienteView p)
        {
            if (!ModelState.IsValid) return View(p);
            var content = new StringContent(JsonSerializer.Serialize(p), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("api/Pacientes", content);
            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");
            ModelState.AddModelError("", "Erro ao cadastrar o paciente");
            return View(p);
        }


        public async Task<IActionResult> DeletarPaciente(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Pacientes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var paciente = JsonSerializer.Deserialize<PacienteView>(json, _jsonOptions);
                return View(paciente);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/Pacientes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarPaciente), new { id });

        }
        public async Task<ActionResult> EditarPaciente(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Pacientes/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var paciente = JsonSerializer.Deserialize<PacienteView>(json, _jsonOptions);
                return View(paciente);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditarPaciente(PacienteView p, int id)
        {
            if (id != p.Id) return BadRequest();
            if (!ModelState.IsValid) return View(p);

            var content = new StringContent(JsonSerializer.Serialize(p), System.Text.Encoding.UTF8, "application/json");

            var resp = await _apiClient.PutAsync($"api/Pacientes/{id}", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro no cadastro");

            return View(p);

        }
    }
}
