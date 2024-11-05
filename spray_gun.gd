extends Node3D

		
func _fire() -> void:
	print("fire")

func _unhandled_input(event):
	if Input.is_action_just_pressed("fire"):
		_fire()
