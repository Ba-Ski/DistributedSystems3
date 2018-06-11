using System;
using System.Collections.Generic;
using System.Linq;

namespace DistrSystems3
{
    internal class Server : MarshalByRefObject, ICallsToServer
    {
        private readonly Dictionary<string,List<string>> _clients;
        private readonly List<string> _freeTasks;
        
        public Server()
        {
            _clients = new Dictionary<string, List<string>>();
            _freeTasks = new List<string>();
        }

        public void Connect(string id)
        {
            lock (_clients)
            {
                if (!_clients.ContainsKey(id))
                {
                    _clients.Add(id,new List<string>());
                }
            }
        }

        public void Disconnect(string id)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(id))
                {
                    var list = new string[_clients[id].Count];
                    _clients[id].CopyTo(list);
                    _clients.Remove(id);
                    AddTask(list);
                }
            }
        }

        public List<string> GetTasks(string id)
        {
            lock (_clients)
            {
                if (_clients.ContainsKey(id))
                {
                    return _clients[id];
                }
                else
                {
                    return new List<string>();
                }
            }
        }

        public void AddTask(IEnumerable<string> ids)
        {
            lock (_freeTasks)
            {
                _freeTasks.AddRange(ids);
            }
            DistributeTasks();
        }

        private void DistributeTasks()
        {
            lock (_clients)
            {
                var minTaskClientId = _clients
                    .OrderBy(c => c.Value.Count)
                    .Select(c => c.Key);

                AssignTasksToClients(minTaskClientId);
            }
        }

        private void AssignTasksToClients(IEnumerable<string> minTaskClientId)
        {
            foreach (var id in minTaskClientId)
            {
                lock (_freeTasks)
                {
                    if (!_freeTasks.Any())
                    {
                        return;
                    }

                    _clients[id].Add(_freeTasks.First());
                    _freeTasks.Remove(_freeTasks.First());
                }
            }
        }

        public void BalanceLoad()
        {
            
            lock (_clients)
            {
                foreach(var client in _clients)
                {
                    lock (_freeTasks)
                    {
                        _freeTasks.AddRange(client.Value);
                        client.Value.Clear();
                    }
                }
            }
            DistributeTasks();
        }
    }
}
