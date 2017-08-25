import bpy	
import io_scene_fbx.export_fbx_bin
import os
import math
from mathutils import Matrix
from functools import cmp_to_key
from bpy_extras.io_utils import axis_conversion

outfile = os.getenv("UNITY_BLENDER_EXPORTER_OUTPUT_FILE")

class FakeOp:
	def report(self, tp, msg):
		print("%s: %s" % (tp, msg))

def children_selection_recursive(obj_parent, value):
    for obj in obj_parent.children:
        children_selection_recursive(obj, value)
        obj.select = value

for obj in bpy.data.objects:
	if obj.parent is not None:
		continue;
	obj.select = False if obj.name[0] in '_.' else True
	children_selection_recursive(obj, obj.select)

kwargs = dict(
	global_matrix=axis_conversion(to_forward='-Z',to_up='Y').to_4x4(),

	axis_up = 'Y',
	axis_forward = '-Z',
	bake_space_transform = True,

	version = 'BIN7400',
	use_selection = True,
	object_types = {'MESH', 'ARMATURE'},
	use_mesh_modifiers = True,
	mesh_smooth_type = 'OFF',
	use_mesh_edges = False,
	use_tspace = False,
	use_custom_props = False,
	add_leaf_bones = True,
	primary_bone_axis = 'Y',
	secondary_bone_axis = 'X',
	use_armature_deform_only = False,
	bake_anim = True,
	bake_anim_use_all_bones = True,
	bake_anim_use_nla_strips = True,
	bake_anim_use_all_actions = True,
	bake_anim_step = 1.0,
	bake_anim_simplify_factor = 1.0,
	use_anim = True,
	use_anim_action_all = True,
	use_default_take = True,
	use_anim_optimize = True,
	anim_optimize_precision = 6.0,
	path_mode = 'AUTO',
	embed_textures = False,
	batch_mode = 'OFF',
	use_batch_own_dir = True,
)

io_scene_fbx.export_fbx_bin.save(FakeOp(), bpy.context, filepath=outfile, **kwargs)

print("Finished blender to FBX conversion " + outfile)


