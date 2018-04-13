using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in both code and config file together.
     
    public class EmployeeService : IEmployeeService
    {
        private static Dictionary<int, Employee> _employeeCollection = new Dictionary<int, Employee>();
        public Employee GetEmployee(int id)
        {
            if (_employeeCollection.ContainsKey(id)) return _employeeCollection[id];
            else return null;
        }

        public void SaveEmployee(Employee employee)
        {
            _employeeCollection.Add(employee.Id, employee);
        }
    }
}
