
namespace ForeingExchange.Infrastucture
{
    using System;
    using ViewModels;
    public class InstanceLocator
    {
        public MainViewModel Main
        {
            get;
            set;
        }
        public InstanceLocator()
        {
            Main = new MainViewModel();
        }
    }
}
