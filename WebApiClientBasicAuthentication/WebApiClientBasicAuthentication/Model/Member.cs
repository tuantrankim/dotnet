using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/* JSON Respone Structure
{
  "ResponseStatus": {
    "ErrorCode": "",
    "Message": "",
    "StackTrace": "",
    "Errors": [
      {
        "ErrorCode": "",
        "FieldName": "",
        "Message": ""
      }
    ]
  },
  "MemberId": "int",
  "FirstName": "",
  "LastName": "",
  "ProductName": "",
  "EmailAddress": "",
  "ZipCode": ""
}
*/

namespace WebApiClientBasicAuthentication.Model
{
    public class Member:BaseResponse
    {
        public int? MemberId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string EmailAddress { get; set; }
        public string ZipCode { get; set; }
        public string ProductName { get; set; }
    }

    //public class MemberResult
    //{
    //    public ResponseStatus ResponseStatus { get; set; }
    //    public int? MemberId { get; set; }
    //    public string FirstName { get; set; }
    //    public string LastName { get; set; }
    //    public string ProductName { get; set; }
    //    public string EmailAddress { get; set; }
    //    public string ZipCode { get; set; }
    //}
    public class BaseResponse
    {
        public ResponseStatus ResponseStatus { get; set; }
    }
    public class ResponseStatus
    {
        public string ErrorCode { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public List<Error> Errors { get; set; }
    }
    public class Error
    {
        public string ErrorCode { get; set; }
        public string FieldName { get; set; }
        public string Message { get; set; }
    }
}


