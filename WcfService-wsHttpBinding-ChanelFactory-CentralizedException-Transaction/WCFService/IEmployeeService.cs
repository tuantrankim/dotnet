﻿using System.ServiceModel;

namespace WCFService
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the interface name "IEmployeeService" in both code and config file together.
    [ServiceContract(SessionMode = SessionMode.Required)]  
    public interface IEmployeeService
    {
        [OperationContract]
        Employee GetEmployee(int id);
        [OperationContract]
        void SaveEmployee(Employee employee);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Mandatory)]
        
        void SaveStep1(Employee employee);

        [OperationContract]
        [TransactionFlow(TransactionFlowOption.Mandatory)]
        void SaveStep2(Employee employee);
    }
}
