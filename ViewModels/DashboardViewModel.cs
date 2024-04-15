using System;
using System.Collections;
using BusManagementSystem.Models;

namespace BusManagementSystem.ViewModels
{
    public class DashboardViewModel : IEnumerable
    {
        public List<Driver> Drivers { get; set; }

        public IEnumerator GetEnumerator()
        {
            throw new NotImplementedException();
        }
    }
}