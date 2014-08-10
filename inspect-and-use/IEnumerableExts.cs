using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using com.tinylabproductions.TLPLib.Collection;
using com.tinylabproductions.TLPLib.Functional;

namespace com.tinylabproductions.TLPLib.Extensions {
  public static class IEnumerableExts {
    public static String asString(
      this IEnumerable enumerable, 
      bool newlines=true, bool fullClasses=false
    ) {
      var items = (
        from object item in enumerable
        let enumItem = item as IEnumerable
        select enumItem == null ? item.ToString() : enumItem.asString()
      ).ToArray();
      var itemsStr = 
        string.Join(string.Format(",{0} ", newlines ? "\n " : ""), items);
      if (items.Length != 0 && newlines) itemsStr = "\n  " + itemsStr + "\n";

      var type = enumerable.GetType();
      return string.Format(
        "{0}[{1}]",
        fullClasses ? type.FullName : type.Name,
        itemsStr
      );
    }

    public static string mkString(
      this IEnumerable enumerable, string sep, string start=null, string end=null
    ) {
      var b = new StringBuilder();
      var first = true;
      b.Append(start ?? "");
      foreach (var x in enumerable) {
        if (first) {
          b.Append(x);
          first = false;
        }
        else {
          b.Append(sep);
          b.Append(x);
        }
      }
      b.Append(end ?? "");

      return b.ToString();
    }

    public static Option<Tpl<T, int>> FindWithIndex<T>(
      this IEnumerable<T> enumerable, Fn<T, bool> predicate
    ) {
      var index = 0;
      foreach (var item in enumerable) {
        if (predicate(item)) return F.some(F.t(item, index));
        index += 1;
      }
      return F.none<Tpl<T, int>>();
    }

    public static Option<A> headOpt<A>(this IEnumerable<A> enumerable) {
      foreach (var e in enumerable) return F.some(e);
      return F.none<A>();
    }

    /**
     * Iterates the collection. Tries to find a member using predicate. If it
     * doesn't find one, returns last element of enumerable.
     * 
     * Returns None if enumerable is empty.
     **/
    public static Option<A> findOrLast<A>(
      this IEnumerable<A> enumerable, Fn<A, bool> predicate
    ) {
      var last = F.none<A>();
      foreach (var a in enumerable) {
        var current = F.some(a);
        if (predicate(a)) return current;
        last = current;
      }
      return last;
    }

    private static Option<A> minMax<A, B>(
      this IEnumerable<A> enumerable, Fn<A, B> selector, Fn<int, bool> decider
    ) {
      var aOpt = F.none<A>();
      var bOpt = F.none<B>();

      var comparer = Comparer<B>.Default;
      foreach (var a in enumerable) {
        var b = selector(a);
        if (
          // ReSharper disable once RedundantTypeArgumentsOfMethod
          // Mono Compiler bug.
          bOpt.fold<bool>(() => true, prevB => decider(comparer.Compare(b, prevB)))
        ) {
          aOpt = F.some(a);
          bOpt = F.some(b);
        }
      }

      return aOpt;
    }

    public static Option<A> min<A, B>(
      this IEnumerable<A> enumerable, Fn<A, B> selector
    ) {
      return enumerable.minMax(selector, _ => _ < 0);
    }

    public static Option<A> max<A, B>(
      this IEnumerable<A> enumerable, Fn<A, B> selector
    ) {
      return enumerable.minMax(selector, _ => _ > 0);
    }

    public static Option<T> FindOpt<T>(
      this IEnumerable<T> enumerable, Fn<T, bool> predicate
    ) {
      // ReSharper disable once RedundantTypeArgumentsOfMethod
      // Mono compiler bug.
      return enumerable.FindWithIndex(predicate).map<T>(t => t._1);
    }

    public static Option<B> FindFlatMap<A, B>(
      this IEnumerable<A> enumerable, Fn<A, Option<B>> predicate
    ) {
      foreach (var item in enumerable) {
        var opt = predicate(item);
        if (opt.isDefined) return opt;
      }
      return F.none<B>();
    }

    public static Option<int> IndexWhere<T>(
      this IEnumerable<T> enumerable, Fn<T, bool> predicate
    ) {
      // ReSharper disable once RedundantTypeArgumentsOfMethod
      // Mono compiler bug.
      return enumerable.FindWithIndex(predicate).map<int>(t => t._2);
    }

