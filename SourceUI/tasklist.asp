<%
'	===============================================================================
'	@Programe ID			: tasklist.asp
'	@Programe Name			: Task Sheduler Monitoring
'	@Writer Name			: flux
'	@Write Date				: 2019-01-17
'	@Description			: 
'	@Editor Name			: 
'	@Edit Date				: 
'	===============================================================================

Option Explicit 

Response.Expires = -1 
'Response.ExpiresAbsolute = Now() - 1 
Response.AddHeader "pragma", "no-cache" 
Response.CacheControl = "no-cache" 
Response.Buffer = true 

Dim s : s = request("s")

response.write "# Monitor Server : " & s
%>
<!DOCTYPE html>
<html lang="en">
<head>
    <title id='Description'>ENT Task Sheduler Monitoring</title>
    <link rel="stylesheet" href="./jqwidgets/styles/jqx.base.css" type="text/css" />
	<meta name="google" content="notranslate">
    <meta http-equiv="X-UA-Compatible" content="IE=edge,chrome=1" />
    <meta name="viewport" content="width=device-width, initial-scale=1 maximum-scale=1 minimum-scale=1" />	
    <script type="text/javascript" src="./scripts/jquery-1.12.4.min.js"></script>
    <script type="text/javascript" src="./jqwidgets/jqxcore.js"></script>
    <script type="text/javascript" src="./jqwidgets/jqxbuttons.js"></script>
    <script type="text/javascript" src="./jqwidgets/jqxscrollbar.js"></script>
    <script type="text/javascript" src="./jqwidgets/jqxmenu.js"></script>
    <script type="text/javascript" src="./jqwidgets/jqxgrid.js"></script>
    <script type="text/javascript" src="./jqwidgets/jqxgrid.selection.js"></script> 
    <script type="text/javascript" src="./jqwidgets/jqxgrid.columnsresize.js"></script> 
    <script type="text/javascript" src="./jqwidgets/jqxgrid.sort.js"></script> 
    <script type="text/javascript" src="./jqwidgets/jqxdata.js"></script> 
    <!--<script type="text/javascript" src="./scripts/demos.js"></script>-->
    <script type="text/javascript">
		
		var getParam = function(key){
			var _parammap = {};
			document.location.search.replace(/\??(?:([^=]+)=([^&]*)&?)/g, function () {
				function decode(s) {
					return decodeURIComponent(s.split("+").join(" "));
				}
	 
				_parammap[decode(arguments[1])] = decode(arguments[2]);
			});
	 
			return _parammap[key];
		};

        $(document).ready(function () {

		 var server = "<%=s%>";
	     //var url = "http://127.0.0.1:10080/TaskWeb/Index";
		 var url = "";
		 if(server=="BatchPlay") {
			url = "http://dev.fluxdev.com/Batch/GateAPI/JsonAPI.aspx?p=http://127.0.0.1:10080/TaskWeb/Index";
		 } else if(server=="BatchMovie") {
			url = "http://dev.fluxdev.com/Batch/GateAPI/JsonAPI.aspx?p=http://211.233.74.92:10080/TaskWeb/Index";
		 }
//alert(url);
            // Prepare the response data
            var source =
            {
                datatype: "json",
                datafields: [
                    { name: 'Name', type: 'string' },
                    { name: 'State', type: 'string' },
                    { name: 'Enabled', type: 'string' },
                    { name: 'LastTaskResult', type: 'string' },
                    { name: 'LastRunTime', type: 'string' },
                    { name: 'NextRunTime', type: 'string' }
                ],
                id: 'id',
                url: url
            };
           var dataAdapter = new $.jqx.dataAdapter(source);

            $("#grid").jqxGrid(
            {
                //width: getWidth('Grid'),
                width: 1200,
				height:1500,
                source: dataAdapter,
                //columnsresize: true,
				sortable: true,
                ready: function () {
                    $("#grid").jqxGrid('sortby', 'LastTaskResult', 'desc'); //asc desc
                },
                columns: [
                  { text: 'Name', datafield: 'Name', width: 400 },
                  { text: 'State', datafield: 'State', width: 90},
                  { text: 'Enabled', datafield: 'Enabled', width: 90 },
                  { text: 'LastTaskResult', datafield: 'LastTaskResult', width: 110 },
                  { text: 'LastRunTime', datafield: 'LastRunTime', width: 180 },
                  { text: 'NextRunTime', datafield: 'NextRunTime', width: 180 }
              ]
            });
        });
    </script>
</head>
<body class='default'>
        <div id="grid"></div>

</body>
</html>
