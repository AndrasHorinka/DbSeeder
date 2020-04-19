namespace DbSeeder.WPF.Model
{
    public class JsonField
    {
        public string FieldName { get; set; }
        public string FieldType { get; set; }
        public object FieldValue { get; set; }

        public JsonField(string fieldName, string fieldType)
        {
            FieldName = fieldName;
            FieldType = fieldType;
        }

    }
}
