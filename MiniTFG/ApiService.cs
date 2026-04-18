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

        // Clase interna para deserialización flexible
        private class UserSkinDto
        {
            public int Id { get; set; }
            public int SkinId { get; set; }
        }

        // ------------------ IMÁGENES ------------------

        /// <summary>
        /// Descarga una imagen desde una URL del servidor (reescribiendo localhost para el emulador)
        /// y devuelve un ImageSource. Si el valor es un nombre de archivo local, lo devuelve directamente.
        /// </summary>
        public async Task<ImageSource> GetImageSourceAsync(string urlOrFile, string porDefecto = null)
        {
            if (string.IsNullOrWhiteSpace(urlOrFile))
                return porDefecto != null ? ImageSource.FromFile(porDefecto) : null;

            // Si no parece una URL, tratar como nombre de archivo local (ej: skins)
            if (!urlOrFile.Contains("://") && !urlOrFile.StartsWith('/'))
                return ImageSource.FromFile(urlOrFile);

            try
            {
                // Reescribir localhost → 10.0.2.2 para el emulador Android
                string url = urlOrFile;
#if DEBUG
                url = url.Replace("://localhost", "://10.0.2.2");
#endif
                // Descargar bytes con el HttpClient que tiene bypass SSL
                byte[] bytes;
                if (url.StartsWith('/'))
                    bytes = await _httpClient.GetByteArrayAsync(url.TrimStart('/'));
                else
                    bytes = await _httpClient.GetByteArrayAsync(url);

                // Guardar en archivo de caché para evitar que desaparezca con StreamImageSource
                string cacheFile = System.IO.Path.Combine(FileSystem.CacheDirectory, $"img_{Guid.NewGuid():N}.tmp");
                await File.WriteAllBytesAsync(cacheFile, bytes);
                return ImageSource.FromFile(cacheFile);
            }
            catch
            {
                return porDefecto != null ? ImageSource.FromFile(porDefecto) : null;
            }
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

        // ------------------ PASOS RECETA ------------------

        public async Task<List<PasoReceta>> GetPasosRecetaAsync(int recetaId)
        {
            var response = await _httpClient.GetAsync($"api/PasosReceta/receta/{recetaId}");
            if (!response.IsSuccessStatusCode)
                return new List<PasoReceta>();
            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<PasoReceta>>(json, _options) ?? new List<PasoReceta>();
        }

        public async Task<PasoReceta> PostPasoRecetaAsync(PasoReceta paso)
        {
            var response = await _httpClient.PostAsJsonAsync("api/PasosReceta", paso);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<PasoReceta>();
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

        // ------------------ SKINS ------------------

        // Obtener skins compradas por usuario (devuelve List<string> con SkinId)
        public async Task<List<string>> GetUserSkinsAsync(int usuarioId)
        {
            var res = await _httpClient.GetAsync($"api/Skins/user/{usuarioId}");
            if (!res.IsSuccessStatusCode) return new List<string>();
            var json = await res.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<string>>(json, _options);
        }

        public async Task<List<Skin>> GetSkinsAsync()
        {
            var response = await _httpClient.GetAsync("api/skins");

            if (!response.IsSuccessStatusCode)
                return new List<Skin>();

            var json = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<List<Skin>>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }

        // ==========================================
        // 2. OBTENER SKINS COMPRADAS POR UN USUARIO
        // ==========================================
        public async Task<List<int>> GetPurchasedUserSkinsAsync(int usuarioId)
        {
            var response = await _httpClient.GetAsync($"api/userskins/user/{usuarioId}");

            if (!response.IsSuccessStatusCode)
                return new List<int>();

            var json = await response.Content.ReadAsStringAsync();

            try
            {
                // 1. Intenta deserializar como array de números puros: [1, 2, 3]
                var result = JsonSerializer.Deserialize<List<int>>(json, _options);
                if (result != null)
                    return result;
            }
            catch (JsonException)
            {
                // No es un array de números, continúa
            }

            try
            {
                // 2. Intenta como array de objetos con propiedades "id" o "skinid": [{"id":1}, {"skinid":2}]
                var dtos = JsonSerializer.Deserialize<List<UserSkinDto>>(json, _options);
                if (dtos != null)
                {
                    return dtos
                        .Select(d => d.Id != 0 ? d.Id : d.SkinId)
                        .Where(id => id > 0)
                        .ToList();
                }
            }
            catch (JsonException)
            {
                // No es un array de objetos, continúa
            }

            try
            {
                // 3. Intenta como array de strings: ["1", "2", "3"]
                var stringIds = JsonSerializer.Deserialize<List<string>>(json, _options);
                if (stringIds != null && stringIds.Count > 0)
                {
                    return stringIds
                        .Where(s => int.TryParse(s, out _))
                        .Select(s => int.Parse(s))
                        .ToList();
                }
            }
            catch (JsonException)
            {
                // No es un array de strings
            }

            // Si llegamos aquí, no se pudo deserializar en ningún formato
            return new List<int>();
        }

        // ============================
        // 3. COMPRAR UNA SKIN
        // ============================
        public async Task<bool> PurchaseSkinAsync(int usuarioId, int skinId)
        {
            var response = await _httpClient.PostAsync($"api/userskins/purchase/{usuarioId}/{skinId}", null);
            return response.IsSuccessStatusCode;
        }

        // ============================
        // 4. ACTIVAR UNA SKIN
        // ============================
        public async Task<bool> ActivateSkinAsync(int usuarioId, int skinId, string tipo)
        {
            var response = await _httpClient.PostAsync($"api/userskins/activate/{usuarioId}/{skinId}/{tipo}", null);
            return response.IsSuccessStatusCode;
        }
    }
}
