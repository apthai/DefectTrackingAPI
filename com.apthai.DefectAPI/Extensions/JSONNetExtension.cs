using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Newtonsoft.Json.Extensions
{



    public static class JSONNetExtension
    {

        // recursively yield all children of json
        public static IEnumerable<JToken> GetJTokenAllChildren(this JToken json)
        {
            foreach (var c in json.Children())
            {
                yield return c;
                foreach (var cc in GetJTokenAllChildren(c))
                {
                    yield return cc;
                }
            }
        }

        //http://stackoverflow.com/questions/11676159/json-net-how-to-remove-nodes
        public static void RemoveFields(this JToken token, params string[] fields)
        {
            JContainer container = token as JContainer;
            if (container == null) return;

            List<JToken> removeList = new List<JToken>();
            foreach (JToken el in container.Children())
            {
                JProperty p = el as JProperty;
                if (p != null && fields.Contains(p.Name))
                {
                    removeList.Add(el);
                }
                el.RemoveFields(fields);
            }

            foreach (JToken el in removeList)
            {
                el.Remove();
            }
        }

        public static List<JToken> FindTokens(this JToken containerToken, string[] names)
        {
            List<JToken> matches = new List<JToken>();
            FindTokens(containerToken, names, matches);
            return matches;
        }

        private static void FindTokens(JToken containerToken, string[] names, List<JToken> matches)
        {
            if (containerToken.Type == JTokenType.Object)
            {
                foreach (JProperty child in containerToken.Children<JProperty>())
                {
                    foreach (var name in names)
                    {
                        if (child.Name == name)
                        {
                            matches.Add(child.Value);
                        }
                        FindTokens(child.Value, names, matches);
                    }
                }
            }
            else if (containerToken.Type == JTokenType.Array)
            {
                foreach (JToken child in containerToken.Children())
                {
                    FindTokens(child, names , matches);
                }
            }
        }




        public static T GetValue<T>(this JToken jToken, string key,
                            T defaultValue = default(T))
        {
            T returnValue = defaultValue;

            if (jToken==null)
                return returnValue; 

            if (jToken[key] != null)
            {
                object data = null;
                string sData = jToken[key].ToString();

                var type = typeof(T);


                if (type == typeof(int))
                    data = int.Parse(sData);

                else if (type == typeof(int?))
                    if (string.IsNullOrEmpty(sData.Trim()) || string.IsNullOrWhiteSpace(sData.Trim()))
                        data = null;
                    else
                        data = int.Parse(sData);

                else if (type == typeof(bool))
                    data = bool.Parse(sData);

                else if (type == typeof(float))
                    data = float.Parse(sData);

                else if (type == typeof(DateTime))
                    data = DateTime.Parse(sData);

                else if (type == typeof(DateTime?))
                {
                    if (!string.IsNullOrEmpty(sData))
                        data = DateTime.Parse(sData);
                }

                    
                else if (type == typeof(double))
                    data = double.Parse(sData);

                else if (type == typeof(string))
                    data = sData;

                else if (type == typeof(Guid))
                    data = Guid.Parse(sData);

                //if (null == data && type.IsValueType)
                //    throw new ArgumentException("Cannot parse type \"" +
                //        type.FullName + "\" from value \"" + sData + "\"");

                //returnValue = (T)Convert.ChangeType(data,
                //    type, System.Globalization.CultureInfo.InvariantCulture);


                var isNullable = (Nullable.GetUnderlyingType(type) != null); // Nullable<T>
                if (!isNullable)
                {
                    if (null == data && type.IsValueType)
                        throw new ArgumentException("Cannot parse type \"" +
                            type.FullName + "\" from value \"" + sData + "\"");

                    returnValue = (T)Convert.ChangeType(data,
                        type, System.Globalization.CultureInfo.InvariantCulture);
                }
                else
                {
                    returnValue = (T)data;

                }


            }

            return returnValue;
        }


    }
}
