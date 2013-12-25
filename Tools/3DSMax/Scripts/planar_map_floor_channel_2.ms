for OBJ in Geometry do
(
	suffixPosition = findString OBJ.name "_floor"
	
	if suffixPosition != undefined do
	(
		select OBJ
		max modify mode
	
		-- Bung a UVW modifier on it
		addModifier OBJ (Unwrap_UVW())
			
		OBJ.unwrap_uvw.unwrap.setMapChannel 2
			
			-- Grab all the mesh's faces
			faces = OBJ.Faces as bitarray

			-- Modify faces
			OBJ.Unwrap_UVW.setTVSubObjectMode 3
				
			-- Select all the faces
			OBJ.Unwrap_UVW.unwrap2.selectFaces faces
				
			
			
			-- I think this may be useless, but I dare not remove it
			OBJ.Unwrap_UVW.unwrap.planarMap()
				
			-- This does the actual mapping. Mode 1 is planar-mapping	
			OBJ.Unwrap_UVW.unwrap5.mappingMode 1	
				
				-- Use "best-fit" projection. Not vital in 2D	
			OBJ.Unwrap_UVW.unwrap.setProjectionType 2
				
				OBJ.Unwrap_UVW.unwrap5.mappingAlign 1
				
			-- Collapse that modifiers stack, sir
			maxOps.CollapseNode OBJ false
	)
	
)