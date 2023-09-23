namespace Assets.Core.Entity
{
    //[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    //public sealed class EntityAttribute : Attribute
    //{
    //    public EntityType type;
    //    static Dictionary<EntityType, Type> entities = new Dictionary<EntityType, Type>();
    //    public EntityAttribute(EntityType type)
    //    {
    //        this.type = type;
    //    }
    //    static EntityAttribute()
    //    {
    //        foreach (var type in ReflectionCache.AllTypes)
    //        {
    //            var attr = type.GetCustomAttribute<EntityAttribute>();
    //            if (attr != null)
    //            {
    //                entities.Add(attr.type, type);
    //            }
    //        }
    //    }
    //    public static bool SpawnEntity<T>(EntityType type, out T instance)
    //    {
    //        var (i, e) = SpawnEntity<T>(type);
    //        instance = i;
    //        return !e;
    //    }
    //    public static (T instance, bool error) SpawnEntity<T>(EntityType type)
    //    {
    //        if (entities.TryGetValue(type, out var ent_type))
    //        {
    //            return ((T)Activator.CreateInstance(ent_type), false);
    //        }
    //        return (default(T), true);
    //    }
    //}
}