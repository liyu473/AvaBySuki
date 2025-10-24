using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace AvaAIChat.Extensions;

public static class ObjectExtension
{
    /// <summary>
    /// 将 source 的属性值全部复制到 target，对象本身不替换
    /// </summary>
    public static void UpdatePropertiesFrom<T>(this T target, T source)
        where T : class
    {
        if (target == null || source == null)
            return;

        var properties = typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite);

        foreach (var prop in properties)
        {
            var value = prop.GetValue(source);
            prop.SetValue(target, value);
        }
    }

    /// <summary>
    /// 高性能的属性复制（首次调用时编译表达式树，后续调用几乎是直接赋值的性能），对象本身不替换
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void UpdatePropertiesHighQualityFrom<T>(this T target, T source)
        where T : class
    {
        if (target == null || source == null)
            return;
        PropertyCopier<T>.CopyAction(target, source);
    }

    /// <summary>
    /// 高性能的属性复制，对象本身不替换，排除可通知集合
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="target"></param>
    /// <param name="source"></param>
    public static void UpdatePropertiesHighQualityExcludeGenericTypeFrom<T>(this T target, T source)
        where T : class
    {
        if (target == null || source == null)
            return;
        PropertyCopierExcludeGenericType<T>.CopyAction(target, source);
    }

    private static class PropertyCopier<T>
    {
        public static readonly Action<T, T> CopyAction;

        static PropertyCopier()
        {
            var target = Expression.Parameter(typeof(T), "target");
            var source = Expression.Parameter(typeof(T), "source");

            var assigns = typeof(T)
                .GetProperties()
                .Where(p => p.CanRead && p.CanWrite)
                .Select(p =>
                    Expression.Assign(
                        Expression.Property(target, p),
                        Expression.Property(source, p)
                    )
                );

            var body = Expression.Block(assigns);
            CopyAction = Expression.Lambda<Action<T, T>>(body, target, source).Compile();
        }
    }

    private static class PropertyCopierExcludeGenericType<T>
    {
        public static readonly Action<T, T> CopyAction;

        static PropertyCopierExcludeGenericType()
        {
            var target = Expression.Parameter(typeof(T), "target");
            var source = Expression.Parameter(typeof(T), "source");

            var statements = new List<Expression>();

            foreach (var p in typeof(T).GetProperties().Where(p => p.CanRead && p.CanWrite))
            {
                var targetProp = Expression.Property(target, p);
                var sourceProp = Expression.Property(source, p);

                // 针对 ObservableCollection<T> 和 BindingList<T> 特殊处理
                if (p.PropertyType.IsGenericType)
                {
                    var genType = p.PropertyType.GetGenericTypeDefinition();
                    if (
                        genType == typeof(ObservableCollection<>)
                        || genType == typeof(BindingList<>)
                    )
                    {
                        // if (target.Prop != null && source.Prop != null) { target.Prop.Clear();
                        // foreach (var item in source.Prop) target.Prop.Add(item); }

                        var itemType = p.PropertyType.GetGenericArguments()[0];
                        var enumerableType = typeof(IEnumerable<>).MakeGenericType(itemType);

                        var clearMethod = p.PropertyType.GetMethod("Clear")!;
                        var addMethod = p.PropertyType.GetMethod("Add")!;
                        var getEnumerator = enumerableType.GetMethod("GetEnumerator")!;

                        var enumeratorVar = Expression.Variable(
                            typeof(IEnumerator<>).MakeGenericType(itemType),
                            "enumerator"
                        );
                        var loopVar = Expression.Variable(itemType, "item");

                        var breakLabel = Expression.Label("LoopBreak");

                        var assignEnumerator = Expression.Assign(
                            enumeratorVar,
                            Expression.Call(sourceProp, getEnumerator)
                        );

                        var loop = Expression.Loop(
                            Expression.Block(
                                Expression.IfThenElse(
                                    Expression.Call(
                                        enumeratorVar,
                                        typeof(System.Collections.IEnumerator).GetMethod(
                                            "MoveNext"
                                        )!
                                    ),
                                    Expression.Block(
                                        [loopVar],
                                        Expression.Assign(
                                            loopVar,
                                            Expression.Property(enumeratorVar, "Current")
                                        ),
                                        Expression.Call(targetProp, addMethod, loopVar)
                                    ),
                                    Expression.Break(breakLabel)
                                )
                            ),
                            breakLabel
                        );

                        var block = Expression.Block(
                            [enumeratorVar],
                            Expression.IfThen(
                                Expression.AndAlso(
                                    Expression.NotEqual(targetProp, Expression.Constant(null)),
                                    Expression.NotEqual(sourceProp, Expression.Constant(null))
                                ),
                                Expression.Block(
                                    Expression.Call(targetProp, clearMethod),
                                    assignEnumerator,
                                    loop
                                )
                            )
                        );

                        statements.Add(block);
                        continue;
                    }
                }

                // 默认赋值
                statements.Add(Expression.Assign(targetProp, sourceProp));
            }

            var body = Expression.Block(statements);
            CopyAction = Expression.Lambda<Action<T, T>>(body, target, source).Compile();
        }
    }
}