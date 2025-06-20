using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json.Serialization;

namespace gamecore.actionsystem
{
    public class InternalContractResolver : DefaultContractResolver
    {
        protected override List<MemberInfo> GetSerializableMembers(Type objectType)
        {
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            var members = new List<MemberInfo>();

            members.AddRange(objectType.GetFields(flags));
            members.AddRange(objectType.GetProperties(flags));

            return members;
        }
    }
}
