using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fibonatix.CommDoo.Requests;
using Fibonatix.CommDoo.Responses;
using Fibonatix.CommDoo.Genesis;

namespace Fibonatix.CommDoo
{
    public class ConnectorFactory
    {
        private static NullConnector nullConn = new NullConnector();
        public static IConnector Create(Request req) {
            IConnector ret = null;
            switch (req.getAcquirer() ) {
                case AcquirerType.Genesis:
                    ret = Genesis.GenesisConnector.getConnector(req);
                    break;
                case AcquirerType.Kalixa:
                    ret = Kalixa.KalixaConnector.getConnector(req);
                    break;
                case AcquirerType.ProcessingCom:
                    ret = ProcessingCom.PComConnector.getConnector(req);
                    break;
                case AcquirerType.Borgun:
                    ret = Borgun.BorgunConnector.getConnector(req);
                    break;
                default:
                    ret = nullConn;
                    break;
            }
            return ret;
        }
    }
}
