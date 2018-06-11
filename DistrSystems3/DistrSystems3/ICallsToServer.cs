using System.Collections.Generic;

namespace DistrSystems3
{

    public interface ICallsToServer
    {
        void Connect(string id);
        void Disconnect(string id);
        List<string> GetTasks(string id);
        void AddTask(IEnumerable<string> ids);
        void BalanceLoad();
    }
}
