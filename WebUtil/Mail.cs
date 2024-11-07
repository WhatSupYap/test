using iTextSharp.text.log;
using iTextSharp.xmp.impl;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.util;

namespace WebUtil
{
    public class Mail : IDisposable
    {
        private bool disposed = false;
        private Util Utils = null;

        /// <summary>
        /// 생성자
        /// </summary>
        public Mail(Util util)
        {
            Utils = util;
        }

        /// <summary>
        /// BU별 메일발송을 위한 함수
        /// </summary>
        /// <param name="idx"></param>
        /// <param name="mailto"></param>
        /// <param name="subject"></param>
        /// <param name="html"></param>
        /// <param name="mailtype"></param>
        /// <param name="bu"></param>
        /// <param name="regid"></param>
        public void SendMail(string idx, string mailto, string subject, string html, string mailtype, string bu, string regid, string mailfrom ="", string mailcc="")
        {
            try
            {
                MailMessage message = new MailMessage();
                DataTable mailaddr = new DataTable(); 

                if (!string.IsNullOrEmpty(mailtype))
                {
                    //mail 수신자 조회
                    mailaddr = GetMailAddr(mailtype);
                }

                if (string.IsNullOrEmpty(mailfrom)) {
                    DataRow[] selectedRowsFROM = mailaddr.Select("GUBUN = 'FROM'");
                    mailfrom = selectedRowsFROM[0].Field<string>("EMAIL");
                }
                
                //발송인 설정
                message.From = new MailAddress(mailfrom);

                //수신인 설정
                if (!string.IsNullOrEmpty(mailto))
                {
                    message.To.Add(mailto);
                }
                else
                {
                    DataRow[] selectedRowsTO = mailaddr.Select($"GUBUN = 'TO' AND (BU_CC  = '{bu}' OR BU_CC  = 'ALL')");
                    HashSet<string> emailSet = new HashSet<string>();
                    foreach (DataRow row in selectedRowsTO)
                    {
                        string email = row.Field<string>("EMAIL");
                        if (!emailSet.Contains(email))
                        {
                            emailSet.Add(email);
                            message.To.Add(email);
                            if (!string.IsNullOrEmpty(mailto))
                            {
                                mailto += "; ";
                            }
                            mailto += email;
                        }
                    }
                }

                //참조인 설정
                if (!string.IsNullOrEmpty(mailcc))
                {
                    message.CC.Add(mailcc);
                }
                else if(!string.IsNullOrEmpty(bu))
                {
                    DataRow[] selectedRowsCC = mailaddr.Select($"GUBUN = 'CC' AND (BU_CC  = '{bu}' OR BU_CC  = 'ALL')");
                    HashSet<string> emailSetCC = new HashSet<string>();
                    mailcc = string.Empty;
                    foreach (DataRow row in selectedRowsCC)
                    {
                        string email = row.Field<string>("EMAIL");
                        if (!emailSetCC.Contains(email))
                        {
                            emailSetCC.Add(email);
                            message.CC.Add(email);
                            if (!string.IsNullOrEmpty(mailcc))
                            {
                                mailcc += "; ";
                            }
                            mailcc += email;
                        }
                    }
                }

                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = html;

                string SMTPHost = ConfigurationManager.AppSettings["SMTPHost"].ToString();
                SmtpClient client = new SmtpClient(SMTPHost);
                client.Port = 25;

                client.Send(message);

                Utils.Excute(@"SP_MAIL_LOG_INS", new
                {
                    @Ref_Idx = idx,
                    @MAIL_TYPE = mailtype,
                    @MAIL_FROM = mailfrom,
                    @MAIL_TO = mailto,
                    @MAIL_CC = mailcc,
                    @MAIL_BCC = "",
                    @MAIL_SUBJECT = subject,
                    @MAIL_BODY = html.ToString(),
                    @MAIL_YN = "Y",
                    @SMTP_HOST = SMTPHost,
                    @REG_ID = regid
                });

                if (message != null) { message.Dispose(); message = null; }
                if (client != null) { client.Dispose(); client = null; }

            }
            catch (Exception ex)
            {
                
            }

        }


        /// <summary>
        /// 메일 수신자 대상 주소 조회
        /// </summary>
        /// <returns></returns>
        public DataTable GetMailAddr(string mailtype)
        {
            using (DataSet ds = Utils.GetList(@"SP_MAIL_ADDR_SEL", new
            {
                @REPORT_MAIL_TYPE = mailtype

            }, 600))
            {
                return ds.Tables[0];
            }
        }

        #region --------------------------IDisposable Members-----------------------------
        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            // This object will be cleaned up by the Dispose method.
            // Therefore, you should call GC.SupressFinalize to
            // take this object off the finalization queue
            // and prevent finalization code for this object
            // from executing a second time.
            GC.SuppressFinalize(this);

        }

        private void Dispose(bool disposing)
        {
            // Check to see if Dispose has already been called.
            if (!this.disposed)
            {
                // If disposing equals true, dispose all managed
                // and unmanaged resources.
                if (disposing)
                {
                }

                // Call the appropriate methods to clean up
                // unmanaged resources here.
                // If disposing is false,
                // only the following code is executed.

                // Note disposing has been done.
                disposed = true;
            }
        }
        #endregion

    }
}
