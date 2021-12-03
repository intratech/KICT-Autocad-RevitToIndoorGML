
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.ExtensibleStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IndoorGML.Exporter.Revit.Utils
{
       public class ExternalData
    {
        private string roomTypeSchema = "IndoorRoomType";
        private string roomFunctionSchema = "IndoorRoomFunction";
        private string indoorSchema = "IndoorGML";
        private Guid guid = new Guid("528a78f9-2990-46da-918a-541852693653");

        private Schema schema;
        private Schema GetSchema()
        {
            
            SchemaBuilder schemaBuilder = new SchemaBuilder(guid);

            // allow anyone to read the object
            schemaBuilder.SetReadAccessLevel(AccessLevel.Public);

            // restrict writing to this vendor only
            schemaBuilder.SetWriteAccessLevel(
              AccessLevel.Vendor);

            // required because of restricted write-access
            schemaBuilder.SetVendorId("Intratech");

            // create a field to store an XYZ
            FieldBuilder fieldBuilder = schemaBuilder.AddSimpleField(roomTypeSchema, typeof(string));
            fieldBuilder.SetDocumentation("A stored indoorGML room type");

            fieldBuilder = schemaBuilder.AddSimpleField(roomFunctionSchema, typeof(string));
            fieldBuilder.SetDocumentation("A stored indoorGML room function ");

            schemaBuilder.SetSchemaName(indoorSchema);

           return schemaBuilder.Finish(); // register the Schema object
        }
        public void Add(Document doc, Element el, string roomType, string roomFunction)
        {
            Transaction createSchemaAndStoreData = new Transaction(doc, "tCreateAndStore");

            createSchemaAndStoreData.Start();

            if (schema == null)
                schema = GetSchema();
            Entity entity = new Entity(schema);
            // get the field from the schema
            entity.Set<string>(schema.GetField(roomTypeSchema), roomType); // set the value for this entity
            entity.Set<string>(schema.GetField(roomFunctionSchema), roomFunction); // set the value for this entity

            el.SetEntity(entity); // store the entity in the element

            createSchemaAndStoreData.Commit();
        }

        public void GetRoomName (Element el,out string roomType,out string roomFunction)
        {
            roomType = "";
            roomFunction = "";
           
            try
            {
                if (schema == null)
                    schema = GetSchema();
                var indoorInfo=   el.GetEntity(schema);
                if(indoorInfo != null && indoorInfo.Schema != null)
                {
                     roomType = indoorInfo.Get<string>(schema.GetField(roomTypeSchema));
                    roomFunction = indoorInfo.Get<string>(schema.GetField(roomFunctionSchema));
                }
            }
            catch { }
        }
    }
}
