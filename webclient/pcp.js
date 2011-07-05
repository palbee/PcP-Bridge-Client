// Copyright (c) 2011, Paul Albee 
// All rights reserved. 
// 
// Redistribution and use in source and binary forms, with or without 
// modification, are permitted provided that the following conditions are met: 
// 
//  * Redistributions of source code must retain the above copyright notice, 
//    this list of conditions and the following disclaimer. 
//  * Redistributions in binary form must reproduce the above copyright 
//    notice, this list of conditions and the following disclaimer in the 
//    documentation and/or other materials provided with the distribution. 
//  * Neither the name of  nor the names of its contributors may be used to 
//    endorse or promote products derived from this software without specific 
//    prior written permission. 
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE 
// IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE 
// ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE 
// LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR 
// CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS 
// INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN 
// CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) 
// ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE 
// POSSIBILITY OF SUCH DAMAGE.


// Dojo is copyrighted by the Dojo Foundation. License information can be obtained at
// http://bugs.dojotoolkit.org/browser/dojo/trunk/LICENSE
// Requires
dojo.require("dijit.Dialog");
dojo.require("dijit.form.Button");
dojo.require("dijit.form.Select");
dojo.require("dojo.io.iframe");
dojo.require("dojo.string");
dojo.require("dojox.json.ref");
dojo.require("dojox.widget.Standby");
dojo.require("dojox.xml.parser");

//var base_url = "http://127.0.0.1:9000/pcpbridge/"
var base_url = "https://bridge.cmich.edu/pcpbridge/"
var workflow_url = base_url + "workflows"
var upload_url = base_url + "upload"
var standby;
// Helpers
var errorDialog;
dojo.addOnLoad(function() {
    errorDialog = new dijit.Dialog({
        title: "PcP Error",
        style: "min-width: 300px"
        });
});

dojo.ready(function(){

    standby = new dojox.widget.Standby({target: "formNode"});
    document.body.appendChild(standby.domNode);
    var retrieveWorkflowsButton = dojo.byId("retrieveWorkflowsButton"),
        uploadFileButton = dojo.byId("uploadFileButton");
    
    dojo.connect(retrieveWorkflowsButton, "onclick", retrieveWorkflows);
    dojo.connect(uploadFileButton, "onclick", uploadFile);
    dojo.connect(dojo.byId("title"), "onchange", uploadEnable);
    dojo.connect(dojo.byId("description"), "onchange", uploadEnable);
    dojo.connect(dojo.byId("title"), "onkeyup", uploadEnable);
    dojo.connect(dojo.byId("description"), "onkeyup", uploadEnable);
    dojo.connect(dojo.byId("file"), "onchange", uploadEnable);
    dojo.connect(dojo.byId("user"), "onkeypress", function(evt){if(evt.keyCode == 13) dojo.byId("password").focus();});
    dojo.connect(dojo.byId("password"), "onkeypress", function(evt){if(evt.keyCode == 13) retrieveWorkflows(evt);});
    dojo.connect(dojo.byId("workflow"), "onchange", function(evt){dojo.byId("uuid").value = dojo.byId("workflow").value;});
    uploadEnable();     
});

function uploadEnable(){
    var titleLen = dojo.byId("title").value.length;
    var descrLen = dojo.byId("description").value.length;
    var workflowCount = dojo.byId('workflow').length;
    var gotFile = dojo.byId('file').value.length == 0;
    var button = dojo.byId("uploadFileButton");
    button.disabled = (titleLen == 0 || descrLen == 0 || workflowCount == 0 | gotFile);
}

function report_errors(error){
    standby.hide();
    errorDialog.attr("content", error.toString());
    errorDialog.show();
    // console.debug(error);
}

function populate_workflows(newContent){
    var options;
    var errors;
    
    // Extract workflow list or errors from newContent.
    dojo.withDoc(newContent, function(){
        errors = dojo.query("div[name='error']");
        if (errors.length == 0){
            errors = ""
            var stringrep = "";
            options = dojo.query("select[name=workflow] *");
            options.forEach(function(node, index, nodeList){stringrep += node.outerHTML;})
            options = stringrep;
        } else {
            errors = errors[0].innerHTML;
        }
    });
    
    if(errors.length != 0){
        printStackTrace();
        report_errors(new Error(errors));
        return;
    }

    // Present the workflows in the interface.
    var updateTarget= dojo.query("#formNode select[name=workflow]");
    updateTarget.empty();
    var tempdiv = dojo.create("div", { innerHTML:options });
    options = tempdiv.innerHTML;
    updateTarget.addContent(options);
    uploadEnable();
    dojo.byId("uuid").value = dojo.byId("workflow").value;
    dojo.query(".auth").addClass("hide");
    dojo.query(".upload").removeClass("hide");
    standby.hide()
}
    
function retrieveWorkflows(evt) {
    standby.show();
    var form = dojo.byId("formNode");
    var sendObj = dojo.io.iframe.send({
         url: workflow_url,
         method:"POST",
         form:form,
         handleAs:"xml",
         load: populate_workflows,
         error: report_errors
     });
    
}

function check_status(newContent){
    var status_text;
    var status_code;
    var upload_uuid;
    dojo.withDoc(newContent, function() {
        status_code = dojo.query('[name="status"]')[0].title;
        status_text = dojo.query('[name="status"]')[0].innerHTML;
        upload_uuid = dojo.query('[name="upload_uuid"]')[0].innerHTML;
    });
    // console.log(status_text);
    // console.log(status_code);
    // console.log(upload_uuid);
    dojo.query("div#results p[name=status]")[0].innerText = "Status : " + status_text + " ("+ status_code +")";
    dojo.query("div#results p[name=upload_uuid]")[0].innerText = "Upload ID : " + upload_uuid;
    dojo.query(".upload").addClass("hide");
    dojo.query(".results").removeClass("hide");
    standby.hide();
}

function uploadFile(evt) {
    standby.show();
    var form = dojo.byId("formNode");
    var sendObj = dojo.io.iframe.send({
          url: upload_url,
          method:"POST",
          form:form,
          handleAs:"xml",
          load: check_status,
          error: report_errors,
          sync: true,
      });
    
}
