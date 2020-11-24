# RevitToIndoorGML
This is a plugin on Revit 2019 for exporting spatial data [boundary, space, connection between door to room] to IndoorGML

## Environment 
> Windows 64-bit

> NET framework 4.5

> Visual studio 2019 

## Architecture 
#### IndoorGML.Exporter.Revit
> IndoorGML Revit Plugin

#### IndoorGML.Core
> Core base class of IndoorGML XML
- Reading IndoorGML
- Writing IndoorGML
- Combining IndoorGMLs

#### IndoorGML.Apdater
> Received the spatial & connection information export to IndoorGML format




## Solution file for build
> Open solution & and do the nuget to restore the dependency thirdparty 

> IndoorGML.Exporter.Revit\InDoorGML.Exporter.Revit.sln

## Plugin install
> After build solution sucessfull, you could register the plugin by
- Update Assembly path in IndoorGMLExporter.2019.addin to new dll locate
- Copy "IndoorGML.Exporter.Revit\IndoorGMLExporter.2019.addin"  
- To C:\ProgramData\Autodesk\Revit\Addins\2019

## Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

Please make sure to update tests as appropriate.

## License
[MIT](https://choosealicense.com/licenses/mit/)


## Author
> Intratech Corp 2020 

>> Website: https://intratech3d.com

>> Email: info@intra.co.kr