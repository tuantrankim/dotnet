using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WcfClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnGetEmployee_Click(object sender, EventArgs e)
        {
            using (EmployeeService.EmployeeServiceClient client = new EmployeeService.EmployeeServiceClient())
            {
                var emp = client.GetEmployee(Convert.ToInt32(txtID.Text));
                if (emp != null)
                {
                    txtName.Text = emp.Name;
                    txtGender.Text = emp.Gender;
                    txtDateOfBirth.Text = emp.DateOfBirth.ToShortDateString();
                    lblMessage.Text = "Get employee successfully.";
                }
                else
                {
                    lblMessage.Text = "Cannot find the employee.";
                }
            }
            
        }

        private void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            using (EmployeeService.EmployeeServiceClient client = new EmployeeService.EmployeeServiceClient())
            {
                var emp = new EmployeeService.Employee() { 
                Id = Convert.ToInt32(txtID.Text),
                Name = txtName.Text,
                Gender = txtGender.Text,
                DateOfBirth = Convert.ToDateTime(txtDateOfBirth.Text)
                };
                client.SaveEmployee(emp);
                lblMessage.Text = "Save employee successfully.";
            }
        }
    }
}
