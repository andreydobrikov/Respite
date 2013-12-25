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
	
	if suffixPosition != undefined do
	(
		objectName = substring OBJ.name 1 (suffixPosition - 1)
		print objectName
		
		ambientExists = false
		for ambient  in ambientObjects do
		(
			if (findString ambient.name objectName ) != undefined do
			(
				ambientExists = true
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
		
	-- Use "best-fit" projection. Not vital in 2D	
	ambient.Unwrap_UVW.unwrap.setProjectionType 4
	
	-- I think this may be useless, but I dare not remove it
	ambient.Unwrap_UVW.unwrap.planarMap()
		
	-- This does the actual mapping. Mode 1 is planar-mapping	
	ambient.Unwrap_UVW.unwrap5.mappingMode 1	
		
	-- Collapse that modifiers stack, sir
	maxOps.CollapseNode ambient false
)
