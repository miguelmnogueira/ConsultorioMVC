using ConsultorioMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace ConsultorioMVC.Controllers
{
    public class MedicosController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public MedicosController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("ConsultorioApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }
        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("api/Medicos");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var medicos = System.Text.Json.JsonSerializer.Deserialize<List<MedicoView>>(json, _jsonOptions);
                return View(medicos);
            }
            return View(new List<MedicoView>());
        }

        public async Task<IActionResult> CriarMedico()
        {
            var response = await _apiClient.GetAsync("api/Consultorios");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();

            var consultorios = JsonConvert.DeserializeObject<List<ConsultorioView>>(json);

            ViewBag.Consultorios = consultorios;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarMedico(MedicoView m)
        {
            if (!ModelState.IsValid) return View(m);
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(m), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("api/Medicos", content);
            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");
            ModelState.AddModelError("", "Erro ao cadastrar o médico");
            var response = await _apiClient.GetAsync("api/Consultorios");

            if (!response.IsSuccessStatusCode)
            {
                return View();
            }

            var json = await response.Content.ReadAsStringAsync();

            var consultorios = JsonConvert.DeserializeObject<List<ConsultorioView>>(json);

            ViewBag.Consultorios = consultorios;
            return View(m);
        }


        public async Task<IActionResult> DeletarMedico(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Medicos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var medico = System.Text.Json.JsonSerializer.Deserialize<MedicoView>(json, _jsonOptions);
                return View(medico);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/Medicos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarMedico), new { id });

        }
        public async Task<ActionResult> EditarMedico(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Medicos/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var medico = System.Text.Json.JsonSerializer.Deserialize<MedicoView>(json, _jsonOptions);
                return View(medico);
            }
            return NotFound();
        }
        [HttpPost]
        public async Task<IActionResult> EditarMedico(MedicoView m, int id)
        {
            if (id != m.Id) return BadRequest();
            if (!ModelState.IsValid) return View(m);

            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(m), System.Text.Encoding.UTF8, "application/json");

            var resp = await _apiClient.PutAsync($"api/Medicos/{id}", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro no cadastro");

            return View(m);

        }
    }
}
