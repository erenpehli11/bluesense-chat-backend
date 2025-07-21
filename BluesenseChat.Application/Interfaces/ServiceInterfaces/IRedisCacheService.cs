﻿using System;
using System.Threading.Tasks;

namespace BluesenseChat.Application.Interfaces.ServiceInterfaces
{
    public interface IRedisCacheService
    {
        Task SetAsync<T>(string key, T value, TimeSpan? expiry = null);
        Task<T?> GetAsync<T>(string key);
        Task RemoveAsync(string key);
    }
}
