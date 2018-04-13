using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Transactions;

namespace WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "EmployeeService" in both code and config file together.
     [GlobalErrorHandlerBehavior(typeof(GlobalErrorHandler))]
    public class EmployeeService : IEmployeeService
    {
        private static Dictionary<int, Employee> _employeeCollection = new Dictionary<int, Employee>();
        public Employee GetEmployee(int id)
        {
            if (_employeeCollection.ContainsKey(id)) return _employeeCollection[id];
            else
            {
                throw new Exception("Cannot find employee id " + id);
            }
        }

         [OperationBehavior(TransactionScopeRequired = false)]
        public void SaveEmployee(Employee employee)
        {
            try
            {
                _employeeCollection.Add(employee.Id, employee);
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

         [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = false)]
        public void SaveStep1(Employee employee)
        {
            try
            {
                Transaction currentTx = Transaction.Current;
                Console.WriteLine("Save step 1 is in tx {0}", currentTx.TransactionInformation.DistributedIdentifier.ToString());
                _employeeCollection.Add(employee.Id, employee);
                DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
                OperationContext.Current.SetTransactionComplete();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }

         [OperationBehavior(TransactionScopeRequired = true, TransactionAutoComplete = false)]
        public void SaveStep2(Employee employee)
        {
            try
            {
                Transaction currentTx = Transaction.Current;
                Console.WriteLine("Save step 2 is in tx {0}", currentTx.TransactionInformation.DistributedIdentifier.ToString());
                _employeeCollection.Add(employee.Id, employee);
                DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
                OperationContext.Current.SetTransactionComplete();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
    }
}
