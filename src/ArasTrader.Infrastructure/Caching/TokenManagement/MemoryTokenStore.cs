using ArasTrader.Application.Interfaces;
using ArasTrader.Application.Models;
using Microsoft.Extensions.Caching.Memory;

namespace ArasTrader.Infrastructure.Caching.TokenManagement
{
    internal class MemoryTokenStore : ITokenStore
    {
        private const string CacheKey = "ARAS_TOKEN";
        private readonly IMemoryCache _cache;

        public MemoryTokenStore(IMemoryCache cache)
        {
            _cache = cache;
        }
        public TokenState? Get()
        {
            return _cache.Get<TokenState>(CacheKey);
        }
        public void Save(TokenState token)
        {
            _cache.Set(CacheKey, token);
        }

        public void Clear()
        {
            _cache.Remove(CacheKey);
        }
    }
}
