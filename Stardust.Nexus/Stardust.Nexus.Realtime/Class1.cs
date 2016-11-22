using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RethinkDb.Driver;
using RethinkDb.Driver.Net;
using RethinkDb.Driver.Net.Clustering;

namespace Stardust.Nexus.Realtime
{
    public class StreamCollector
    {
        private RethinkDB db;

        private Connection connection;

        public StreamCollector()
        {
            db = RethinkDb.Driver.RethinkDB.R;
            db.Connection().Hostname("localhost").Port(8080);

            connection = db.Connection().Hostname("localhost").Port(8080).Connect();
            db.Db("stardustRealtime")
                .Table("usage")
                .Insert(new
                {
                    id = Guid.NewGuid().ToString(),
                    System = "_self",
                    Host = "_service",
                    Environment = "this",
                    Service = "_initialize",
                    LogTime = DateTime.UtcNow
                })
                .Run(connection);

        }


    }
}