    /** Create enumerable with 1 element **/
    public static IEnumerable<T> Yield<T>(this T obj) {
      yield return obj;
    }

    public static IEnumerable<Tpl<A, B>> Zip<A, B>(
      this IEnumerable<A> enum1, IEnumerable<B> enum2, bool strict=true
    ) {
      var e1 = enum1.GetEnumerator();
      var e2 = enum2.GetEnumerator();

      bool hasMore;
      do {
        var hasE1 = e1.MoveNext();
        // Stop iterating if we're not strict and first enumerable is finished.
        if (!strict && !hasE1) break;

        var hasE2 = e2.MoveNext();
        if (hasE1 != hasE2) throw new Exception(
          "Cannot zip through enumerables that are not the same size! " +
          string.Format("E1: {0}, E2: {1}", hasE1, hasE2)
        );

        if (hasE1) yield return F.t(e1.Current, e2.Current);
        hasMore = hasE1;
      } while (hasMore);
    }

    public static IEnumerable<Tpl<A, int>> ZipWithIndex<A>(
      this IEnumerable<A> enumerable
    ) {
      var idx = -1;
      return enumerable.Select(v => {
        idx++;
        return F.t(v, idx);
      });
    }

    public static A RandomElementByWeight<A>(
      this IEnumerable<A> sequence, Func<A, float> weightSelector
    ) {
      var totalWeight = sequence.Sum(weightSelector);
      // The weight we are after...
      var itemWeightIndex = (float) (new Random().NextDouble() * totalWeight);
      var currentWeightIndex = 0f;

      foreach (
        var item in 
          from weightedItem in sequence 
          select new { Value = weightedItem, Weight = weightSelector(weightedItem) }
      ) {
        currentWeightIndex += item.Weight;

        // If we've hit or passed the weight we are after for this item then it's the one we want....
        if (currentWeightIndex >= itemWeightIndex)
          return item.Value;
      }

      throw new Exception();
    }

    public static void each<A>(this IEnumerable<A> enumerable, Act<A> action) {
      foreach (var a in enumerable) action(a);
    }

    public static void eachWithIndex<A>
    (this IEnumerable<A> enumerable, Act<A, uint> action) {
      var index = 0u;
      foreach (var a in enumerable) {
        action(a, index);
        index++;
      }
    }

    /**
     * Returns tuple of linked lists where first one contains all the items
     * that matched the predicate and second - those who didn't.
     **/
    public static Tpl<LinkedList<A>, LinkedList<A>> partition<A>(
      this IEnumerable<A> enumerable, Fn<A, bool> predicate
    ) {
      var trues = new LinkedList<A>();
      var falses = new LinkedList<A>();
      foreach (var a in enumerable) {
        if (predicate(a)) trues.AddLast(a);
        else falses.AddLast(a);
      }
      return F.t(trues, falses);
    }
  }
}


namespace UEx
{
    public static class IEnumerableExtensions
    {
        public static T MaxElement<T, TCompare>(this IEnumerable<T> collection, Func<T, TCompare> func)
        where TCompare : IComparable<TCompare>
        {
            T maxItem = default(T);
            TCompare maxValue = default(TCompare);

            if (collection == null)
                return maxItem;

            foreach (var item in collection)
            {
                TCompare temp = func(item);

                if (maxItem == null || temp.CompareTo(maxValue) > 0)
                {
                    maxValue = temp;
                    maxItem = item;
                }

            }
            return maxItem;
        }

        public static T[] RemoveRange<T>(this T[] array, int index, int count)
        {
            if (count < 0)
                throw new ArgumentOutOfRangeException("count", " is out of range");
            if (index < 0 || index > array.Length - 1)
                throw new ArgumentOutOfRangeException("index", " is out of range");

            if (array.Length - count - index < 0)
                throw new ArgumentException("index and count do not denote a valid range of elements in the array", "");

            var newArray = new T[array.Length - count];

            for (int i = 0, ni = 0; i < array.Length; i++)
            {
                if (i < index || i >= index + count)
                {
                    newArray[ni] = array[i];
                    ni++;
                }
            }

            return newArray;
        }
    }
}


