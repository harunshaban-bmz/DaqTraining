using BMZ.Daq.Keysight970A;
using BMZ.Daq;
using BMZ.Instruments.Ports;
using BMZ.SCPI;
using BMZ.SCPI.IP;
using BMZ.TestSystemBase.Data.TestData;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using BMZ.PowerSupply;
using BMZ.PowerSupply.EA;
using BMZ.SCPI.UART;

namespace DaqTraining
{
    internal class CompositionRoot
    {

        public IDataProvider TestData => DIContainer.Resolve<IDataProvider>();

        public IUnityContainer DIContainer
        {
            get;
            private set;
        }

        private static CompositionRoot SingletonInstance;

        private CompositionRoot()
        {
            DIContainer = new UnityContainer();
        }

        public static CompositionRoot Instance
        {
            get
            {
                if (SingletonInstance == null)
                {
                    SingletonInstance = new CompositionRoot();
                }

                return SingletonInstance;
            }
        }

        public void Init()
        {
            ConfigureUnityContainer();
        }

        private void ConfigureUnityContainer()
        {
            DIContainer.RegisterType<ISCPIFrame>(
                "Daq",
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(c => { return new SCPIFrameIp(new PortDataIp { Address = "169.254.60.72", Port = 5025 }); }));
            DIContainer.RegisterType<IDaq, DaqKeysight970A>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<ISCPIFrame>("Daq")));

            //PS();
            //DIContainer.RegisterType<ISCPIFrame>("EA", new InjectionFactory(c => { return new SCPIFrameUart(c.Resolve<IPortDetection>("Instruments").Ports.Single(x => x.InstrumentType == "EA")); }));
            DIContainer.RegisterType<ISCPIFrame>(
                "EA",
                new ContainerControlledLifetimeManager(),
                new InjectionFactory(x => { return new SCPIFrameUart(new PortDataUart { ComPort = "COM6", BaudRate = 9600 }); }));
            DIContainer.RegisterType<IPowerSupply, PowerSupplyEA>(new ContainerControlledLifetimeManager(), new InjectionConstructor(new ResolvedParameter<ISCPIFrame>("EA")));
        }
    }
}
