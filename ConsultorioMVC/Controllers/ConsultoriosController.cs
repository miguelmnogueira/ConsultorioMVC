using ConsultorioMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;


namespace ConsultorioMVC.Controllers
{
    public class ConsultoriosController : Controller
    {
        private readonly HttpClient _apiClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ConsultoriosController(IHttpClientFactory httpClientFactory)
        {
            _apiClient = httpClientFactory.CreateClient("ConsultorioApi");
            _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
        }

        public async Task<IActionResult> Index()
        {
            var res = await _apiClient.GetAsync("api/Consultorios");
            if (res.IsSuccessStatusCode)
            {
                var json = await res.Content.ReadAsStringAsync();
                var consultorios = JsonSerializer.Deserialize<List<ConsultorioView>>(json, _jsonOptions);
                return View(consultorios);
            }
            return View(new List<ConsultorioView>());
        }

        public async Task<IActionResult> CriarConsultorio()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CriarConsultorio(ConsultorioView p)
        {
            p.Uf = "a";
            p.Localidade = "a";
            p.Logradouro = "a";
            p.Bairro = "a";

            ModelState.Remove("Uf");
            ModelState.Remove("Localidade");
            ModelState.Remove("Logradouro");
            ModelState.Remove("Bairro");
            if (!ModelState.IsValid) return View(p);
            var content = new StringContent(JsonSerializer.Serialize(p), System.Text.Encoding.UTF8, "application/json");
            var resp = await _apiClient.PostAsync("api/Consultorios", content);
            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");
            ModelState.AddModelError("", "Erro ao cadastrar o paciente");
            return View(p);
        }


        public async Task<IActionResult> DeletarConsultorio(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Consultorios/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var consultorio = JsonSerializer.Deserialize<ConsultorioView>(json, _jsonOptions);
                return View(consultorio);
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Deletar(int id)
        {
            var resposta = await _apiClient.DeleteAsync($"api/Consultorios/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            return RedirectToAction(nameof(DeletarConsultorio), new { id });

        }
        public async Task<ActionResult> EditarConsultorio(int id)
        {
            var resposta = await _apiClient.GetAsync($"api/Consultorios/{id}");
            if (resposta.IsSuccessStatusCode)
            {
                var json = await resposta.Content.ReadAsStringAsync();
                var consultorio = JsonSerializer.Deserialize<ConsultorioView>(json, _jsonOptions);
                return View(consultorio);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditarConsultorio(ConsultorioView c, int id)
        {
            if (id != c.Id) return BadRequest();
            if (!ModelState.IsValid) return View(c);

            var content = new StringContent(JsonSerializer.Serialize(c), System.Text.Encoding.UTF8, "application/json");

            var resp = await _apiClient.PutAsync($"api/Consultorios/{id}", content);

            if (resp.IsSuccessStatusCode) return RedirectToAction("Index");

            ModelState.AddModelError("", "Erro no cadastro");

            return View(c);

        }
    }
}

