using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WcfClient
{
    public partial class Form1 : Form
    {
        WCFService.ServiceWrapper<WCFService.IEmployeeService> sw;
        
        public Form1()
        {
            InitializeComponent();
            sw = new WCFService.ServiceWrapper<WCFService.IEmployeeService>(ServiceEndpoint);
        }

        const String ServiceEndpoint = "http://localhost:8080/EmployeeService";
        private void btnGetEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                var emp = sw.Channel.GetEmployee(Convert.ToInt32(txtID.Text));
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
            catch (FaultException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnSaveEmployee_Click(object sender, EventArgs e)
        {
            try
            {
                var emp = new WCFService.Employee()
                {
                    Id = Convert.ToInt32(txtID.Text),
                    Name = txtName.Text,
                    Gender = txtGender.Text,
                    DateOfBirth = Convert.ToDateTime(txtDateOfBirth.Text)
                };
                sw.Channel.SaveEmployee(emp);
                lblMessage.Text = "Save employee successfully.";
            }
            catch (FaultException ex)
            {
                MessageBox.Show(ex.Message);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
