document.addEventListener("DOMContentLoaded", function () {

    const connection = new signalR.HubConnectionBuilder()
        .withUrl(`/ChatHub?chatId=${currentChatId}`)
        .withAutomaticReconnect()
        .build();

    // Get references to UI elements
    const sendButton = document.getElementById("sendButton");
    const messageInput = document.getElementById("message");
    const chatHistoryList = document.getElementById("chatHistory");
    const chatWindow = document.getElementById("chat-window");

    // Disable the send button until the connection is established
    sendButton.disabled = true;

    // Utility function to scroll the chat window to the bottom
    function scrollToBottom() {
        chatWindow.scrollTop = chatWindow.scrollHeight;
    }

    // 3. DEFINE CLIENT-SIDE HUB METHODS

    // This function is called by the hub when a new message is received
    connection.on("ReceiveMessage", function (user, message) {
        const li = document.createElement("li");
        li.className = "mb-2";
        // Note: Using textContent to prevent HTML injection attacks
        li.innerHTML = `<strong>${user}:</strong>`;

        const messageText = document.createElement('span');
        messageText.textContent = ` ${message}`;
        li.appendChild(messageText);

        chatHistoryList.appendChild(li);
        scrollToBottom();
    });

    // This function is called by the hub to load the initial message history
    connection.on("LoadHistory", function (history) {
        chatHistoryList.innerHTML = ''; // Clear any existing messages
        history.forEach(function (item) {
            const li = document.createElement("li");
            li.className = "mb-2";
            li.innerHTML = `<strong>${item.user}:</strong>`;

            const messageText = document.createElement('span');
            messageText.textContent = ` ${item.message}`;
            li.appendChild(messageText);

            chatHistoryList.appendChild(li);
        });
        scrollToBottom();
    });

    // 4. START THE CONNECTION AND WIRE UP EVENTS
    async function start() {
        try {
            await connection.start();
            console.log("SignalR Connected.");
            sendButton.disabled = false;

            // Once connected, invoke the hub method to get the chat history
            await connection.invoke("GetChatHistory", currentChatId);

        } catch (err) {
            console.error(err);
            // Keep the button disabled if connection fails
            setTimeout(start, 5000); // Attempt to reconnect every 5 seconds
        }
    };

    connection.onclose(async () => {
        sendButton.disabled = true;
        console.log("SignalR Connection Closed.");
        await start(); // Attempt to restart the connection
    });

    // Function to send a message
    async function sendMessage() {
        const message = messageInput.value;
        if (message && message.trim() !== "") {
            try {
                // Invoke the "SendMessage" method on the hub
                await connection.invoke("SendMessage", message, currentChatId);
                messageInput.value = ""; // Clear input field
                messageInput.focus();
            } catch (err) {
                console.error(err);
            }
        }
    }

    // Add event listeners
    sendButton.addEventListener("click", sendMessage);
    messageInput.addEventListener("keypress", function (event) {
        if (event.key === "Enter") {
            event.preventDefault(); // Prevent form submission
            sendMessage();
        }
    });

    // Start the connection
    start();
});