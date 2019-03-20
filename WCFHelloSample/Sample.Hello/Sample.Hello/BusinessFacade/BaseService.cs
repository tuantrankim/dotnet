using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;

namespace Sample.Hello.BusinessFacade
{
    public class BaseService
    {
        private string baseUrl = "";
        private string userName = "";
        private string password = "";
        private string oAuthAccessKey = "";
        private Authentication authentication = Authentication.None;
        public BaseService(string baseUrl, Authentication authentication = Authentication.None, string userName = "", string password = "", string oAuthAccessKey = "")
        {
            this.baseUrl = baseUrl;
            this.userName = userName;
            this.password = password;
            this.oAuthAccessKey = oAuthAccessKey;
            this.authentication = authentication;
        }
        public async Task<T1> PostJsonAsync<T1, T2>(int changeSourceId, string apiUrl, T2 requestParam)
        {
            HttpResponseMessage response;
            string jsonRequest = JsonConvert.SerializeObject(requestParam, Formatting.Indented);
            //if (String.IsNullOrEmpty(apiUrl))
            //{
            //    throw new ApplicationException("apiUrl required");
            //}

            string requestStr = "";

            if (apiUrl.StartsWith("/"))
            {
                requestStr = baseUrl + apiUrl;
            }
            else
            {
                requestStr = baseUrl + "/" + apiUrl;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    Uri requestUrl = new Uri(requestStr);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    if (authentication == Authentication.Basic)
                    {
                        var credentials = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
                    }
                    else if (authentication == Authentication.OAuth)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuthAccessKey);
                        //client.DefaultRequestHeaders.Add("IDENTITY_KEY", oAuthAccessKey);
                    }
                    //need to install nuget webApi client new version
                    response = await client.PostAsJsonAsync(requestUrl, requestParam).ConfigureAwait(false);
                    
                    if (response.IsSuccessStatusCode)
                    {
                        //var jsonAsString = await response.Content.ReadAsStringAsync();
                        //var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Member>(jsonAsString);
                        //return output;
                        return await response.Content.ReadAsAsync<T1>().ConfigureAwait(false);
                    }
                        //Allow to stream internal error from response body
                        //This may get JSON parse exception
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        string message = "HTTPRespone error " + response.StatusCode + ".\n" + response.ToString();
                        Exception ex = new Exception(message);
                        //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                        ////LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message, "", "", false, "", "", "");
                        
