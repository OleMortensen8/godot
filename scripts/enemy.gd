extends CharacterBody3D

# Export a NodePath for setting the target position from the editor
@export var targetPositionNodePath: NodePath
# Export movement speed
@export var movementSpeed: float = 2.0

# Store the target position
var movement_target_position: Vector3

@onready var navigation_agent: NavigationAgent3D = $NavigationAgent3D

func _ready():
	# These values need to be adjusted for the actor's speed
	# and the navigation layout.
	navigation_agent.path_desired_distance = 0.5
	navigation_agent.target_desired_distance = 0.5

	# Make sure to not await during _ready.
	actor_setup()

func actor_setup():
	# Set the movement target from the exported NodePath
	if targetPositionNodePath:
		var targetNode = get_node_or_null(targetPositionNodePath)
		if targetNode:
			movement_target_position = targetNode.global_transform.origin

func set_movement_target(movement_target: Vector3):
	navigation_agent.set_target_position(movement_target)

func _physics_process(delta):
	# Continuously update the movement target
	if targetPositionNodePath:
		var targetNode = get_node_or_null(targetPositionNodePath)
		if targetNode:
			movement_target_position = targetNode.global_transform.origin
			set_movement_target(movement_target_position)

	if navigation_agent.is_navigation_finished():
		return

	var current_agent_position: Vector3 = global_position
	var next_path_position: Vector3 = navigation_agent.get_next_path_position()

	var direction: Vector3 = current_agent_position.direction_to(next_path_position)
	velocity = direction.normalized() * movementSpeed
	global_translate(velocity * delta) # Move the character manually
