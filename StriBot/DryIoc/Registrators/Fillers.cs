using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using DryIoc;
using StriBot.DryIoc.Implementations;
using StriBot.DryIoc.Interfaces;

namespace StriBot.DryIoc.Registrators
{
    public static class Fillers
    {
        private const int DefaultPriority = 10;

        public static IContainer RegisterFillers(this IContainer owner)
        {
            var containerFillers = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .FindAll()
                .SortedByPriority();

            foreach (var filler in containerFillers)
            {
                (Activator.CreateInstance(filler) as IContainerFiller)?.Fill(owner);
            }

            return owner;
        }

        private static IEnumerable<Type> FindAll(this Type[] types)
            => types.Where(type => !type.IsAbstract && typeof(IContainerFiller).IsAssignableFrom(type));

        private static IEnumerable<Type> SortedByPriority(this IEnumerable<Type> types)
            => types.OrderBy(Priority);

        private static int Priority(this Type type)
            => type.GetCustomAttribute<FillPriorityAttribute>(false)?.Priority ?? DefaultPriority;
    }
}