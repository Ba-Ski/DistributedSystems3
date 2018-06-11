using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;

namespace DistrSystems3
{
    public static class ServerConfigurator
    {
        public static void CreateServer(int port = 123, string uri = "RemoteServer")
        {
            var provider = new BinaryServerFormatterSinkProvider {TypeFilterLevel = TypeFilterLevel.Full};

            // Creating the IDictionary to set the port on the channel instance.
            IDictionary props = new Dictionary<string, int>();
            props["port"] = port; // This must match number on client

            // Pass the properties for the port setting and the server provider
            var mTcpChannel = new TcpChannel(props, null, provider);
            ChannelServices.RegisterChannel(mTcpChannel, false);

            // Create the server object for clients to connect to
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(Server),
                uri,
                WellKnownObjectMode.Singleton);

            Console.ReadLine();
        }
    }
}
