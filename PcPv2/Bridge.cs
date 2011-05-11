using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.IO;
using System.Collections.Specialized;


namespace PcPv2
{
    class Bridge
    {
        private string username = null; // username for Podcast producer
        private string password = null; // password for podcast producer
        private string server = null; // hostname (or ip address) of PcPBridge.

        public string Server
        {
            get { return server; }
            set { this.server = value; }
        }

        public string Password
        {
            private get { return password; }
            set { this.password = value; }
        }
        public string Username
        {
            get { return username; }
            set { this.username = value; }
        }
        public Bridge(string username, string password, string server)
        {
            this.username = username;
            this.password = password;
            this.server = server;
        }

        public String retrieveWorkflows()
        {
            // this is what we are sending
            string post_data_pattern = "user={0}&password={1}";
            string post_data = String.Format(post_data_pattern, username, password);
            // this is where we will send it
            string uri_pattern = "https://{0}/pcpbridge/workflows";
            string uri = String.Format(uri_pattern, server);

            // Create a request using a URL that can receive a post. 
            WebRequest request = WebRequest.Create(uri);
            request.Method = "POST";
            byte[] byteArray = Encoding.UTF8.GetBytes(post_data);
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;

            // Send the post request
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            // Get the response.
            WebResponse response = request.GetResponse();
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            return responseFromServer;

        }

        public NameValueCollection bundleFile(string uuid, string file_path, string title, string description)
        {
            NameValueCollection upload_components = new NameValueCollection();

            upload_components.Add("packed_file", Path.GetTempFileName());


            // Construct the information for the form fields in the submit web page.
            NameValueCollection nvc = new NameValueCollection();
            nvc.Add("user", username);
            nvc.Add("password", password);
            nvc.Add("title", title);
            nvc.Add("description", description);
            nvc.Add("uuid", uuid);




            // Construct the multipart boundary
            string boundary = "----------------------------" + DateTime.Now.Ticks.ToString("x");
            upload_components.Add("Boundary", boundary);

            // Create a stream to write the encoded request contents to.
            //Stream requestBodyStream = new System.IO.MemoryStream();
            Stream requestBodyStream = File.OpenWrite(upload_components.Get("packed_file"));
            // Prepare the ASCII boundary
            byte[] boundarybytes = System.Text.Encoding.ASCII.GetBytes("\r\n--" + boundary + "\r\n");


            // Encode the non-file fields into the request body.
            string formdataTemplate = "\r\n--" + boundary + "\r\nContent-Disposition: form-data; name=\"{0}\";\r\n\r\n{1}";
            foreach (string key in nvc.Keys)
            {
                string formitem = string.Format(formdataTemplate, key, nvc[key]);
                byte[] formitembytes = System.Text.Encoding.UTF8.GetBytes(formitem);
                requestBodyStream.Write(formitembytes, 0, formitembytes.Length);
            }

            // Add in the multipart boundary 
            requestBodyStream.Write(boundarybytes, 0, boundarybytes.Length);

            // Add in the file header
            string headerTemplate = "Content-Disposition: form-data; name=\"{0}\"; filename=\"{1}\"\r\nContent-Type: application/octet-stream\r\n\r\n";
            string header = string.Format(headerTemplate, "file", System.IO.Path.GetFileName(file_path));
            byte[] headerbytes = System.Text.Encoding.UTF8.GetBytes(header);
            requestBodyStream.Write(headerbytes, 0, headerbytes.Length);

            // Read the upload file, writing it into the request body.
            byte[] buffer = new byte[1024];
            int bytesRead = 0;
            FileStream fileStream = new FileStream(file_path, FileMode.Open, FileAccess.Read);
            while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
            {
                requestBodyStream.Write(buffer, 0, bytesRead);
            }
            fileStream.Close();

            // Write the closing multipart boundary
            requestBodyStream.Write(boundarybytes, 0, boundarybytes.Length);
            requestBodyStream.Close();
            return upload_components;
        }

        public void sendFile(string uuid, string file_path, string title, string description, UploadState state)
        {
            state.statusCB("Encoding File");
            NameValueCollection upload_components = bundleFile(uuid, file_path, title, description);
            FileStream encodedFileStream = File.OpenRead(upload_components.Get("packed_file"));

            // this is where we will send it
            string uri_pattern = "https://{0}/pcpbridge/upload";
            string uri = String.Format(uri_pattern, server);
            long bufferLength = encodedFileStream.Length;

            // Create the request object 
            HttpWebRequest bridgeSubmitRequest = (HttpWebRequest)WebRequest.Create(uri);
            bridgeSubmitRequest.ContentType = "multipart/form-data; boundary=" + upload_components.Get("Boundary");
            bridgeSubmitRequest.Method = "POST";
            bridgeSubmitRequest.KeepAlive = true;
            bridgeSubmitRequest.Credentials = System.Net.CredentialCache.DefaultCredentials;
            bridgeSubmitRequest.ContentLength = bufferLength;
            state.request = bridgeSubmitRequest;
            state.filename = upload_components.Get("packed_file");
            state.totalBytes = bufferLength;

            encodedFileStream.Close();
            bridgeSubmitRequest.BeginGetRequestStream(new AsyncCallback(postFile_GetRequestStreamCallback), state);
        }

        private void postFile_GetRequestStreamCallback(IAsyncResult asynchronousResult)
        {
            UploadState state = (UploadState)asynchronousResult.AsyncState;
            state.statusCB("Sending File");
            // End the operation
            try
            {
                Stream postStream = state.request.EndGetRequestStream(asynchronousResult);
                const int BUFFER = 4096;

                byte[] buffer = new byte[BUFFER];
                FileStream encodedFileStream = File.OpenRead(state.filename);
                int bytes_read = 0;
                long total_read = 0;
                bytes_read = encodedFileStream.Read(buffer, 0, BUFFER);
                while (bytes_read > 0)
                {
                    total_read += bytes_read;
                    postStream.Write(buffer, 0, bytes_read);
                    bytes_read = encodedFileStream.Read(buffer, 0, BUFFER);
                    state.progCB(((double)total_read / (double)state.totalBytes) * 100.0);
                }

                postStream.Close();
                encodedFileStream.Close();
                File.Delete(state.filename);
            }

            catch (Exception e)
            {
                state.failureCB(e,"Upload Problem");
                return;
            }
            state.uploadDoneCB();
            // Start the asynchronous operation to get the response
            state.request.BeginGetResponse(new AsyncCallback(GetResponseCallback), state);
        }

        private static void GetResponseCallback(IAsyncResult asynchronousResult)
        {
            UploadState state = (UploadState)asynchronousResult.AsyncState;
            state.statusCB("Waiting For Response");

            // End the operation
            try
            {
                HttpWebResponse response = (HttpWebResponse)state.request.EndGetResponse(asynchronousResult);
                Stream streamResponse = response.GetResponseStream();
                StreamReader streamRead = new StreamReader(streamResponse);
                state.response = streamRead.ReadToEnd();
                // Close the stream object
                streamResponse.Close();
                streamRead.Close();
                response.Close();
            }
            catch (Exception e)
            {
                state.failureCB(e, "Result Retrieval Problem");
                return;
            }
            // Release the HttpWebResponse
           
            state.statusCB("Done");
            state.responseDoneCB();

        }


    }
}
