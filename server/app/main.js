const net = require("net");
net.createServer(function(socket) {
	console.log("有新的连接");
	socket.on("data", function(data) {
		console.log("request: " + data);
		socket.write('{"pid":1,"retCode":0}');
	});
	socket.on("end", function(data) {
		console.log("socket end");
	});
	socket.on("close", function(data) {
		console.log("连接已断开");
	});
	socket.write("Hello!");
}).listen(19621);
console.log("started");
