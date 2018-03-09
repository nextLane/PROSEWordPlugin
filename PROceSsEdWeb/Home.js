
(function () {
    "use strict";
	var jsonObjRun;
    var messageBanner;
	var progGen;
	var resValues;
	var extractResults = [];
    // The initialize function must be run each time a new page is loaded.
    Office.initialize = function (reason) {
        $(document).ready(function () {
            // Initialize the FabricUI notification mechanism and hide it
            var element = document.querySelector('.ms-MessageBanner');
            messageBanner = new fabric.MessageBanner(element);
            messageBanner.hideBanner();
			
            $("#template-description").text("Learns and generates program from the examples provided below.");
            $('#button-text').text("Add!");
			$('#button-desc').text("Adds examples to be learned!");
			$('#clear-button-text').text("Clear All");
			$('#clear-button-desc').text("Clears all the examples");
			$('#learn-button-text').text("Learn and Run!");
			$('#learn-button-desc').text("Interacts with PROSE to learn based on examples and then runs on the document!");
			$('#examples-text-area').empty();

			$('#add-button').click(addExample);
			$('#clear-button').click(clearAllExamples);
			$('#learn-button').click(sendLearnRunRequest);

            loadSampleData();
			
        });
    };

    function loadSampleData() {
        // Run a batch operation against the Word object model.
        Word.run(function (context) {
            // Create a proxy object for the document body.
            var body = context.document.body;

            // Queue a commmand to clear the contents of the body.
            body.clear();
            // Queue a command to insert text into the end of the Word document body.
            body.insertText(
                "Someone is Sumit Gulwani\rSomeone is Harry Potter\rSomeone is Aditi Bhatnagar\rSomeone is Karthik Raman\rSomeone is Dumbledore",
                Word.InsertLocation.end);

            // Synchronize the document state by executing the queued commands, and return a promise to indicate task completion.
            return context.sync();
        })
        .catch(errorHandler);
	}

	function addExample() {
		Word.run(function (context) {
			// Queue a command to get the current selection and then
			// create a proxy range object with the results.
			var range = context.document.getSelection();
			var doc = context.document.body;
			// This variable will keep the search results for the longest word.
			var searchResults;

			// Queue a command to load the range selection result.
			context.load(range, 'text');
			context.load(doc,'text')

			// Synchronize the document state by executing the queued commands
			// and return a promise to indicate task completion.
			return context.sync()
				.then(function () {
					//$('#examples-text-area').append(range.text);
					var str = doc.text;
					var selText = range.text;
					var start = str.indexOf(selText);
					$('#examples-text-area').append(start + ":" + (start + selText.length));
					$('#examples-text-area').append("\n");
				})
				.then(context.sync);
		})
			.catch(errorHandler);
	} 

	function clearAllExamples() {
		Word.run(function (context) {
			return context.sync()
				.then(function () {

					$('#examples-text-area').empty();
					var jsonObj = null;
					var jsonObjRun = null;
					var progGen = null;
				})
				.then(context.sync);
		})
			.catch(errorHandler);
	} 
	 function createJsonInput(callback) {
		
		 Word.run(function (context) {
			 var jsonObj;
			var doc = context.document.body;
			context.load(doc, 'text');
			return context.sync()
				.then(function () {


					var allText = doc.text;
					var allInputs = $('#examples-text-area').val().split("\n");

					jsonObj = {
						"examples": [
							{
								"text": allText,
								"selections": []
							}
						],
						"type": "Sequence"
					};
					var tempObj = new Object();
					for (var i = 0; i < allInputs.length; i++) {
						if (!allInputs[i] || 0 === allInputs[i].length)
							continue;
						var start = "startPos";
						var end = "endPos";
						tempObj = new Object();
						tempObj[start] = allInputs[i].split(":")[0];
						tempObj[end] = allInputs[i].split(":")[1];
						jsonObj.examples[0].selections.push(tempObj);
					}

					$('#examples-text-area').append(JSON.stringify(jsonObj));
					callback(jsonObj);
				})
				.then(context.sync);
		})
			.catch(errorHandler);
	}

	function sendXHR(jsonObject)
	{
		var response = "";
		var jsonResp;
		var xhr = new XMLHttpRequest();
		xhr.withCredentials = true;


		xhr.addEventListener("readystatechange", function () {
			if (this.readyState === 4) {
				response = this.responseText;
				$('#examples-text-area').empty();
				$('#examples-text-area').val(response);

			}
		});

		xhr.open("POST", "http://localhost:2217/api/textextract/extract");
		xhr.setRequestHeader("content-type", "application/json; charset=utf-8");
		xhr.setRequestHeader("cache-control", "no-cache");
		xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
		xhr.send(JSON.stringify(jsonObject));

	}
	
	function sendLearnRunRequest() {

		createJsonInput(sendXHR);
		
		Word.run(function (context) {
			return context.sync();
		})
			.catch(errorHandler);
	} 


	function highlightExtractions() {
		Word.run(function (context) {
			var range1 = context.document.body;
			context.load(range1, 'text');
			// Synchronize the document state by executing the queued commands
			// and return a promise to indicate task completion.
			return context.sync()
				.then(function () {
					var value = $('textarea').val();
					value = value.replace(/\\/g, "");
					resValues = value.split('"');
					for (var i = 0; i < resValues.length; i++) {
						if (resValues[i].length < 3) {
							resValues.splice(i, 1);
							i = i - 1;
						}

					}
				})
				.then(context.sync)
				.then(function () {
					$('#examples-text-area').empty();
					$('#examples-text-area').append("************\nExtractions:\n");
					for (var index = 0; index < resValues.length; index++) {
						$('#examples-text-area').append(resValues[index]);
					}

				})
				.then(context.sync);
		})
			.catch(errorHandler);
	}



    //$$(Helper function for treating errors, $loc_script_taskpane_home_js_comment34$)$$
    function errorHandler(error) {
        // $$(Always be sure to catch any accumulated errors that bubble up from the Word.run execution., $loc_script_taskpane_home_js_comment35$)$$
        showNotification("Error:", error);
        console.log("Error: " + error);
        if (error instanceof OfficeExtension.Error) {
            console.log("Debug info: " + JSON.stringify(error.debugInfo));
        }
    }

    // Helper function for displaying notifications
    function showNotification(header, content) {
        $("#notification-header").text(header);
        $("#notification-body").text(content);
        messageBanner.showBanner();
        messageBanner.toggleExpansion();
    }
})();
