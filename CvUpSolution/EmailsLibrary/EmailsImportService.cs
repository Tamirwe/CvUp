using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailsLibrary
{
	public class EmailsImportService : IEmailsImportService
	{
		private IConfigurationSection gmailSettings;

		public EmailsImportService(IConfiguration config)
        {
			gmailSettings = config.GetRequiredSection("gMmailSettings");

		}

		public bool ReadNotSeenEmails()
        {
			using (var client = new ImapClient(new ProtocolLogger("imap.log")))
			{
				client.Connect("imap.gmail.com", 993, SecureSocketOptions.SslOnConnect);

				client.Authenticate("cvup.files@gmail.com", "vinrkkgwgvsylilr");

				client.Inbox.Open(FolderAccess.ReadOnly);

				var uids = client.Inbox.Search(SearchQuery.NotSeen);

				foreach (var uid in uids)
				{
					var message = client.Inbox.GetMessage(uid);

					// write the message to a file
					//message.WriteTo(string.Format("{0}.eml", uid));
				}

				client.Disconnect(true);
			}

			return true;
        }
    }
}
