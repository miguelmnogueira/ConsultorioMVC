using ConsultorioMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace ConsultorioMVC.Controllers
{
    public class ConsultasController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConsultasController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("ConsultorioApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("api/Consultas");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var consultas = System.Text.Json.JsonSerializer.Deserialize<List<ConsultaView>>(json, _jsonOptions);
                return View(consultas);
            }
            return View(new List<ConsultaView>());
        }

        public async Task<IActionResult> CriarConsulta()
        {
            var pacienteResponse = await _apiClient.GetAsync("api/Pacientes");
            var medicoResponse = await _apiClient.GetAsync("api/Medicos");

            if (!pacienteResponse.IsSuccessStatusCode)
            {
                return View();
            }

            var pacienteJson = await pacienteResponse.Content.ReadAsStringAsync();
            var medicoJson = await medicoResponse.Content.ReadAsStringAsync();

            var pacientes = JsonConvert.DeserializeObject<List<PacienteView>>(pacienteJson);
            var medicos = JsonConvert.DeserializeObject<List<PacienteView>>(medicoJson);

            ViewBag.Pacientes = pacientes;
            ViewBag.Medicos = medicos;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarConsulta(ConsultaView c)
        {
            if (!ModelState.IsValid) return View(c);
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(c), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("api/Consultas", content);
            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");
            ModelState.AddModelError("", "Erro ao cadastrar a consulta");
            var response = await _apiClient.GetAsync("api/Consultorios");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();

            var consultorios = JsonConvert.DeserializeObject<List<ConsultorioView>>(json);

            ViewBag.Consultorios = consultorios;
            return View(c);
        }


        public async Task<IActionResult> DeletarConsulta(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Consultas/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var medico = System.Text.Json.JsonSerializer.Deserialize<ConsultaView>(json, _jsonOptions);
                return View(medico);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/Consultas/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarConsulta), new { id });

        }
        public async Task<ActionResult> EditarConsulta(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Consultas/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var consulta = System.Text.Json.JsonSerializer.Deserialize<ConsultaView>(json, _jsonOptions);
                return View(consulta);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditarConsulta(ConsultaView     m, int id)
        {
            if (id != m.Id) return BadRequest();
            if (!ModelState.IsValid) return View(m);

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(m), System.Text.Encoding.UTF8, "application/json");

            var resp = await _apiClient.PutAsync($"api/Consultas/{id}", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro no cadastro");

            return View(m);

        }
    }
}
