using System.Collections;
using System.Collections.Generic;

namespace PNetJson
{
    public static class DynamicEnumerable
    {
        public static IEnumerable<T> DynamicCast<T>(this IEnumerable source)
        {
#if UNITY_EDITOR ||UNITY_STANDALONE||UNITY_WII||UNITY_IOS||UNITY_ANDROID||UNITY_PS3||UNITY_PS4||UNITY_SAMSUNGTV||UNITY_XBOX360||UNITY_XBOXONE||UNITY_TIZEN||UNITY_TVOS||UNITY_WP_8||UNITY_WP_8_1||UNITY_WSA||UNITY_WEBGL
            var enumerator = source.GetEnumerator();
            while (enumerator.MoveNext())
            {
                yield return (T)(enumerator.Current);
            }
#else
            foreach (dynamic current in source)
            {
                yield return (T)(current);
            }
#endif
        }
    }

}
