using Microsoft.EntityFrameworkCore;
using persistences.Entities;
using System.Reflection;

namespace persistences.Extentions
{
    public static class ModelBuilderExtensions
    {
        public static void RegisterAllEntities<BaseModel>(this ModelBuilder modelBuilder, params Assembly[] assemblies)
        {
            IEnumerable<Type> types = assemblies.SelectMany(a => a.GetExportedTypes()).Where(c => c.IsClass && !c.IsAbstract && c.IsPublic &&
                c.Name != nameof(BaseEntity) &&
                typeof(BaseModel).IsAssignableFrom(c));
            foreach (Type type in types)
                modelBuilder.Entity(type);
        }
    }
}
