To install the web client.
1) Put blank.html and pcp_webclient_new.html where web pages are served from. 
2) Put pcp.js in the javascript directory. 
3) In pcp_webclient_new.html edit the line 'dojoBlankHtmlUrl : "blank.html",' to reflect the absolute url for 'blank.html'. 
4) in pcp_webclient_new.html edit the line '<script src="pcp.js" type="text/javascript"></script>' to reflect the url for 'pcp.js'.


This software used the Google CDN for getting the Dojo libraries. If you wish to use a local copy of Dojo, install version 1.6.0 and then change the relevant script and link entries.