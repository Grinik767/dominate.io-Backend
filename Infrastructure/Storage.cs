using System.Collections.Concurrent;
using Domain;

namespace Infrastructure;

public class Storage
{
    private readonly ConcurrentDictionary<string, Lobby> _data = new();
    
    
}