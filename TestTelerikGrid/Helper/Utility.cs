using Newtonsoft.Json;

namespace TestTelerikGrid.Helper
{
    public static class Utility
    {
        internal static string GenerateComponentId(string prefix)
        {
            return prefix + Guid.NewGuid().ToString();
        }

        #region IEnumerable
        internal static IEnumerable<T> Add<T>(this IEnumerable<T> source, T item)
        {
            var loListItems = source.ToList();
            loListItems.Add(item);

            return loListItems.AsEnumerable();
        }
        #endregion

        #region Object
        internal static T Clone<T>(this T self)
        {
            var serialized = JsonConvert.SerializeObject(self);

            return JsonConvert.DeserializeObject<T>(serialized);
        }

        internal static TResult ConvertObjectToObject<TResult>(object poSource)
        {
            var loJsonSerialized = JsonConvert.SerializeObject(poSource);

            return JsonConvert.DeserializeObject<TResult>(loJsonSerialized);
        }

        public static bool CompareObject(this object poObj, object poAnotherObj)
        {
            if (ReferenceEquals(poObj, poAnotherObj)) return true;

            if ((poObj == null) || (poAnotherObj == null)) return false;

            if (poObj.GetType() != poAnotherObj.GetType()) return false;

            var objJson = JsonConvert.SerializeObject(poObj);
            var anotherObjJson = JsonConvert.SerializeObject(poAnotherObj);

            return objJson == anotherObjJson;
        }
        #endregion
    }
}
