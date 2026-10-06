extends Node


const PORT: int = 9080
const IGNORED_INTERFACES: Array[String] = [
	"lo",
	"rndis",
	"usb",
	"rmnet",
	"tun",
	"tap",
	"dummy",
	"p2p",
	"docker",
	"vbox",
	"vmnet"
]
# The URL we will connect to.
# Use "ws://localhost:9080" if testing with the minimal server example below.
# `wss://` is used for secure connections,
# while `ws://` is used for plain text (insecure) connections.
@export var websocket_url = "ws://192.168.0.12:9080"
@export var line_edit: LineEdit
@export var scan_button: Button
@export var connection: Control
@export var gamepad: Control

# Our WebSocketClient instance.
var socket = WebSocketPeer.new()

var disconnected: bool = true

var msg: Dictionary = {}


func _ready():
	_on_scan_button_pressed()


func transition_interfaces():
	connection.visible = not connection.visible
	gamepad.visible = not gamepad.visible
	
	if gamepad.visible:
		DisplayServer.screen_set_orientation(DisplayServer.SCREEN_LANDSCAPE)
	else:
		DisplayServer.screen_set_orientation(DisplayServer.SCREEN_PORTRAIT)


func attempt_connection():
	var address: String = "ws://" + line_edit.text + ":9080/general"
	# Initiate connection to the given URL.
	var err = socket.connect_to_url(address)
	if err == OK:
		print("Connecting to %s..." % address)
		# Wait for the socket to connect.
		await get_tree().create_timer(2).timeout

		# Send data.
		print("> Sending test packet.")
		socket.send_text("Test packet")
		transition_interfaces()
		disconnected = false
		set_process(true)
	else:
		push_error("Unable to connect.")
		set_process(false)


func _physics_process(_delta: float) -> void:
	if disconnected:
		return
	# Call this in `_process()` or `_physics_process()`.
	# Data transfer and state updates will only happen when calling this function.
	socket.poll()

	# get_ready_state() tells you what state the socket is in.
	var state = socket.get_ready_state()

	# `WebSocketPeer.STATE_OPEN` means the socket is connected and ready
	# to send and receive data.
	if state == WebSocketPeer.STATE_OPEN:
		#while socket.get_available_packet_count():
			#var packet = socket.get_packet()
			#if socket.was_string_packet():
				#var packet_text = packet.get_string_from_utf8()
				#print("< Got text data from server: %s" % packet_text)
			#else:
				#print("< Got binary data from server: %d bytes" % packet.size())
		if !msg.is_empty():
			socket.send_text(str(msg))
			print(msg)
			
	# `WebSocketPeer.STATE_CLOSING` means the socket is closing.
	# It is important to keep polling for a clean close.
	elif state == WebSocketPeer.STATE_CLOSING:
		pass

	# `WebSocketPeer.STATE_CLOSED` means the connection has fully closed.
	# It is now safe to stop polling.
	elif state == WebSocketPeer.STATE_CLOSED:
		# The code will be `-1` if the disconnection was not properly notified by the remote peer.
		var code = socket.get_close_code()
		print("WebSocket closed with code: %d. Clean: %s" % [code, code != -1])
		set_process(false) # Stop processing.


func is_private_ipv4(addr: String) -> bool:
	if not addr.is_valid_ip_address() or ":" in addr:
		return false
	
	var p := addr.split(".")
	var a := int(p[0])
	var b := int(p[1])
	
	return (
		a == 10
		or (a == 192 and b == 168)
		or (a == 172 and b >= 16 and b <= 31))


func get_local_prefixes() -> Array[String]:
	var prefixes: Array[String] = []
	for interface in IP.get_local_interfaces():
		var interface_name: String = interface["name"].to_lower()
		var friendly: String = str(interface.get("friendly", "")).to_lower()
		var skip: bool = false
		for ignored in IGNORED_INTERFACES:
			if interface_name.begins_with(ignored) or friendly.begins_with(ignored):
				skip = true
				break
		if skip:
			continue
		for addr in interface["addresses"]:
			if is_private_ipv4(addr):
				var parts = addr.split(".")
				var prefix: String = "%s.%s.%s" % [parts[0], parts[1], parts[2]]
				if prefix not in prefixes:
					prefixes.append(prefix)
	
	return prefixes


func find_server(port: int, timeout: float = 1.5) -> String:
	var prefixes := get_local_prefixes()
	var peers: Dictionary[String, StreamPeerTCP] = {}
	for prefix in prefixes:
		for i in range(1, 255):
			var ip := prefix + "." + str(i)
			var p := StreamPeerTCP.new()
			if p.connect_to_host(ip, port) == OK:
				peers[ip] = p
	
	var candidates: Array[String] = []
	var t: float = 0
	while t < timeout and candidates.size() < peers.size():
		await get_tree().process_frame
		t += get_process_delta_time()
		for ip in peers:
			var p := peers[ip]
			p.poll()
			if p.get_status() == StreamPeerTCP.STATUS_CONNECTED and ip not in candidates:
				candidates.append(ip)
	
	for p: StreamPeerTCP in peers.values():
		p.disconnect_from_host()
	
	for ip in candidates:
		var check: bool = await verify(ip, port)
		if check:
			return ip
	
	return ""


func verify(ip: String, port: int = PORT, timeout: float = 2) -> bool:
	var ws := WebSocketPeer.new()
	if ws.connect_to_url("ws://%s:%d/general" % [ip, port]) != OK:
		return false
	
	var t: float = 0
	while t < timeout:
		await get_tree().process_frame
		t += get_process_delta_time()
		ws.poll()
		if ws.get_ready_state() == WebSocketPeer.STATE_OPEN:
			ws.close()
			return true
		elif ws.get_ready_state() == WebSocketPeer.STATE_CLOSED:
			return false
	
	ws.close()
	return false


func _on_connect_button_pressed() -> void:
	attempt_connection()


func _on_scan_button_pressed() -> void:
	scan_button.text = "Scanning..."
	scan_button.disabled = true
	
	var ip: String = await find_server(PORT)
	if ip:
		line_edit.text = ip
	
	scan_button.disabled = false
	scan_button.text = "Scan"


func _on_disconnect_button_pressed() -> void:
	socket.close()
	transition_interfaces()
