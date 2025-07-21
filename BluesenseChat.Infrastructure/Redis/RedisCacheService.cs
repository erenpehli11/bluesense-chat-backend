﻿using BluesenseChat.Application.Interfaces.ServiceInterfaces;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System.Text.Json;

public class RedisCacheService : IRedisCacheService
{
    private readonly IDatabase _db;

    public RedisCacheService(IConfiguration configuration)
    {
        var redis = ConnectionMultiplexer.Connect(configuration["Redis:ConnectionString"]);
        _db = redis.GetDatabase();
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiry = null)
    {
        var json = JsonSerializer.Serialize(value);
        await _db.StringSetAsync(key, json, expiry);
    }

    public async Task<T?> GetAsync<T>(string key)
    {
        var json = await _db.StringGetAsync(key);
        if (json.IsNullOrEmpty) return default;
        return JsonSerializer.Deserialize<T>(json!);
    }

    public async Task RemoveAsync(string key)
    {
        await _db.KeyDeleteAsync(key);
    }
}
