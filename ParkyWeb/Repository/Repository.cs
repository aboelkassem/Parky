﻿using Newtonsoft.Json;
using ParkyWeb.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ParkyWeb.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly IHttpClientFactory _clientFactory;

        public Repository(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<bool> CreateAsync(string url, T objToCreate, string token = "")
        {
            // make a request
            var request = new HttpRequestMessage(HttpMethod.Post, url);
            if (objToCreate != null) // body or payload of the request
                request.Content = new StringContent(JsonConvert.SerializeObject(objToCreate), Encoding.UTF8, "application/json");
            else
                return false;

            // make the request and get the response
            var client = _clientFactory.CreateClient();
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer",token);
            }

            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.Created)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<bool> DeleteAsync(string url, int Id, string token = "")
        {
            // make a request
            var request = new HttpRequestMessage(HttpMethod.Delete, url+Id);

            // make the request and get the response
            var client = _clientFactory.CreateClient();
            // if there is token send to authorize
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }

            return false;
            
        }

        public async Task<IEnumerable<T>> GetAllAsync(string url, string token = "")
        {
            // make a request
            var request = new HttpRequestMessage(HttpMethod.Get, url);

            // make the request and get the response
            var client = _clientFactory.CreateClient();
            // if there is token send to authorize
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonString);
            }

            return null;
            
        }

        public async Task<T> GetAsync(string url, int Id, string token = "")
        {
            // make a request
            var request = new HttpRequestMessage(HttpMethod.Get, url+Id);

            // make the request and get the response
            var client = _clientFactory.CreateClient();
            // if there is token send to authorize
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(jsonString);
            }

            return null;
        }

        public async Task<bool> UpdateAsync(string url, T objToUpdate, string token = "")
        {
            // make a request
            var request = new HttpRequestMessage(HttpMethod.Patch, url);
            if (objToUpdate != null) // body or payload of the request
                request.Content = new StringContent(JsonConvert.SerializeObject(objToUpdate), Encoding.UTF8, "application/json");
            else
                return false;

            // make the request and get the response
            var client = _clientFactory.CreateClient();
            // if there is token send to authorize
            if (token != null && token.Length != 0)
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
