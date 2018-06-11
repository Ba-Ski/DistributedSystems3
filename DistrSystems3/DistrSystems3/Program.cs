namespace DistrSystems3
{
    class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                ServerConfigurator.CreateServer();
            }

            else
            {
                switch (args[0].ToLower())
                {
                    case "c":
                        new Client().RunDummyClient();
                        break;
                    case "s":
                        ServerConfigurator.CreateServer();
                        break;
                    case "cm":
                        new Client().RunAdvancedClient();
                        break;
                    default:
                        ServerConfigurator.CreateServer();
                        break;
                }
            }
        }
    }
}
