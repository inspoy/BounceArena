const net = require("net");
net.createServer(function(socket) {
	socket.on("data", function(data) {
		console.log("收到了：" + data);
		socket.write("收到了：" + data);
	});
	socket.write("Hello!");
}).listen(12345);
