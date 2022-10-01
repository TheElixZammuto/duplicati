using HttpServer;
using HttpServer.HttpModules;
using HttpServer.Rules;
using HttpServer.Sessions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Duplicati.Server.WebServer
{
    internal class MetricsHandler : HttpModule
    {
        public static readonly Dictionary<String, String>  METADATA_FILTER = new Dictionary<String,String>{
                {"LastBackupDate","backup_last"},
                {"BackupListCount","backup_count" },
                {"TotalQuotaSpace","quota_total" },
                {"FreeQuotaSpace", "quota_free" },
                {"AssignedQuotaSpace", "quota_assigned" },
                {"TargetFilesSize", "targetfiles_size" },
                {"TargetFilesCount","targetfiles_count" },
                {"SourceFilesSize", "sourcefiles_size" },
                {"SourceFilesCount","sourcefiles_count" },
                {"LastBackupStarted", "lastbackup_started" },
                {"LastBackupFinished","lastbackup_finished" },
                {"LastBackupDuration","lastbackup_duration" },
                {"LastErrorDate","lasterror_date" },
                {"LastRestoreDuration","lasterror_duration" },
                {"LastRestoreStarted","lastrestore_started" },
                {"LastRestoreFinished","lastrestore_finished" },
                {"LastCompactDuration","lastcompact_duration" },
                {"LastCompactStarted","lastcompact_started" },
                {"LastCompactFinished","lastcompact_finished" }
            };
        public override bool Process(IHttpRequest request, IHttpResponse response, IHttpSession session)
        {
            if (!request.Uri.AbsolutePath.StartsWith("/metrics", StringComparison.OrdinalIgnoreCase)) return false;
            response.Status = System.Net.HttpStatusCode.OK;
            response.Reason = "OK";
            response.ContentType = "text/plain; charset=utf-8";
            response.AddHeader("Cache-Control", "no-cache, no-store, must-revalidate, max-age=0");
            var s = new StringBuilder();
            s.AppendLine("duplicati_ok 1");
            var backups = Program.DataConnection.Backups;
            
            foreach (var backup in backups)
            {
                foreach (var mk in backup.Metadata)
                {
                    if(METADATA_FILTER.ContainsKey(mk.Key))s.AppendLine("duplicati_backup_"+ METADATA_FILTER[mk.Key]+"{id=" + backup.ID + ",name="+backup.Name+"} " + mk.Value);
                }
            }
            var ba = Encoding.UTF8.GetBytes(s.ToString());
            response.ContentLength = ba.Length;
            response.Body = new MemoryStream(ba);
            response.Send();
            return true;
        }
    }
}