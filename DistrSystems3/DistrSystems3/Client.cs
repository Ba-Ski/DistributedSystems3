using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Threading.Tasks;

namespace DistrSystems3
{
    public class Client
    {
        private const int PollDelay = 5000;
        private bool _stop;
        private readonly List<string> _tasks;

        public Client()
        {
            _stop = false;
            _tasks = new List<string>();
        }

        public void RunAdvancedClient()
        {
            var mRemoteObject = GetServerInstance();
            var guid = Guid.NewGuid().ToString();
            var t = Task.Run(() => GetTasks(guid, mRemoteObject));
            Console.WriteLine($"your id: {guid}");
            Console.WriteLine(" type 'c' for connect");
            Console.WriteLine(" type 'd' for disconnect");
            Console.WriteLine(" type 'q' for quit");

            Console.WriteLine(" type 'a' for add task");
            Console.WriteLine(" type 'b' for balance the load");


            while (!_stop)
            {
                var result = Console.ReadLine();
                var cmd = result?[0] ?? '?';
                switch (cmd)
                {
                    case 'c':
                        mRemoteObject.Connect(guid);
                        break;
                    case 'd':
                        mRemoteObject.Disconnect(guid);
                        break;
                    case 'a':
                        mRemoteObject.AddTask(new List<string>() {Guid.NewGuid().ToString()});
                        break;
                    case 'q':
                        mRemoteObject.Disconnect(guid);
                        _stop = true;
                        break;
                    case 'b':
                        mRemoteObject.BalanceLoad();
                        break;
                    default:
                        break;
                }
            }

        }

        public void RunDummyClient()
        {
            var mRemoteObject = GetServerInstance();
            var guid = Guid.NewGuid().ToString();
            var task = Task.Run(() => GetTasks(guid, mRemoteObject));
            
            Console.WriteLine($"your id: {guid}");
            Console.WriteLine(" type 'c' for connect");
            Console.WriteLine(" type 'd' for disconnect");
            Console.WriteLine(" type 'q' for quit");

            while (!_stop)
            {
                var result = Console.ReadLine();
                var cmd = result?[0] ?? '?';

                switch (cmd)
                {
                    case 'c':
                        mRemoteObject.Connect(guid);
                        break;
                    case 'd':
                        mRemoteObject.Disconnect(guid);
                        break;
                    case 'q':
                        mRemoteObject.Disconnect(guid);
                        _stop = true;
                        break;
                    default:
                        break;
                }
            }

        }

        private static ICallsToServer GetServerInstance()
        {
            Console.WriteLine("type host");
            var host = Console.ReadLine();

            var mTcpChan = new TcpChannel(0);
            ChannelServices.RegisterChannel(mTcpChan, false);

            // Create the object for calling into the server
            var mRemoteObject = (ICallsToServer)
                Activator.GetObject(typeof(ICallsToServer),
                    $"tcp://{host}:123/RemoteServer");
            return mRemoteObject;
        }

        private void GetTasks(string id, ICallsToServer mRemoteObject)
        {
            while (true)
            {
                Task.Delay(PollDelay);
                
                var tasksFromServer = mRemoteObject.GetTasks(id);
                var addedTasks = tasksFromServer.Except(_tasks).ToList();
                _tasks.AddRange(addedTasks);
                
                OutputList("added tasks", addedTasks);
                var deletedTasks = _tasks.Except(tasksFromServer).ToList();
                foreach (var task in deletedTasks)
                {
                    _tasks.Remove(task);
                }

                OutputList("deleted tasks", deletedTasks);
            }
        }

        private static void OutputList(string message, IEnumerable<string> list)
        {
            var enumerable = list as string[] ?? list.ToArray();
            if (!enumerable.Any())
            {
                return;
            }

            Console.WriteLine(message + $" at {DateTime.Now}");
            foreach (var x in enumerable)
            {
                Console.WriteLine(x);
            }

            Console.WriteLine("-----------");
        }
    }
}
