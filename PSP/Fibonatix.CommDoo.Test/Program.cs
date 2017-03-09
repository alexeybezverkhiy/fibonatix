using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;

using Genesis.Net.Common;
using Genesis.Net.Entities;
using Genesis.Net.Entities.Requests.Initial;
using Genesis.Net.Entities.Requests.Initial.ThreeD;
using Genesis.Net.Entities.Requests.Query;
using Genesis.Net.Entities.Requests.Referential;
using Genesis.Net.Specs;
using Genesis.Net.Specs.Mocks;

using Fibonatix.CommDoo;
using Fibonatix.CommDoo.Requests;

namespace Genesis.Net.Test
{
    class Program
    {

        static void Main(string[] args)
        {
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
            System.Net.ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            Program p = new Program();
            Fibonatix.CommDoo.Test.GenesisTests genesis = new Fibonatix.CommDoo.Test.GenesisTests();
            Fibonatix.CommDoo.Test.KalixaTests kalixa = new Fibonatix.CommDoo.Test.KalixaTests();
            Fibonatix.CommDoo.Test.PComTests pcom = new Fibonatix.CommDoo.Test.PComTests();
            Fibonatix.CommDoo.Test.BorgunTests borgun = new Fibonatix.CommDoo.Test.BorgunTests();

            borgun.FullTests();


            Console.ReadKey();
        }
    }
}
