for OBJ in Geometry do
(
	select OBJ
	resetxform OBJ
	maxOps.CollapseNode OBJ false
	
	OBJ.pivot = [0, 0, 0]
)