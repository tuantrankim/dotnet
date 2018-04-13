using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in both code and config file together.
     [GlobalErrorHandlerBehavior(typeof(GlobalErrorHandler))]
    public class EmployeeService : IEmployeeService
    {
        private static Dictionary<int, Employee> _employeeCollection = new Dictionary<int, Employee>();
        public Employee GetEmployee(int id)
        {
            LongRun().GetAwaiter().GetResult();
            if (_employeeCollection.ContainsKey(id)) return _employeeCollection[id];
            else
            {
                throw new Exception("Cannot find employee id " + id);
            }
        }

        public async Task<Employee> GetEmployeeAsyn(int id)
        {
            await LongRun();
            if (_employeeCollection.ContainsKey(id)) return _employeeCollection[id];
            else
            {
                throw new Exception("Cannot find employee id " + id);
            }
        }
        public void SaveEmployee(Employee employee)
        {
            try
            {
                LongRun().Wait();
                _employeeCollection.Add(employee.Id, employee);
                
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public async Task SaveEmployeeAsyn(Employee employee)
        {
            try
            {
                await LongRun();
                _employeeCollection.Add(employee.Id, employee);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

        public async Task LongRun()
        {
            await Task.Run(() => Thread.Sleep(3000));
        }
    }
}
