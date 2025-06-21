extends MeshInstance3D

var array_mesh: ArrayMesh

#var verts = PackedVector3Array()
#var uvs = PackedVector2Array()
#var normals = PackedVector3Array()
#var indices = PackedInt32Array()

@export var rings = 50
@export var radial_segments = 50
@export var radius = 16

@export_range(1, 128) var subdivisions := 1 :
	set(new_subdivisions):
		subdivisions = new_subdivisions
		regenerate_mesh()

@export var reload := false :
		set(new_reload):
			reload = false 
			regenerate_mesh()
			
			
func regenerate_mesh() -> void:
	if !array_mesh:
		array_mesh = ArrayMesh.new()
		mesh = array_mesh
	array_mesh.clear_surfaces()
	#var surface_array := generate_cube(subdivisions);
	var surface_array := generate_sphere(subdivisions);
	array_mesh.add_surface_from_arrays(Mesh.PRIMITIVE_TRIANGLES, surface_array)
	
func _ready():
	regenerate_mesh()

func generate_plane(subDivs : int, index: int, direction: Vector3, center: Vector3) -> Array:
	var surfaceArray: Array =  []
	surfaceArray.resize(Mesh.ARRAY_MAX)
	
	var verts := PackedVector3Array()
	var uvs := PackedVector2Array()
	var normals := PackedVector3Array()
	var indices := PackedInt32Array()
	
	direction = direction.normalized()
	var binormal := Vector3(direction.z, direction.x, direction.y) / subDivs
	var tangent := binormal.rotated(direction, PI / 2.0)
	var offset := -subDivs * (binormal + tangent) / 2.0 + center
	
	for x: int in subDivs:
		for y: int in subDivs:
			var vertex_offset := binormal * x + tangent * y + offset
			var index_offset := 4 * (x * subDivs + y) + index
			
			verts.append_array([
				vertex_offset, 
				vertex_offset + tangent, 
				vertex_offset + binormal + tangent, 
				vertex_offset + binormal
			]) 
			normals.append_array([
				direction, direction, direction, direction
			]) 
			indices.append_array([
				index_offset, index_offset + 1, index_offset + 2,
				index_offset, index_offset + 2, index_offset + 3
			])
		
	surfaceArray[Mesh.ARRAY_VERTEX] = verts
	#surfaceArray[Mesh.ARRAY_TEX_UV] = uvs
	surfaceArray[Mesh.ARRAY_NORMAL] = normals
	surfaceArray[Mesh.ARRAY_INDEX] = indices
	
	return surfaceArray
	
func generate_cube(subDivs: int) -> Array:
	var surfaceArray: Array = []
	surfaceArray.resize(Mesh.ARRAY_MAX)
	
	var verts := PackedVector3Array()
	var uvs := PackedVector2Array()
	var normals := PackedVector3Array()
	var indices := PackedInt32Array()
	
	const directions: PackedVector3Array = [
		Vector3.UP, Vector3.DOWN, Vector3.LEFT, Vector3.RIGHT, Vector3.FORWARD, Vector3.BACK
	]
	
	for i: int in directions.size():
		var index = 4 * i * subDivs *  subDivs
		var plane = generate_plane(subDivs, index, directions[i], directions[i] / 2.0)
		verts.append_array(plane[Mesh.ARRAY_VERTEX])
		normals.append_array(plane[Mesh.ARRAY_NORMAL])
		indices.append_array(plane[Mesh.ARRAY_INDEX])
		
	surfaceArray[Mesh.ARRAY_VERTEX] = verts
	#surfaceArray[Mesh.ARRAY_TEX_UV] = uvs
	surfaceArray[Mesh.ARRAY_NORMAL] = normals
	surfaceArray[Mesh.ARRAY_INDEX] = indices
	
	return surfaceArray
	
func generate_sphere(subDivs: int) -> Array:
	var surfaceArray := generate_cube(subDivs)
	
	for i: int in surfaceArray[Mesh.ARRAY_VERTEX].size():
		var vertex: Vector3 = surfaceArray[Mesh.ARRAY_VERTEX][i]
		surfaceArray[Mesh.ARRAY_VERTEX][i] = vertex.normalized() / 2.0
		surfaceArray[Mesh.ARRAY_NORMAL][i] = vertex.normalized()
		
	return surfaceArray 
