using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

namespace MiniTFG
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };


        public ApiService()
        {
            var handler = new HttpClientHandler();
            
            #if DEBUG
                handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                
                _httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://10.0.2.2:7257/")
                };
            #else
                _httpClient = new HttpClient(handler)
                {
                    BaseAddress = new Uri("https://localhost:7257/")
                };
            #endif
        }

        // ------------------ USUARIOS ------------------

        public async Task<Usuario[]> GetUsuariosAsync()
        {
            return await _httpClient.GetFromJsonAsync<Usuario[]>("api/usuarios");
        }

        public async Task<Usuario> GetUsuarioByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Usuario>($"api/usuarios/{id}");
        }

        public async Task<Usuario> PostUsuarioAsync(Usuario usuario)
        {
            var response = await _httpClient.PostAsJsonAsync("api/usuarios", usuario);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Usuario>();
        }

        public async Task<Usuario> LoginAsync(string correo, string contrasena)
        {
            var request = new { correo = correo, contrasena = contrasena };

            var response = await _httpClient.PostAsJsonAsync("api/usuarios/login", request);

            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return null;

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<Usuario>();
        }


        // ------------------ RECETAS ------------------

        public async Task<Receta[]> GetRecetasAsync()
        {
            return await _httpClient.GetFromJsonAsync<Receta[]>("api/recetas");
        }

        public async Task<Receta> GetRecetaByIdAsync(int id)
        {
            return await _httpClient.GetFromJsonAsync<Receta>($"api/recetas/{id}");
        }

        public async Task<Receta> PostRecetaAsync(Receta receta)
        {
            var response = await _httpClient.PostAsJsonAsync("api/recetas", receta);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Receta>();
        }

        public async Task<bool> DeleteRecetaAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/recetas/{id}");
            return response.IsSuccessStatusCode;
        }

        // ------------------ LIKES ------------------

        public async Task<Like> PostLikeAsync(Like like)
        {
            var response = await _httpClient.PostAsJsonAsync("api/likes", like);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<Like>();
        }
        
        public async Task<IEnumerable<Like>> GetLikesUsuarioAsync(int usuarioId)
        {
            var response = await _httpClient.GetAsync($"api/Likes/usuario/{usuarioId}");

            if (!response.IsSuccessStatusCode)
                return Enumerable.Empty<Like>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<IEnumerable<Like>>(json, _options)
                   ?? Enumerable.Empty<Like>();
        }


        public async Task<bool> DeleteLikeAsync(int usuarioId, int recetaId)
        {
            var response = await _httpClient.DeleteAsync($"api/Likes?usuarioId={usuarioId}&recetaId={recetaId}");
            return response.IsSuccessStatusCode;
        }

        // ------------------ VALORACIONES ------------------

        public async Task<bool> PostValoracionAsync(Valoracion valoracion)
        {
            var json = JsonSerializer.Serialize(valoracion);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("api/Valoraciones", content);

            return response.IsSuccessStatusCode;
        }

        public async Task<List<Valoracion>> GetValoracionesAsync(int usuarioValoradoId)
        {
            var response = await _httpClient.GetAsync($"api/Valoraciones/{usuarioValoradoId}");

            if (!response.IsSuccessStatusCode)
                return new List<Valoracion>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Valoracion>>(json);
        }

        public async Task<List<Valoracion>> GetValoracionesPorUsuarioAsync(int usuarioQueValoraId)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/Valoraciones/usuario/{usuarioQueValoraId}");
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<Valoracion>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            catch
            {
                return null;
            }
        }

    }
}
