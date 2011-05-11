using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace PcPv2
{
    /// <summary>
    /// Base class for state object that gets passed around amongst async methods 
    /// when doing async web request/response for data transfer.  We store basic 
    /// things that track current state of a download, including # bytes transfered,
    /// as well as some async callbacks that will get invoked at various points.
    /// </summary>
    public class UploadState
    {
        public long totalBytes;		    // Total bytes to read
        public bool success;    // We successfully completed communications. This does not mean everything worked.
        // Callbacks for response packet info & progress
        public ProgressDelegate progCB;
        public DoneDelegate uploadDoneCB;
        public StatusDelegate statusCB;
        public HttpWebRequest request;
        public String response;
        public String filename;
        public ResponseDoneDelegate responseDoneCB;
        public ExceptionAlertDelegate failureCB;
        public UploadState()
        {
            this.success = false;


        }
    }
}
