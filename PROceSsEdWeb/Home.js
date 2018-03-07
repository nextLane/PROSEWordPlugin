
(function () {
    "use strict";
	var jsonObj;
    var messageBanner;

    // The initialize function must be run each time a new page is loaded.
    Office.initialize = function (reason) {
        $(document).ready(function () {
            // Initialize the FabricUI notification mechanism and hide it
            var element = document.querySelector('.ms-MessageBanner');
            messageBanner = new fabric.MessageBanner(element);
            messageBanner.hideBanner();
			
            //// If not using Word 2016, use fallback logic.
            //if (!Office.context.requirements.isSetSupported('WordApi', '1.1')) {
            //    $("#template-description").text("This sample displays the selected text.");
            //    $('#button-text').text("Display!");
            //    $('#button-desc').text("Display the selected text");
                
            //    $('#highlight-button').click(displaySelectedText);
            //    return;
            //}

            $("#template-description").text("Learns and generates program from the examples provided below.");
            $('#button-text').text("Add!");
			$('#button-desc').text("Adds examples to be learned!");
			$('#clear-button-text').text("Clear All");
			$('#clear-button-desc').text("Clears all the examples");
			$('#learn-button-text').text("Learn! Generate program");
			$('#learn-button-desc').text("Interacts with PROSE to learn based on examples!");
			$('#examples-text-area').empty();

			$('#add-button').click(addExample);
			$('#clear-button').click(clearAllExamples);
			$('#learn-button').click(sendLearnRequest);
            loadSampleData();
			//displaySelectedText();
            // Add a click event handler for the highlight button.
            //$('#highlight-button').click(hightlightLongestWord);
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
                "This is a sample text inserted in the document",
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
				})
				.then(context.sync);
		})
			.catch(errorHandler);
	} 
	function createJsonInput() {
		
		Word.run(function (context) {
			var doc = context.document.body;
			context.load(doc, 'text');
			return context.sync()
				.then(function () {


					var allText = doc.text;
					var allInputs = $('#examples-text-area').val().split("\n");
					//$('#examples-text-area').append(allInputs[0].split(":")[0]);
					//$('#examples-text-area').append(allInputs[1].split(":")[0]);
					//$('#examples-text-area').append(allInputs[2].split(":")[0]);

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

				})
				.then(context.sync);
		})
			.catch(errorHandler);
		
		//return JSON.stringify(jsonObj);

	}
	function sendLearnRequest() {

		createJsonInput();
		//var data = "\r\n{\r\n\t\"trainInput\":\"{\r\n\r\n  \\\"datatype\\\": \\\"local\\\",\r\n\r\n  \\\"data\\\": [\r\n\r\n    {\r\n\r\n      \\\"Name\\\": \\\"John\\\",\r\n\r\n      \\\"status\\\": \\\"To Be Processed\\\",\r\n\r\n      \\\"LastUpdatedDate\\\": \\\"2013-05-31 08:40:55.0\\\"\r\n\r\n    },\r\n\r\n    {\r\n\r\n      \\\"Name\\\": \\\"Paul\\\",\r\n\r\n      \\\"status\\\": \\\"To Be Processed\\\",\r\n\r\n      \\\"LastUpdatedDate\\\": \\\"2013-06-02 16:03:00.0\\\"\r\n\r\n    }\r\n\r\n  ]\r\n\r\n}\",\r\n\"trainOutput\" :\"[\r\n\r\n    {\r\n\r\n      \\\"John\\\" : \\\"To Be Processed\\\"\r\n\r\n    },\r\n\r\n    {\r\n\r\n      \\\"Paul\\\" : \\\"To Be Processed\\\"\r\n\r\n    }\r\n\r\n  ]\"\r\n}";
		var prog = "";
		var xhr = new XMLHttpRequest();
		xhr.withCredentials = true;

		xhr.addEventListener("readystatechange", function () {
			if (this.readyState === 4) {
				prog = this.responseText;
				$('#examples-text-area').empty();
				$('#examples-text-area').val(prog);
				console.log(prog);
			}
		});

		xhr.open("POST", "http://localhost:2217/api/textextract/learn");
		xhr.setRequestHeader("content-type", "application/json; charset=utf-8");
		xhr.setRequestHeader("cache-control", "no-cache");
		xhr.setRequestHeader("Access-Control-Allow-Origin", "*");
		xhr.send(JSON.stringify(jsonObj));
		prog = xhr.response;
		

		Word.run(function (context) {
			//$('#examples-text-area').empty();
			//$('#examples-text-area').append(data);
			$('#examples-text-area').append(xhr.responseType);
			return context.sync();
		})
			.catch(errorHandler);
	} 

    function hightlightLongestWord() {
        Word.run(function (context) {
            // Queue a command to get the current selection and then
            // create a proxy range object with the results.
            var range = context.document.getSelection();
            
            // This variable will keep the search results for the longest word.
            var searchResults;
            
            // Queue a command to load the range selection result.
            context.load(range, 'text');

            // Synchronize the document state by executing the queued commands
            // and return a promise to indicate task completion.
            return context.sync()
                .then(function () {
                    // Get the longest word from the selection.
                    var words = range.text.split(/\s+/);
                    var longestWord = words.reduce(function (word1, word2) { return word1.length > word2.length ? word1 : word2; });

                    // Queue a search command.
                    searchResults = range.search(longestWord, { matchCase: true, matchWholeWord: true });

                    // Queue a commmand to load the font property of the results.
                    context.load(searchResults, 'font');
                })
                .then(context.sync)
                .then(function () {
                    // Queue a command to highlight the search results.
                    searchResults.items[0].font.highlightColor = '#FFFF00'; // Yellow
                    searchResults.items[0].font.bold = true;
                })
                .then(context.sync);
        })
        .catch(errorHandler);
    } 


    function displaySelectedText() {
        Office.context.document.getSelectedDataAsync(Office.CoercionType.Text,
            function (result) {
                if (result.status === Office.AsyncResultStatus.Succeeded) {
                    showNotification('The selected text is:', '"' + result.value + '"');
                } else {
                    showNotification('Error:', result.error.message);
                }
            });
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
