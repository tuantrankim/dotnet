using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
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

        const String ServiceEndpoint = "net.tcp://localhost:8080/EmployeeService";
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

        private async void btnSaveEmployee_Click(object sender, EventArgs e)
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
                await Task.Run(() => { sw.Channel.SaveEmployee(emp); });
                
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
        private async void btnSaveEmployee2_Click(object sender, EventArgs e)
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
                await Task.Run(() => { sw.Channel.SaveEmployee2(emp); });

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
        private void btnTransaction_Click(object sender, EventArgs e)
        {
            try
            {

                using (TransactionScope scope = new TransactionScope())
                {
                    var emp = new WCFService.Employee()
                    {
                        Id = Convert.ToInt32(txtID.Text),
                        Name = "aaaa",
                        Gender = txtGender.Text,
                        DateOfBirth = Convert.ToDateTime(txtDateOfBirth.Text)
                    };
                    try
                    {
                        sw.Channel.SaveStep1(emp);
                    }
                    catch { }
                    emp.Id++;
                    emp.Name = "bbb";
                    sw.Channel.SaveStep2(emp);

                    
                    scope.Complete();
                    lblMessage.Text = "Save employee successfully.";
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

        private async void btnSaveEmployeeAsync_Click(object sender, EventArgs e)
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
                await Task.Run(() => { sw.Channel.AsyncSaveEmployee(emp); });

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
