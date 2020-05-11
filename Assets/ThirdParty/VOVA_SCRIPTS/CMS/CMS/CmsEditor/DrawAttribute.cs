using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace CMS.Editor
{

    public enum DrawAttributeTypes
    {
        None,
        NotForDraw,
        IdPopup,
        ObjectSellectionField,
        ScriptableSelectionWindow,


    }

    [AttributeUsage(AttributeTargets.Field)]
    public class DrawAttribute: System.Attribute
    {
        public DrawAttributeTypes DrawAttributeTypes;
        public string Label = "";
        //public object match;
        public DrawAttribute(DrawAttributeTypes drawAttributeTypes, string label)
        {
            this.DrawAttributeTypes = drawAttributeTypes;
            this.Label = label;
        }
        public DrawAttribute(DrawAttributeTypes drawAttributeTypes)
        {
            this.DrawAttributeTypes = drawAttributeTypes;
        }
        public DrawAttribute(string label)
        {
            this.Label = label;
        }
    }
}
