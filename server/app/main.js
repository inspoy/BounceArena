const net = require("net");
net.createServer(function(socket) {
	console.log("有新的连接");
	socket.on("data", function(data) {
		console.log("request: " + data);
		socket.write("收到了：" + data);
	});
	socket.on("end", function(data) {
		console.log("socket end");
	});
	socket.on("close", function(data) {
		console.log("连接已断开");
	});
	socket.write("Hello!");
	setTimeout(function() {
		socket.destroy()
	}, 2000);
}).listen(12345);
console.log("started");
