extends CharacterBody3D

@export var look_sensitivity : float = 0.006
@export var jump_velocity := 6.0
@export var auto_bhop := true
@export var walk_speed := 7.0
@export var sprint_speed := 8.5
@export var ground_accel  := 14.0
@export var ground_decel := 10.0
@export var ground_friction := 6.0

@export var air_cap := 0.85
@export var air_accel := 800.0
@export var air_move_speed := 500.0

var wish_dir := Vector3.ZERO

func get_move_speed() -> float:
	return sprint_speed if Input.is_action_pressed("sprint") else walk_speed

func _ready():
	print("ahhh")
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
			get_node("Head").rotate_x(event.relative.y * look_sensitivity)
			get_node("Head").rotation.x = clamp(get_node("Head").rotation.x, deg_to_rad(-90), deg_to_rad(90))
func _physics_process(delta):
	var input_dir = Input.get_vector("left", "right", "up", "down").normalized()
	wish_dir = self.global_transform.basis * Vector3(-input_dir.x, 0., -input_dir.y)
	if is_on_floor():
		if Input.is_action_just_pressed("jump"):
			self.velocity.y = jump_velocity
		_handle_ground_physics(delta)
	else:
		_handle_air_physics(delta)
		
	move_and_slide()
	
func _handle_ground_physics(delta) -> void:
	var cur_speed_in_wish_dir = self.velocity.dot(wish_dir)
	var add_speed_till_cap = get_move_speed() - cur_speed_in_wish_dir
	if add_speed_till_cap > 0:
		var accel_speed = ground_accel * delta * get_move_speed()
		accel_speed = min(accel_speed, add_speed_till_cap)
		self.velocity += accel_speed * wish_dir
		
	var control = max(self.velocity.length(), ground_decel)	
	var drop = control * ground_friction * delta
	var new_speed = max(self.velocity.length() - drop, 0.0)
	if self.velocity.length() > 0:
		new_speed /= self.velocity.length()
	self.velocity *= new_speed	
	
	
func _handle_air_physics(delta) -> void:
	self.velocity.y -= ProjectSettings.get_setting("physics/3d/default_gravity") * delta
	var cur_speed_in_wish_dir = self.velocity.dot(wish_dir)
	var capped_speed = min((air_move_speed * wish_dir).length(), air_cap)
	var add_speed_till_cap = capped_speed - cur_speed_in_wish_dir
	if add_speed_till_cap > 0:
		var accel_speed = air_accel * air_move_speed * delta
		accel_speed = min(accel_speed, add_speed_till_cap)
		self.velocity += accel_speed * wish_dir
	
func _process(delta):
	pass
