using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using System.Transactions;

namespace WCFService
{
        [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Multiple, AddressFilterMode = AddressFilterMode.Any)]
    //enable to support transaction
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall, ConcurrencyMode = ConcurrencyMode.Single, AddressFilterMode = AddressFilterMode.Any)]
     [GlobalErrorHandlerBehavior(typeof(GlobalErrorHandler))]
    public class EmployeeService : IEmployeeService
    {
        static readonly EmployeeService instance = new EmployeeService();
        EmployeeService()
        {
        }

        static EmployeeService()
		{

		}
        public static EmployeeService Instance
        {
            get
            {
                return instance;
            }
        }

        private static Dictionary<int, Employee> _employeeCollection = new Dictionary<int, Employee>();
        public Employee GetEmployee(int id)
        {
            if (_employeeCollection.ContainsKey(id)) return _employeeCollection[id];
            else
            {
                throw new Exception("Cannot find employee id " + id);
            }
        }
        const String ServiceEndpoint = "";
        public Employee GetEmployeeInternal(int id)
        {            
            WCFService.ServiceWrapper<WCFService.IEmployeeService> sw;
            sw = new WCFService.ServiceWrapper<WCFService.IEmployeeService>(ServiceEndpoint);
            Employee emp = sw.Channel.GetEmployee(id);
            return emp;
        }
        //enable to support transaction
        // [OperationBehavior(TransactionScopeRequired = false)]
        public void SaveEmployee(Employee employee)
        {
            try
            {
                Console.WriteLine("empID={0}. Start={1}", employee.Id, DateTime.Now.ToLongTimeString());
                System.Threading.Thread.Sleep(10000);
                //DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
                _employeeCollection.Add(employee.Id, employee);
                Console.WriteLine("empID={0}. End={1}", employee.Id, DateTime.Now.ToLongTimeString());
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
        //enable to support transaction
         //[OperationBehavior(TransactionScopeRequired = false)]
         public void SaveEmployee2(Employee employee)
         {
             try
             {
                 Console.WriteLine("empID={0}. Start={1}", employee.Id, DateTime.Now.ToLongTimeString());
                 System.Threading.Thread.Sleep(10000); 
                 //DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
                _employeeCollection.Add(employee.Id, employee);
                 Console.WriteLine("empID={0}. End={1}", employee.Id, DateTime.Now.ToLongTimeString());
             }
             catch (Exception ex)
             {
                 throw new FaultException(ex.Message);
             }
         }
         //enable to support transaction
         //[OperationBehavior(TransactionScopeRequired = false)]
         public async void AsyncSaveEmployee(Employee employee)
         {
             try
             {
                 Console.WriteLine("empID={0}. Start={1}", employee.Id, DateTime.Now.ToLongTimeString());
                 await Task.Run(() => { System.Threading.Thread.Sleep(10000); });
                 //DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
                 _employeeCollection.Add(employee.Id, employee);
                 Console.WriteLine("empID={0}. End={1}", employee.Id, DateTime.Now.ToLongTimeString());
             }
             catch (Exception ex)
             {
                 throw new FaultException(ex.Message);
             }
         }
         //enable to support transaction
         //[OperationBehavior(TransactionScopeRequired = true)]
        public void SaveStep1(Employee employee)
        {
            try
            {
                Transaction currentTx = Transaction.Current;
                Console.WriteLine("Save step 1 is in tx {0}", currentTx.TransactionInformation.DistributedIdentifier.ToString());
                //_employeeCollection.Add(employee.Id, employee);
                System.Threading.Thread.Sleep(3000);
                DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
               // OperationContext.Current.SetTransactionComplete();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
        //enable to support transaction
         //[OperationBehavior(TransactionScopeRequired = true)]
        public void SaveStep2(Employee employee)
        {
            try
            {
                Transaction currentTx = Transaction.Current;
                Console.WriteLine("Save step 2 is in tx {0}", currentTx.TransactionInformation.DistributedIdentifier.ToString());
                //_employeeCollection.Add(employee.Id, employee);
                System.Threading.Thread.Sleep(3000);
                DataAccessLayer.Insert(employee.Id, employee.Name, employee.DateOfBirth);
               // OperationContext.Current.SetTransactionComplete();
            }
            catch (Exception ex)
            {
                throw new FaultException(ex.Message);
            }
        }
    }
}
