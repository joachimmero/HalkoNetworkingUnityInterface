using System;
using System.Collections.Generic;
using System.Reflection;
namespace HalkoNetworking
{
    public class HalkoAttributeHandler
    {
        //Public methods:

        /// <summary>
        /// Get the methods with an attribute of "attribute" of class "type".
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<MethodInfo> GetMethodsWithAttribute(TypeInfo type, Type attribute)
        {
            //Get all methods from class of type "type".
            MethodInfo[] methods = type.GetMethods(
                BindingFlags.Public |
                BindingFlags.NonPublic |
                BindingFlags.Instance
                );

            List<MethodInfo> methodsWithAttribute = new List<MethodInfo>();
            //Iterate through all the methods and check if they have
            //the attribute "attribute".
            for(int i = methods.Length - 1; i >= 0; --i)
            {
                //Get the custom attributes of the method.
                IEnumerable<Attribute> attributes = methods[i].GetCustomAttributes(attribute);
                //Check if the methods has any attributes.
                if (attributes != null)
                {
                    //Iterate through the attributes of the method.
                    foreach (Attribute a in attributes)
                    {
                        //Check if the attribute is of type "attribute".
                        if(a.GetType() == attribute)
                        {
                            //Add the method to the methodsWithAttribute-list.
                            methodsWithAttribute.Add(methods[i]);
                        }
                    }
                }
            }
            //Return a list of the methods in the class
            //with an attribute of type "attribute.
            return methodsWithAttribute;
        }
    }
}
