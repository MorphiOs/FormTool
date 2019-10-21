/*
 * Created by SharpDevelop.
 * User: Owner
 * Date: 10/10/2019
 * 
 */
using System;
using System.Collections.Generic;
using System.Windows;
using RestSharp;
using System.Net;

namespace FormTool
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        // classes for loginObject
        public class Revision
        {
            // also applies to formObject
            public string id { get; set; }
            public string url { get; set; }
        }
        public class Settings
        {
            public string units { get; set; }
            public string dateFormat { get; set; }
        }
        public class loginObject
        {
            public string token { get; set; }
            public string sessionToken { get; set; }
            public string schema { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string email { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public string firstName { get; set; }
            public string lastName { get; set; }
            // added for failure uname/pass
            public string message { get; set; }
            public Revision revision { get; set; }
            public Settings settings { get; set; }
        }
        // class for dep(Department)Object
        public class Org
        {
            public string id { get; set; }
            public string url { get; set; }
        }
        public class depObject
        {
            public string schema { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public Org org { get; set; }
            public List<string> tokens { get; set; }
        }
        // classes for formObject
        public class RootFormId
        {
            public string id { get; set; }
            public string url { get; set; }
        }
        public class Provider
        {
            public string type { get; set; }
            public int distance { get; set; }
            public string vector { get; set; }
            //public Fields fields { get; set; }
        }
        public class Field
        {
            public string id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public bool title { get; set; }
            public string cloneable { get; set; }
            public object options { get; set; }
            //public List<Field2> fields { get; set; }
            public List<Provider> provider { get; set; }
        }
        public class Import
        {
            public string type { get; set; }
            public int version { get; set; }
        }
        public class Ocalc
        {
            public string config { get; set; }
        }
        public class Spidacalc
        {
            public bool useEnvironment { get; set; }
            public bool useIncline { get; set; }
            public List<List<object>> wires { get; set; }
            public string clientfile { get; set; }
        }
        public class Options
        {
            public Ocalc ocalc { get; set; }
            public Spidacalc spidacalc { get; set; }
        }
        public class Metadata
        {
            public Import import { get; set; }
            public Options options { get; set; }
        }
        public class formObject
        {
            public string schema { get; set; }
            public string type { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public RootFormId rootFormId { get; set; }
            public Revision revision { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
            public List<Field> fields { get; set; }
            public Metadata metadata { get; set; }
        }
        public class jobObject
        {
            public string schema { get; set; }
            public string id { get; set; }
            public string url { get; set; }
            public string name { get; set; }
            public DateTime createdAt { get; set; }
            public DateTime updatedAt { get; set; }
        }
        public string[] mnyForms;
        public Window1()
        {
            InitializeComponent();
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            //textbox1.Text = "";
            //password1.Password = "";
        }
        void button1_Click(object sender, RoutedEventArgs e)
        {
            var client = new RestClient("https://office.ikegps.com");
            var loginRequest = new RestRequest("v1/login", Method.POST);
            loginRequest.AddHeader("ContentType", "application/json");
            loginRequest.AddParameter("application/json", "{\"username\" : \"" + textbox1.Text + "\", \"password\" : \"" + password1.Password + "\" }", ParameterType.RequestBody);
            IRestResponse response = client.Execute(loginRequest);
            loginObject stepOne = SimpleJson.DeserializeObject<loginObject>(response.Content);
            if (stepOne.message == "Validation failed") {
                MessageBox.Show("Check your username / password");
            } else {
                var depRequest = new RestRequest("v1/department.json", Method.GET);
                depRequest.AddHeader("ContentType", "application/json");
                depRequest.AddHeader("Authorization", "token " + stepOne.token);
                IRestResponse response2 = client.Execute(depRequest);
                char[] charsToTrim = { '[', ']' };
                string jStrip = response2.Content.Trim(charsToTrim);
                depObject stepTwo = SimpleJson.DeserializeObject<depObject>(jStrip);
                var formRequest = new RestRequest("v1/form.json", Method.GET);
                formRequest.AddHeader("ContentType", "application/json");
                formRequest.AddHeader("Authorization", "token " + stepOne.token);
                IRestResponse response3 = client.Execute(formRequest);
                jStrip = response3.Content.Trim(charsToTrim);
                // ALT + 0241 = ñ
                jStrip = jStrip.Replace("\"schema\"", "ñ");
                string[] mSplit = new string[] { "ñ" };
                mnyForms = jStrip.Split(mSplit, StringSplitOptions.None);
                int index = 0;
                foreach (string form in mnyForms) {
                    index++;
                    mnyForms[index] = "{\"schema\"" + mnyForms[index].Substring(0, mnyForms[index].Length - 2);
                    formObject stepThree = SimpleJson.DeserializeObject<formObject>(mnyForms[index]);
                    listbox1.Items.Add(stepThree.name);
                    if (index == 8) {
                        break;
                    }
                }
            }
        }
        void button2_Click(object sender, RoutedEventArgs e)
        {
            if (listbox1.SelectedIndex != -1) {
                MessageBox.Show(mnyForms[listbox1.SelectedIndex + 1]);
            }
        }
    }
}