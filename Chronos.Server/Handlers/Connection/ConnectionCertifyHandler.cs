﻿using Chronos.Server.Game.Account;
using Chronos.Server.Manager.Account;
using Chronos.Server.Network;
using Chronos.Protocol.Enums;
using Chronos.Protocol.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chronos.Server.Handlers.Connection
{
    public partial class ConnectionHandler
    {
        [HeaderPacket(CertifyMessage.Header)]
        public static void HandleCertifyMessage(SimpleClient client, CertifyMessage message)
        {
            GameAccount account;
            if(!CredentialsManager.Instance.CheckAccountValidity(out account, message.username, message.password))
            {
                SendCertifyResultMessage(client, ErrorEnum.ERR_CERT_BAD_PASSWORD, ErrorEnum.ERR_CERT_BAD_PASSWORD);
                client.Disconnect();
                return;
            }
            if(!message.realVersion.GetVersionFromCompacted().IsUpToDate())
            {
                SendCertifyResultMessage(client, ErrorEnum.ERR_CERT_VERSION, ErrorEnum.ERR_VERSION_TOO_LOW);
                client.Disconnect();
                return;
            }
            if(SimpleServer.ConnectedClients.Exists(x => x.Account != null && x.Account.Record.Username == message.username))
            {
                SendCertifyResultMessage(client, ErrorEnum.ERR_ACCOUNT_EXIST, ErrorEnum.ERR_ACCOUNT_EXIST);
                client.Disconnect();
                return;

            }
            account.IP_Key = Encoding.UTF8.GetString(message.ip_key);
            account.HDSN = message.hdsn;
            client.Account = account;

            SendCertifyResultMessage(client);
        }
        public static void SendCertifyResultMessage(IPacketInterceptor client)
        {
            client.Send(new CertifyResultMessage(ErrorEnum.CERT_OK, 0, false, ErrorEnum.CERT_OK));
        }
        public static void SendCertifyResultMessage(IPacketInterceptor client, ErrorEnum errorCode, ErrorEnum subErrorCode)
        {
            client.Send(new CertifyResultMessage(errorCode, 0, false, subErrorCode));
        }
    }
}