extends CharacterBody3D

@export var look_sensitivity : float = 0.006

func _ready():
	for child in get_node("WorldModel").find_children("*", "VisualInstance3D"):
		child.set_layer_mask_value(1, false)
		child.set_layer_mask_value(2, true)
	
func _unhandled_input(event):
	if event is InputEventMouseButton:
		Input.set_mouse_mode(Input.MOUSE_MODE_CAPTURED)
	elif event.is_action_pressed("ui_cancel"):
		Input.set_mouse_mode(Input.MOUSE_MODE_VISIBLE)
		
	if Input.get_mouse_mode() == Input.MOUSE_MODE_CAPTURED:
		if event is InputEventMouseMotion:
			rotate_y(-event.relative.x * look_sensitivity)
			get_node("Head").rotate_x(-event.relative.y * look_sensitivity)
			get_node("Head").rotation.x = clamp(get_node("Head").rotation.x, deg_to_rad(-90), deg_to_rad(90))
func _physics_process(delta):
	pass
	
func _process(delta):
	pass