//     Linq.cs
//     (c) 2013 Brett Ernst, Jameson Ernst, Robert Marsters, Gabriel Isenberg https://github.com/gisenberg/tabletop.io.gui
//     Licensed under the terms of the MIT license.

using System;
using System.Collections;
using System.Collections.Generic;

namespace Tabletop.io.Linq {
    public static class Enumerable {
        public static bool All<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            foreach (T o in src)
                if (!pred(o))
                    return false;
            return true;
        }

        public static bool Any<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            foreach (T o in src)
                if (pred(o))
                    return true;
            return false;
        }

        public static bool Any<T> (this IEnumerable<T> src) {
            foreach (T o in src)
                return true;
            return false;
        }

        public static IEnumerable<T> Cast<T> (this IEnumerable src) {
            foreach (var o in src)
                yield return (T)o;
        }

        public static IEnumerable<T> Concat<T> (this IEnumerable<T> src1, IEnumerable<T> src2) {
            foreach (T o in src1)
                yield return o;
            foreach (T o in src2)
                yield return o;
        }

        public static bool Contains<T> (this IEnumerable<T> src, T val) {
            foreach (T o in src)
                if (EqualityComparer<T>.Default.Equals(o, val))
                    return true;
            return false;
        }

        public static bool Contains<T> (this IEnumerable<T> src, T val, IEqualityComparer<T> cmp) {
            foreach (T o in src)
                if (cmp.Equals(o, val))
                    return true;
            return false;
        }

        public static int Count<T> (this IEnumerable<T> src) {
            var count = 0;
            foreach (T o in src)
                count++;
            return count;
        }

        public static int Count<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            var count = 0;
            foreach (T o in src)
                if (pred(o))
                    count++;
            return count;
        }

        public static IEnumerable<T> DefaultIfEmpty<T>(this IEnumerable<T> src, T def) {
            int values = 0;

            foreach (T o in src) {
                yield return o;
                values++;
            }

            if (values == 0)
                yield return def;
        }

        public static IEnumerable<T> Empty<T> () {
            yield break;
        }

        public static T FirstOrDefault<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            foreach (T o in src)
                if (pred(o))
                    return o;
            return default(T);
        }

        public static T FirstOrDefault<T> (this IEnumerable<T> src) {
            foreach (T o in src)
                return o;
            return default(T);
        }

        public static T First<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            foreach (T o in src)
                if (pred(o))
                    return o;
            throw new InvalidOperationException("Element not found.");
        }

        public static T First<T> (this IEnumerable<T> src) {
            foreach (T o in src)
                return o;
            throw new InvalidOperationException("Element not found.");
        }

        public static T Last<T> (this IEnumerable<T> src) {
            var ie = src.GetEnumerator();
            if (ie.MoveNext()) {
                T val = ie.Current;
                while (ie.MoveNext())
                    val = ie.Current;
                return val;
            } else
                throw new InvalidOperationException("Element not found.");
        }

        public static T Last<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            var ie = src.GetEnumerator();
            while (ie.MoveNext()) {
                if (pred(ie.Current)) {
                    T val = ie.Current;
                    while (ie.MoveNext()) {
                        if (pred(ie.Current))
                            val = ie.Current;
                    }
                    return val;
                }
            }
            throw new InvalidOperationException("Element not found.");
        }

        public static T LastOrDefault<T> (this IEnumerable<T> src) {
            T val = default(T);
            foreach (T o in src)
                val = o;
            return val;
        }

        public static T LastOrDefault<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            T val = default(T);
            foreach (T o in src)
                if (pred(o))
                    val = o;
            return val;
        }

        public static float Max (this IEnumerable<float> src) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            float acc = iee.Current;
            while (iee.MoveNext())
                acc = Math.Max(iee.Current, acc);
            return acc;
        }

        public static int Max (this IEnumerable<int> src) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            int acc = iee.Current;
            while (iee.MoveNext())
                acc = Math.Max(iee.Current, acc);
            return acc;
        }

        public static int Max<T> (this IEnumerable<T> src, Func<T, int> sel) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            int acc = sel(iee.Current);
            while (iee.MoveNext())
                acc = Math.Max(sel(iee.Current), acc);
            return acc;
        }

        public static float Max<T> (this IEnumerable<T> src, Func<T, float> sel) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            float acc = sel(iee.Current);
            while (iee.MoveNext())
                acc = Math.Max(sel(iee.Current), acc);
            return acc;
        }

        public static float Min (this IEnumerable<float> src) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            float acc = iee.Current;
            while (iee.MoveNext())
                acc = Math.Min(iee.Current, acc);
            return acc;
        }

        public static int Min (this IEnumerable<int> src) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            int acc = iee.Current;
            while (iee.MoveNext())
                acc = Math.Min(iee.Current, acc);
            return acc;
        }

        public static int Min<T> (this IEnumerable<T> src, Func<T, int> sel) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            int acc = sel(iee.Current);
            while (iee.MoveNext())
                acc = Math.Min(sel(iee.Current), acc);
            return acc;
        }

        public static float Min<T> (this IEnumerable<T> src, Func<T, float> sel) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                throw new InvalidOperationException("Empty enumeration");
            float acc = sel(iee.Current);
            while (iee.MoveNext())
                acc = Math.Min(sel(iee.Current), acc);
            return acc;
        }

        public static IEnumerable<T> OrderBy<T, K>(this IEnumerable<T> src, Func<T, K> keySelector) {
            var vals = new List<T>(src);

            vals.Sort((a, b) => Comparer.Default.Compare(keySelector(a), keySelector(b)));

            return vals;
        }

        public static IEnumerable<T> OfType<T> (this IEnumerable src) {
            foreach (object o in src)
                if (o is T)
                    yield return (T)o;
        }

        public static IEnumerable<int> Range (int start, int count) {
            while (count-- > 0)
                yield return start++;
        }

        public static IEnumerable<T> Repeat<T> (T elem, int count) {
            while (count-- > 0)
                yield return elem;
        }

        public static IEnumerable<T> Reverse<T> (this IEnumerable<T> src) {
            var stack = new Stack<T>();
            foreach (T o in src)
                stack.Push(o);
            while (stack.Count > 0)
                yield return stack.Pop();
        }

        public static IEnumerable<U> Select<T, U> (this IEnumerable<T> src, Func<T, U> pred) {
            foreach (T o in src)
                yield return pred(o);
        }

        public static IEnumerable<U> Select<T, U> (this IEnumerable<T> src, Func<T, int, U> pred) {
            var idx = 0;
            foreach (T o in src)
                yield return pred(o, idx++);
        }

        public static IEnumerable<U> SelectMany<T, U> (this IEnumerable<T> src, Func<T, IEnumerable<U>> sel) {
            foreach (T o in src)
                foreach (U o2 in sel(o))
                    yield return o2;
        }

        public static IEnumerable<U> SelectMany<T, U> (this IEnumerable<T> src, Func<T, int, IEnumerable<U>> sel) {
            var idx = 0;
            foreach (T o in src)
                foreach (U o2 in sel(o, idx++))
                    yield return o2;
        }

        public static bool SequenceEqual<T> (this IEnumerable<T> src1, IEnumerable<T> src2) {
            return src1.SequenceEqual(src2, EqualityComparer<T>.Default);
        }

        public static bool SequenceEqual<T> (this IEnumerable<T> src1, IEnumerable<T> src2, IEqualityComparer<T> cmp) {
            var ie1 = src1.GetEnumerator();
            var ie2 = src2.GetEnumerator();
            bool s1, s2 = false;
            while ((s1 = ie1.MoveNext()) && (s2 = ie2.MoveNext())) {
                if (!cmp.Equals(ie1.Current, ie2.Current))
                    return false;
            }
            return !s1 && !s2;
        }

        public static T Single<T> (this IEnumerable<T> src) {
            var ie = src.GetEnumerator();
            if (ie.MoveNext()) {
                T val = ie.Current;
                if (ie.MoveNext())
                    throw new InvalidOperationException("More than one element.");
                return val;
            }
            throw new InvalidOperationException("Element not found.");
        }

        public static T Single<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            var ie = src.GetEnumerator();
            while (ie.MoveNext()) {
                if (pred(ie.Current)) {
                    T val = ie.Current;
                    while(ie.MoveNext())
                        if(pred(ie.Current))
                            throw new InvalidOperationException("More than one element.");
                    return val;
                }
            }
            throw new InvalidOperationException("Element not found.");
        }

        public static IEnumerable<T> Skip<T> (this IEnumerable<T> src, int num) {
            foreach (T o in src) {
                if (num > 0)
                    num--;
                else
                    yield return o;
            }
        }

        public static IEnumerable<T> SkipWhile<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            var ie = src.GetEnumerator();
            while (ie.MoveNext()) {
                if (!pred(ie.Current)) {
                    do {
                        yield return ie.Current;
                    } while (ie.MoveNext());
                    yield break;
                }
            }
        }

        public static IEnumerable<T> SkipWhile<T> (this IEnumerable<T> src, Func<T, int, bool> pred) {
            var ie = src.GetEnumerator();
            var idx = 0;
            while (ie.MoveNext()) {
                if (!pred(ie.Current, idx++)) {
                    do {
                        yield return ie.Current;
                    } while (ie.MoveNext());
                    yield break;
                }
            }
        }

        public static float Sum (this IEnumerable<float> src) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                return 0.0f;
            float acc = iee.Current;
            while (iee.MoveNext())
                acc += iee.Current;
            return acc;
        }

        public static int Sum (this IEnumerable<int> src) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                return 0;
            int acc = iee.Current;
            while (iee.MoveNext())
                acc += iee.Current;
            return acc;
        }

        public static int Sum<T> (this IEnumerable<T> src, Func<T, int> sel) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                return 0;
            int acc = sel(iee.Current);
            while (iee.MoveNext())
                acc += sel(iee.Current);
            return acc;
        }

        public static float Sum<T> (this IEnumerable<T> src, Func<T, float> sel) {
            var iee = src.GetEnumerator();
            if (!iee.MoveNext())
                return 0.0f;
            float acc = sel(iee.Current);
            while (iee.MoveNext())
                acc += sel(iee.Current);
            return acc;
        }

        public static IEnumerable<T> Take<T> (this IEnumerable<T> src, int num) {
            foreach (T o in src) {
                if (num > 0) {
                    num--;
                    yield return o;
                } else
                    yield break;
            }
        }

        public static IEnumerable<T> TakeWhile<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            foreach (T o in src) {
                if (!pred(o))
                    yield break;
                yield return o;
            }
        }

        public static IEnumerable<T> TakeWhile<T> (this IEnumerable<T> src, Func<T, int, bool> pred) {
            var idx = 0;
            foreach (T o in src) {
                if (!pred(o, idx++))
                    yield break;
                yield return o;
            }
        }

        public static T[] ToArray<T> (this IEnumerable<T> src) {
            var coll = src as ICollection<T>;
            if (coll != null) {
                var arr = new T[coll.Count];
                coll.CopyTo(arr, 0);
                return arr;
            } else {
                return src.ToList().ToArray();
            }
        }

        public static Dictionary<U, T> ToDictionary<T, U> (this IEnumerable<T> src, Func<T, U> selKey, IEqualityComparer<U> cmp) {
            var dict = new Dictionary<U, T>(cmp);
            foreach (T o in src)
                dict.Add(selKey(o), o);
            return dict;
        }

        public static Dictionary<U, V> ToDictionary<T, U, V> (this IEnumerable<T> src, Func<T, U> selKey, Func<T, V> selElem, IEqualityComparer<U> cmp) {
            var dict = new Dictionary<U, V>(cmp);
            foreach (T o in src)
                dict.Add(selKey(o), selElem(o));
            return dict;
        }

        public static Dictionary<U, T> ToDictionary<T, U> (this IEnumerable<T> src, Func<T, U> selKey) {
            return src.ToDictionary(selKey, EqualityComparer<U>.Default);
        }

        public static Dictionary<U, V> ToDictionary<T, U, V> (this IEnumerable<T> src, Func<T, U> selKey, Func<T, V> selElem) {
            return src.ToDictionary(selKey, selElem, EqualityComparer<U>.Default);
        }

        public static List<T> ToList<T> (this IEnumerable<T> src) {
            var list = new List<T>();
            list.AddRange(src);
            return list;
        }

        public static IEnumerable<T> Where<T> (this IEnumerable<T> src, Func<T, bool> pred) {
            foreach (T o in src)
                if (pred(o))
                    yield return o;
        }

        public static IEnumerable<T> Where<T> (this IEnumerable<T> src, Func<T, int, bool> pred) {
            var idx = 0;
            foreach (T o in src)
                if (pred(o, idx++))
                    yield return o;
        }
    }
}




