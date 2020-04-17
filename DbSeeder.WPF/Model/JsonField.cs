using System;

namespace DbSeeder.WPF.Model
{
    public class JsonField<T>
    {
        public string FieldName { get; set; }
        public T FieldValue { get; set; }

        public JsonField(string fieldName, T fieldValue)
        {
            FieldName = fieldName;
            FieldValue = fieldValue;
        }

    }
}
