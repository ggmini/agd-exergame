extends Node

# The URL we will connect to.
# Use "ws://localhost:9080" if testing with the minimal server example below.
# `wss://` is used for secure connections,
# while `ws://` is used for plain text (insecure) connections.
@export var websocket_url = "ws://192.168.0.12:9080"
#@export var websocket_url = "ws://88.209.32.97"
#@export var websocket_url = "ws://localhost:9080"
@export var text_edit: TextEdit

# Our WebSocketClient instance.
var socket = WebSocketPeer.new()

var msg: Dictionary = {}


func _ready():
	pass


func attempt_connection():
	var address: String = "ws://" + text_edit.text + ":9080/general"
	# Initiate connection to the given URL.
	var err = socket.connect_to_url(address)
	if err == OK:
		print("Connecting to %s..." % address)
		# Wait for the socket to connect.
		await get_tree().create_timer(2).timeout

		# Send data.
		print("> Sending test packet.")
		socket.send_text("Test packet")
	else:
		push_error("Unable to connect.")
		set_process(false)


func _physics_process(delta: float) -> void:
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


func _on_button_pressed() -> void:
	attempt_connection()
