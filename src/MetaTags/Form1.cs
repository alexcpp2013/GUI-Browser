using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using mshtml;
using System.Xml.Linq;
using System.Net;

namespace MetaTags
{
    public partial class Form1 : Form
    {
        //private WebBrowser Web = null;
        private XDocument xml;
        private WebClient client = new WebClient();

        private enum TypeParameter { Good, Nothing, Error}

        private static class HttpData
        {
            public static string http = "http://";
            public static string https = "https://";
            public static string www = "www.";
            //ww2 ww3 www2 www3 ... must use in address
            //or use List<string>

            public enum TypeParameter { Good, Nothing, Error }

            public static string Culture = "en-US";

            //public const int TREE = 0;
        }

        public Form1()
        {
            InitializeComponent();
            StartNewBrowser();
        }

        private bool Navigate(String address)
        {
            //Use user address for understand what site we loaded exactly
            if (String.IsNullOrEmpty(address)) return false;
            if (address.Equals("about:blank")) return false;
            if (!address.StartsWith(HttpData.http) &&
                !address.StartsWith(HttpData.https))
            {
                address = HttpData.http + address;
            }

            try
            {
                Web.Navigate(new Uri(address));
                return true;
            }
            catch (System.UriFormatException ex)
            {
                throw new Exception("Ошибка в uri. \n" + ex.Message);
                //return false;
            }
            catch (Exception ex)
            {
                throw new Exception("Ошибка при переходе к документу. \n" + ex.Message);
                //return false;
            }

        }

        private void Manage()
        {
            string url = toolStripTextBox1.Text;
            
            if (url == "")
            {
                MessageBox.Show("Указан пустой url.", "Информационное сообщение",
                                MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            //StartNewBrowser();

            LoadPage(url);
        }

        private void LoadPage(string url)
        {
            try
            {
                progressBar1.Visible = true;

                if (Navigate(url) != true)
                    throw (new Exception("Не корректный url."));

                while (Web.ReadyState != WebBrowserReadyState.Complete || Web.IsBusy)
                    //|| Web.Url.AbsoluteUri != url)
                    Application.DoEvents();

                if (Web.Document == null)
                    throw new Exception("Страница не загрузилась.");

                IHTMLDocument2 currentDoc = (IHTMLDocument2)Web.Document.DomDocument;
                if (currentDoc.url.StartsWith("res://"))
                    throw new Exception("Не найдена страница: " + url + "\n");

                if (Web.Document.Body == null)
                    throw new Exception("Документ не заргружен.");

                //progressBar1.Visible = false;

            }
            catch (Exception err)
            {
                progressBar1.Visible = false;
                MessageBox.Show("Произошла ошибка во время работы программы: \n\n" +
                                err.Message, "Ошибка",
                                MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                progressBar1.Visible = false;
            }
        }

        private void StartNewBrowser()
        {
            ClearWebBrowser();

            //Web = new WebBrowser();
            Web.ScriptErrorsSuppressed = true;
            Web.Visible = true;
        }

        private void ClearWebBrowser()
        {
            /*if(Web != null)
                Web.Dispose();
            Web = null;*/
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            ClearAll();
            Close();
        }

        private void ClearAll()
        {
            progressBar1.Visible = false;
            ClearWebBrowser();
            toolStripTextBox1.Clear();
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            Manage();
        }

        private void toolStripTextBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Manage();
            }
        }
    }
}
