using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WebApiClientBasicAuthentication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }


        //string Username = "lafituser";
        //string Password = "-UwCC+#N#42$4jmJ";
        //string First = "Fember01";
        //string Last = "test295581";
        //string BirthDate = "1/1/1950";
        //string ZipCode = "92101";

        private async void btnRun_Click(object sender, EventArgs e)
        {
            Model.Member member = null;
            int memberId = 0;
            int.TryParse(txtMemberId.Text, out memberId);
            if (memberId > 0)
            {
                member = new Model.Member() { MemberId = memberId };
            }
            else
            {
                member = new Model.Member()
                {
                    FirstName = txtFirstName.Text,
                    LastName = txtLastName.Text,
                    DateOfBirth = dtmDateOfBirth.Value.Date,
                    ZipCode = txtZipCode.Text
                };
            }
            try
            {
                Model.Member responeMember = await PostJsonAsync("/verify", member);
                StringBuilder result = new StringBuilder();

                result.AppendLine("MemberId:" + responeMember.MemberId);
                result.AppendLine("FirstName:" + responeMember.FirstName);
                result.AppendLine("LastName:" + responeMember.LastName);
                result.AppendLine("ProductName:" + responeMember.ProductName);
                result.AppendLine("EmailAddress:" + responeMember.EmailAddress);
                result.AppendLine("ZipCode:" + responeMember.ZipCode);

                result.AppendLine("ResponeStatus:");
                result.AppendLine("\tErrorCode:" + responeMember.ResponseStatus.ErrorCode);
                result.AppendLine("\tMessage:" + responeMember.ResponseStatus.Message);
                result.AppendLine("\tStackTrace:" + responeMember.ResponseStatus.StackTrace);
                if (responeMember.ResponseStatus.Errors != null)
                {
                    foreach (var item in responeMember.ResponseStatus.Errors)
                    {
                        result.AppendLine("\t\tError:" + item);
                    }
                }

                txtResponse.Text = result.ToString();
            }
            catch (Exception ex)
            {
                txtResponse.Text = ex.Message;
            }
        }


        public async Task<Model.Member> PostJsonAsync(string apiUrl, Model.Member request)
        {
            string BaseUrl = "https://ashfitness.net/lafitness";
            HttpResponseMessage response;
            if (String.IsNullOrEmpty(apiUrl))
            {
                throw new ApplicationException("apiUrl required");
            }
            using (var client = new HttpClient())
            {
                var url = string.Format("{0}{1}", BaseUrl, apiUrl);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                var credentials = Encoding.ASCII.GetBytes(txtUserName.Text + ":" + txtPassword.Text);
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));

                response = await client.PostAsJsonAsync(new Uri(url), request);
                if (response.IsSuccessStatusCode)
                {
                    //var jsonAsString = await response.Content.ReadAsStringAsync();
                    //var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Member>(jsonAsString);
                    //return output;
                    return await response.Content.ReadAsAsync<Model.Member>();
                }
            }
            return default(Model.Member);
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            txtUserName.ResetText();
            txtPassword.ResetText();
            txtFirstName.ResetText();
            txtLastName.ResetText();
            txtMemberId.ResetText();
            txtZipCode.ResetText();
            dtmDateOfBirth.ResetText();
            txtResponse.ResetText();

        }
    }
}
