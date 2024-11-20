extends Node3D

		
var firing := false
@export var particles_node: GPUParticles3D

func _fire() -> void:
	firing = true
	particles_node.emitting = true
	print("fire")

func _stop_firing() -> void:
	firing = false
	particles_node.emitting = false

func _unhandled_input(event):
	if Input.is_action_pressed("fire"):
		_fire()
	if Input.is_action_just_released("fire"):
		_stop_firing()
