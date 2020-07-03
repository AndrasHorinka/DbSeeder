using DbSeeder.WPF.ViewModels;
using System;
using System.Diagnostics;

namespace DbSeeder.WPF.Services
{
    public static class ExtensionMethods
    {
        /// <summary>
        ///  Copies references and values of source object to result object
        /// </summary>
        /// <param name="source">JsonFieldViewModel: the source object to be copied</param>
        /// <param name="result">JsonFieldViewModel: the result object which will hold the same references</param>
        public static bool CreateDeepCopy(this JsonFieldViewModel source, out JsonFieldViewModel result)
        {
            Type sourceType = source.GetType();
            result = new JsonFieldViewModel();
            Type resultType = result.GetType();

            try
            {
                System.Reflection.FieldInfo[] fields = sourceType.GetFields(System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var field in fields)
                {
                    var valueOfSource = sourceType.GetField(field.Name, System.Reflection.BindingFlags.DeclaredOnly).GetValue(source);
                    resultType.GetField(field.Name).SetValue(result, valueOfSource);
                }

                System.Reflection.PropertyInfo[] properties = sourceType.GetProperties(System.Reflection.BindingFlags.DeclaredOnly);
                foreach (var property in properties)
                {
                    var valueOfSource = sourceType.GetProperty(property.Name, System.Reflection.BindingFlags.DeclaredOnly).GetValue(source);
                    resultType.GetProperty(property.Name).SetValue(result, valueOfSource);
                }

                return true;
            }
            catch (Exception e)
            {
                Trace.WriteLine(e.Message);

                return false;
            }
        }

    }
}
