    const chatId = "CHAT_ID_FROM_SERVER"; // Replace this dynamically via Razor/view/component
    const connection = new signalR.HubConnectionBuilder().withUrl("/ChatHub").build();

    document.getElementById("sendButton").disabled = true;

    // Display incoming messages
    connection.on("ReceiveMessage", function (user, text, timestamp) {
    const li = document.createElement("li");
    li.textContent = `${user} says: ${text} (${new Date(timestamp).toLocaleTimeString()})`;
    document.getElementById("chatHistory").appendChild(li);
});

    connection.start().then(function () {
    document.getElementById("sendButton").disabled = false;

    // Fetch previous messages for this chat
    fetch(`/api/chat/messages?chatId=${chatId}`)
    .then(res => res.json())
    .then(messages => {
    messages.forEach(m => {
    const li = document.createElement("li");
    li.textContent = `${m.creatorName} says: ${m.text} (${new Date(m.createdAt).toLocaleTimeString()})`;
    document.getElementById("chatHistory").appendChild(li);
});
});
}).catch(function (err) {
    console.error(err.toString());
});

    // Send message
    document.getElementById("sendButton").addEventListener("click", function (event) {
    const message = document.getElementById("message").value;
    connection.invoke("SendMessage", chatId, message).catch(function (err) {
    console.error(err.toString());
});
    event.preventDefault();
});