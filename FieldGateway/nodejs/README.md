Steps to run Gateway modules:

1. Setup IoTHub
2. Generate Device Connection String
3. Setup Device Explorer 
4. Start Data Monitor
5. Send Message to Device, Messages include:
	        - compiled query with Json format, there are two examples: gateway-asa-demo/selectstar.json and gateway-asa-demo/selectstartempgreaterthan80
                - url of Blob, where compiled query stored, example: https://xinstorage.blob.core.windows.net/xincontainer/myblob
6. Check Device Explorer for responses
