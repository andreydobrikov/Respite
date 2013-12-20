for ambient  in Geometry do
(
	print ("Mapping " + ambient.name)
	
	-- Select the current object and tell Max it's in modify mode
	select ambient
	max modify mode
	
	-- Bung a UVW modifier on it
	addModifier ambient (Unwrap_UVW())

	-- Grab all the mesh's faces
	faces = ambient.Faces as bitarray

	-- Modify channel 2
	ambient.unwrap_uvw.unwrap.setMapChannel 2
		
	-- Modify faces
	ambient.Unwrap_UVW.setTVSubObjectMode 3
		
	-- Select all the faces
	ambient.Unwrap_UVW.unwrap2.selectFaces faces
		
	
	
	-- I think this may be useless, but I dare not remove it
	ambient.Unwrap_UVW.unwrap.planarMap()
		
	-- This does the actual mapping. Mode 1 is planar-mapping	
	ambient.Unwrap_UVW.unwrap5.mappingMode 1	
		
		-- Use "best-fit" projection. Not vital in 2D	
	ambient.Unwrap_UVW.unwrap.setProjectionType 2
		
		ambient.Unwrap_UVW.unwrap5.mappingAlign 1
		
		ambient.Unwrap_UVW.flipHorizontal()
		
	-- Collapse that modifiers stack, sir
	maxOps.CollapseNode ambient false
)
