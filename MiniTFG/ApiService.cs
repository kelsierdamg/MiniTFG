using Microsoft.Maui.Controls.Shapes;
using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;

namespace MiniTFG
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;

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

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Usuario>>("api/usuarios");
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

        public async Task<List<Receta>> GetRecetasAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<Receta>>("api/recetas");
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

        public async Task<bool> DeleteLikeAsync(int id)
        {
            var response = await _httpClient.DeleteAsync($"api/likes/{id}");
            return response.IsSuccessStatusCode;
        }
    }
}
