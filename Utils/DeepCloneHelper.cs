using System;
using System.Collections;
using System.Reflection;

namespace JYW.Game.Utils
{
    static class DeepCloneHelper
    {
        static readonly BindingFlags BF = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

        public static object Clone(object src)
        {
            if (src == null) return null;

            var t = src.GetType();

            // UnityEngine.Object 계열은 복제하지 않고 참조 유지
            if (typeof(UnityEngine.Object).IsAssignableFrom(t)) return src;

            // 값 형식/문자열/decimal 등 불변 객체는 그대로 반환
            if (t.IsValueType || t == typeof(string)) return src;

            // 배열
            if (t.IsArray)
            {
                var arr = (Array)src;
                var elemType = t.GetElementType();
                var cloned = Array.CreateInstance(elemType, arr.Length);
                for (int i = 0; i < arr.Length; i++)
                    cloned.SetValue(Clone(arr.GetValue(i)), i);
                return cloned;
            }

            // IList (List<T> 등)
            if (typeof(IList).IsAssignableFrom(t))
            {
                var list = (IList)src;
                var cloned = (IList)Activator.CreateInstance(t);
                foreach (var item in list) cloned.Add(Clone(item));
                return cloned;
            }

            // IDictionary (Dictionary<K,V> 등)
            if (typeof(IDictionary).IsAssignableFrom(t))
            {
                var dict = (IDictionary)src;
                var cloned = (IDictionary)Activator.CreateInstance(t);
                foreach (DictionaryEntry e in dict) cloned.Add(Clone(e.Key), Clone(e.Value));
                return cloned;
            }

            // 기타 참조형: 필드 단위 복사
            var inst = Activator.CreateInstance(t, true); // 비공개 생성자 허용
            foreach (var f in t.GetFields(BF))
            {
                // [NonSerialized] 필드는 건너뜀
                if (Attribute.IsDefined(f, typeof(NonSerializedAttribute))) continue;

                var value = f.GetValue(src);
                f.SetValue(inst, Clone(value));
            }
            return inst;
        }
    }
}