                        try
                        { 
                            return await response.Content.ReadAsAsync<T1>().ConfigureAwait(false);
                        } catch(Exception e){}
                    }
                    //else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) throw new Exception(response.ReasonPhrase);
                    else
                    {
                        //Log the bad response
                        string message = "HTTPRespone error " + response.StatusCode + ".\n " + response.ToString() + ".\n Response: \n";
                        if (response.Content != null) message += response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Exception ex = new Exception(message);
                        //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                        ////LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message , "", "", false, "", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                //should handle error log from caller to reduce error log
                //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                throw;
            }
            return default(T1);
        }

        //Use this method to get OAuth token
        public async Task<T1> PostAsync<T1, T2>(int changeSourceId, string apiUrl, T2 requestParam)
        {
            HttpResponseMessage response;
            string jsonRequest = JsonConvert.SerializeObject(requestParam, Formatting.Indented);
            //POST to oauth to get token (must be sent as "application/x-www-form-urlencoded")
            Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonRequest);  //convert to key/value pairs
            //Do not log password or secret
            string[] securities = new [] {"password", "secret", "credential"};
            if (securities.Any(s=>jsonRequest.ToLower().Contains(s)))
            {
                jsonRequest = "{Secured data}";
            }

            //if (String.IsNullOrEmpty(apiUrl))
            //{
            //    throw new ApplicationException("apiUrl required");
            //}

            string requestStr = "";

            if (apiUrl.StartsWith("/"))
            {
                requestStr = baseUrl + apiUrl;
            }
            else
            {
                requestStr = baseUrl + "/" + apiUrl;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    Uri requestUrl = new Uri(requestStr);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    
                    if (authentication == Authentication.Basic)
                    {
                        var credentials = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
                    }
                    else if (authentication == Authentication.OAuth)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuthAccessKey);
                        //client.DefaultRequestHeaders.Add("IDENTITY_KEY", oAuthAccessKey);
                    }
                    

                    var req = new HttpRequestMessage(HttpMethod.Post, requestUrl) { Content = new FormUrlEncodedContent(values) };
                    response = await client.SendAsync(req).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        //var jsonAsString = await response.Content.ReadAsStringAsync();
                        //var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Member>(jsonAsString);
                        //return output;
                        return await response.Content.ReadAsAsync<T1>().ConfigureAwait(false);
                    }
                    //Allow to stream internal error from response body
                    //This may get JSON parse exception
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        string message = "HTTPRespone error " + response.StatusCode + ".\n" + response.ToString();
                        Exception ex = new Exception(message);
                        //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                        ////LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message, "", "", false, "", "", "");

                        try
                        {
                            return await response.Content.ReadAsAsync<T1>().ConfigureAwait(false);
                        }
                        catch (Exception e) { }
                    }
                    //else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) throw new Exception(response.ReasonPhrase);
                    else
                    {
                        //Log the bad response
                        string message = "HTTPRespone error " + response.StatusCode + ".\n " + response.ToString() + ".\n Response: \n";
                        if (response.Content != null) message += response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Exception ex = new Exception(message);
                        //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                        ////LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message , "", "", false, "", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                //should handle error log from caller to reduce error log
                //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                throw;
            }
            return default(T1);
        }

        public async Task<T1> GetAsync<T1, T2>(int changeSourceId, string apiUrl, T2 requestParam)
        {
            HttpResponseMessage response;
            string jsonRequest = JsonConvert.SerializeObject(requestParam, Formatting.Indented);
            
            //Do not log password or secret
            string[] securities = new[] { "password", "secret", "credential" };
            if (securities.Any(s => jsonRequest.ToLower().Contains(s)))
            {
                jsonRequest = "{Secured data}";
            }

            //if (String.IsNullOrEmpty(apiUrl))
            //{
            //    throw new ApplicationException("apiUrl required");
            //}

            string requestStr = "";

            if (apiUrl.StartsWith("/"))
            {
                requestStr = baseUrl + apiUrl;
            }
            else
            {
                requestStr = baseUrl + "/" + apiUrl;
            }
            try
            {
                using (var client = new HttpClient())
                {
                    Uri requestUrl = new Uri(requestStr);

                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    if (authentication == Authentication.Basic)
                    {
                        var credentials = System.Text.Encoding.ASCII.GetBytes(userName + ":" + password);
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(credentials));
                    }
                    else if (authentication == Authentication.OAuth)
                    {
                        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", oAuthAccessKey);
                        //client.DefaultRequestHeaders.Add("IDENTITY_KEY", oAuthAccessKey);
                    }
                    Dictionary<string, string> values = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonRequest);  //convert to key/value pairs
                    if (values != null && values.Count > 0)
                    {
                        var content = new FormUrlEncodedContent(values);
                        string urlParams = await content.ReadAsStringAsync().ConfigureAwait(false);

                        requestStr = requestStr + "?" + urlParams;
                    }
                    response = await client.GetAsync(requestStr).ConfigureAwait(false);

                    if (response.IsSuccessStatusCode)
                    {
                        //var jsonAsString = await response.Content.ReadAsStringAsync();
                        //var output = Newtonsoft.Json.JsonConvert.DeserializeObject<Model.Member>(jsonAsString);
                        //return output;
                        return await response.Content.ReadAsAsync<T1>().ConfigureAwait(false);
                    }
                    //Allow to stream internal error from response body
                    //This may get JSON parse exception
                    else if (response.StatusCode == System.Net.HttpStatusCode.InternalServerError)
                    {
                        string message = "HTTPRespone error " + response.StatusCode + ".\n" + response.ToString();
                        Exception ex = new Exception(message);
                        //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                        ////LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message, "", "", false, "", "", "");

                        try
                        {
                            return await response.Content.ReadAsAsync<T1>().ConfigureAwait(false);
                        }
                        catch (Exception e) { }
                    }
                    //else if (response.StatusCode == System.Net.HttpStatusCode.ServiceUnavailable) throw new Exception(response.ReasonPhrase);
                    else
                    {
                        //Log the bad response
                        string message = "HTTPRespone error " + response.StatusCode + ".\n " + response.ToString() + ".\n Response: \n";
                        if (response.Content != null) message += response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                        Exception ex = new Exception(message);
                        //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                        ////LogHelper.LogError("Sample.Hello.Exception", "Sample.Hello", message , "", "", false, "", "", "");
                    }
                }
            }
            catch (Exception ex)
            {
                //should handle error log from caller to reduce error log
                //LogHelper.LogError("Sample.Hello.Exception", ex, false, MethodInfo.GetCurrentMethod(), changeSourceId, apiUrl, jsonRequest);
                throw;
            }
            return default(T1);
        }

        public enum Authentication { None = 0, Basic = 1 , OAuth = 2}
    }
}
