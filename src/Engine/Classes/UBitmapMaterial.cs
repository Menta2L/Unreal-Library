using System;
using UELib.Core;
using UELib;
using System.Reflection;

namespace UELib.Engine
{

    public class UBitmapMaterial : URenderedMaterial 
    {
        public byte UBits{ get; set; }
        public byte VBits { get; set; }	// texture size log2 (number of bits in size value)
        public int USize{ get; set; }
        public int VSize { get; set; }
        public int UClamp{ get; set; }
        public int VClamp{ get; set; }
        public ETextureFormat Format;
        protected override void Deserialize()
        {
            base.Deserialize();
            foreach (UDefaultProperty property in Properties)
            {
                string propertyVal = property.Decompile();

                var kv = propertyVal.Split(new char[] { '=' }, StringSplitOptions.RemoveEmptyEntries);
                if (property.Name == "Format")
                {
                    Format = (ETextureFormat)Convert.ToInt32(kv[1]);
                }
                else
                {
                    SetValue(kv[0], kv[1]);
                }
            }
        }
        // probably not a bad idea to make extension to UObject
        public void SetValue(string propertyName, object propertyVal)
        {
            Type type = this.GetType();

            PropertyInfo propertyInfo = type.GetProperty(propertyName);
            if (propertyInfo == null)
            {
                return;
            }
            Type propertyType = propertyInfo.PropertyType;
            var targetType = IsNullableType(propertyInfo.PropertyType) ? Nullable.GetUnderlyingType(propertyInfo.PropertyType) : propertyInfo.PropertyType;
            propertyVal = Convert.ChangeType(propertyVal, targetType);
            propertyInfo.SetValue(this, propertyVal, null);

        }
        private static bool IsNullableType(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
        }
    }
}
