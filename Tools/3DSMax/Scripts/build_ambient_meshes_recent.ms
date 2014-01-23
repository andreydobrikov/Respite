ambientObjects = #()

-- Gather the ambient objects
for OBJ in Geometry do
(
	if matchPattern OBJ.name pattern:"?*_ambient" do
	(
			append ambientObjects OBJ
	)
)

for OBJ in Geometry do
(
	suffixPosition = findString OBJ.name "_floor"
	
	if suffixPosition != undefined then
	(
		objectName = substring OBJ.name 1 (suffixPosition - 1)
		print objectName
		
		ambientExists = false
		for ambient  in ambientObjects do
		(
			-- Find the ambient name and make sure it's an exact match for the object name, otherwise odd issues occur,
			-- such as kitchen being matched with kitchen_cleaning_ambient instead of kitchen_ambient
			ambientSuffixPosition = findstring ambient.name "_ambient"
			ambientName = substring ambient.name 1 (ambientSuffixPosition - 1)
			
			if (ambientName ==  objectName ) then
			(
				ambientExists = true
				print ("Found ambient for " + objectName + " at " + ambient.name)
			)
			else
			(
				
			)
		)
		
		if ambientExists == false do
		(
			print (objectName + " Ambient found")
			newObj = copy OBJ
			newObj.name = objectName + "_ambient"
			append ambientObjects newObj
		)
	)
	else
	(
		print ("Ignoring " + OBJ.name)
	)
)
		
-- Apply planar mapping to each quad
for ambient  in ambientObjects do
(
	print ("Mapping " + ambient.name)
	
	-- Select the current object and tell Max it's in modify mode
	select ambient
	max modify mode
	
	-- Bung a UVW modifier on it
	addModifier ambient (Unwrap_UVW())

	-- Grab all the mesh's faces
	faces = ambient.Faces as bitarray

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
