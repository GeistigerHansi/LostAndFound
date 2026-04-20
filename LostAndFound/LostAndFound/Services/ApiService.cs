using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using LostAndFound.Models;

namespace LostAndFound.Services
{
    public class ApiService
    {
        private readonly HttpClient _http;

        public ApiService(string baseAddress)
        {
            _http = new HttpClient { BaseAddress = new Uri(baseAddress) };
        }

        public async Task<List<ItemModel>> GetItemsAsync()
        {
            return await _http.GetFromJsonAsync<List<ItemModel>>("api/items") ?? new List<ItemModel>();
        }

        public async Task<ItemModel?> AddItemAsync(ItemModel item)
        {
            var res = await _http.PostAsJsonAsync("api/items", item);
            if (!res.IsSuccessStatusCode) return null;
            return await res.Content.ReadFromJsonAsync<ItemModel>();
        }

        public async Task<bool> UpdateItemAsync(ItemModel item)
        {
            var res = await _http.PutAsJsonAsync($"api/items/{item.Id}", item);
            return res.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            var res = await _http.DeleteAsync($"api/items/{id}");
            return res.IsSuccessStatusCode;
        }
    }
}
