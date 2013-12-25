for OBJ in Geometry do
(
	pattern = "facility0_"
	if matchPattern OBJ.name pattern:(pattern + "?*") do
	(
		objectName = substring OBJ.name pattern.count OBJ.name.count
		OBJ.name = ("facility1" + objectName)
	)
)