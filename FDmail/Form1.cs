using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Threading;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Mail;
using System.IO;

namespace FDmail
{
    public partial class Form1 : Form
    {
        public interface IEmailService
        {
            bool SendEmailMessage(EmailMessage message);
        }
        public class SmtpConfiguration
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Host { get; set; }
            public int Port { get; set; }
            public bool Ssl { get; set; }
        }
        public class EmailMessage
        {
            public string ToEmail { get; set; }
            public string Subject { get; set; }
            public string Body { get; set; }
            public bool IsHtml { get; set; }
        }
        public class GmailEmailService : IEmailService
        {
            private readonly SmtpConfiguration _config;
            public GmailEmailService(string email, string password)
            {
                _config = new SmtpConfiguration();
                var gmailUserName = email;
                var gmailPassword = password;
                var gmailHost = "smtp.gmail.com";
                var gmailPort = 587;
                var gmailSsl = true;
                _config.Username = gmailUserName;
                _config.Password = gmailPassword;
                _config.Host = gmailHost;
                _config.Port = gmailPort;
                _config.Ssl = gmailSsl;
            }
            public bool SendEmailMessage(EmailMessage message)
            {
                var success = false;
                try
                {
                    var smtp = new SmtpClient
                    {
                        Host = _config.Host,
                        Port = _config.Port,
                        EnableSsl = _config.Ssl,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(_config.Username, _config.Password)
                    };
                    using (var smtpMessage = new MailMessage(_config.Username, message.ToEmail))
                    {
                        smtpMessage.Subject = message.Subject;
                        smtpMessage.Body = message.Body;
                        smtpMessage.IsBodyHtml = message.IsHtml;
                        smtp.Send(smtpMessage);
                    }
                    success = true;
                }
                catch (Exception ex)
                {
                    //todo: add logging integration
                    //throw;
                }
                return success;
            }
        }
        public Form1()
        {
            InitializeComponent();
            textBox2.PasswordChar = '*';
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listView1.Items.Clear();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text) && !string.IsNullOrWhiteSpace(textBox2.Text) && !string.IsNullOrWhiteSpace(textBox3.Text) && !string.IsNullOrWhiteSpace(textBox4.Text) && !string.IsNullOrWhiteSpace(textBox5.Text))
            {
                GmailEmailService gmail = new GmailEmailService(textBox1.Text, textBox2.Text);
                EmailMessage msg = new EmailMessage();
                msg.Body = textBox4.Text;
                msg.IsHtml = true;
                msg.Subject = textBox3.Text;
                foreach (ListViewItem listViewItem in this.listView1.Items)
                {
                    msg.ToEmail = listViewItem.SubItems[1].Text;
                    MessageBox.Show(msg.ToEmail);
                    gmail.SendEmailMessage(msg);
                    Thread.Sleep(Convert.ToInt32(textBox5.Text));
                }
            }else{
                MessageBox.Show("Insira Valores Validos, para que o email seja Enviado!");
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            int count = 0;
            using (OpenFileDialog ofd = new OpenFileDialog() { Filter = "All Files | *.*", ValidateNames = true, Multiselect = true })
            {
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    string[] lines = System.IO.File.ReadAllLines(ofd.FileName);
                    foreach (string line in lines)
                    {
                        count += 1;
                        ListViewItem item = new ListViewItem(count.ToString());
                        item.SubItems.Add(line);
                        listView1.Items.Add(item);
                    }
                }
            }
        }
    }
}
